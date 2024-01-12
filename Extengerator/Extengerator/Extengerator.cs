using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using Extengerator.Common.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

using Context = Microsoft.CodeAnalysis.IncrementalGeneratorInitializationContext;

namespace Extengerator;

[Generator]
public class Extengerator : IIncrementalGenerator
{
    #region Initialization

    public void Initialize(Context context)
    {
        var namesAndContents = CreateAdditionalTextProvider(context);
        var apiClasses = CreateSyntaxProvider(context);

        context.RegisterSourceOutput(namesAndContents.Combine(apiClasses), GenerateCode);
    }

    private static IncrementalValuesProvider<string> CreateAdditionalTextProvider(Context context)
    {
        return context.AdditionalTextsProvider
            .Where(a => a.Path.EndsWith("Extengerator.settings.yaml"))
            .Select((text, cancellationToken)
                => text.GetText(cancellationToken)?.ToString())
            .Where(content => content is not null)!;
    }

    private static IncrementalValueProvider<ImmutableArray<Target>> CreateSyntaxProvider(Context context)
    {
        return context.SyntaxProvider
            .CreateSyntaxProvider(predicate: static (s, _) => IsTarget(s),
                transform: static (n, cn) => GetTarget(n, cn))
            .Where(static m => m.IsValid)
            .Collect();
    }
    
    private static bool IsTarget(SyntaxNode node) => node is ClassDeclarationSyntax;
    
    private static Target GetTarget(GeneratorSyntaxContext ctx, CancellationToken cn)
    {
        var syntax = (ClassDeclarationSyntax) ctx.Node;
        var symbol = ctx.SemanticModel.GetDeclaredSymbol(syntax, cn);

        if (symbol is null || symbol.IsAbstract || !symbol.AllInterfaces.Any())
            return Target.DefaultError;

        return new Target(symbol.ToDisplayString(), symbol.AllInterfaces.Select(i => i.ToDisplayString()));
    }
    
    #endregion

    #region Generation
    
    private static void GenerateCode(SourceProductionContext context,
        (string  content, ImmutableArray<Target> typeList) arg)
    {
        var typeList = arg.typeList;
        if (typeList.IsDefaultOrEmpty)
            return;

        var configurations = DeserializeAdditionalText(arg.content);
        
        // ReSharper disable once ForCanBeConvertedToForeach
        for (var i = 0; i < configurations.Count; ++i)
            GenerateCodeForConfiguration(context, typeList, configurations[i]);
    }

    private static List<Configuration> DeserializeAdditionalText(string content)
    {
        try
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var configurations = deserializer.Deserialize<List<Configuration>>(content);
            return configurations;
        }
        catch (YamlException e)
        {
            //TODO: error handling
            Console.WriteLine(e);
            throw;
        }
        catch (Exception e)
        {
            //TODO: error handling
            Console.WriteLine(e);
            throw;
        }
    }

    private static void GenerateCodeForConfiguration(SourceProductionContext context,
        ImmutableArray<Target> typeList,
        Configuration conf)
    {
        context.CancellationToken.ThrowIfCancellationRequested();

        var replaced = new object[conf.Replacer.Length];
        for (var i = 0; i < conf.Replacer.Length; ++i)
            replaced[i] = ApplyReplacer(context, typeList, conf.Replacer[i], conf.InterfaceType);

        //TODO AK: error handling
        var theCode = string.Format(conf.Template, replaced);

        context.AddSourceNormalized($"{conf.FileName}.g.cs", theCode);
    }

    private static string ApplyReplacer(SourceProductionContext context,
        ImmutableArray<Target> typeList, 
        string replacer,
        string interfaceType)
    {
        StringBuilder builder = new();
        
        // ReSharper disable once ForCanBeConvertedToForeach
        for (var i = 0; i < typeList.Length; ++i)
        {
            context.CancellationToken.ThrowIfCancellationRequested();
                
            var target = typeList[i];
            if (!target.Interfaces.Contains(interfaceType))//TODO AK: error handling
                continue;

            //TODO AK: error handling
            builder.AppendFormat(replacer, target.Class);
        }

        return builder.ToString();
    }

    #endregion

    // private static void ReportWarning(SourceProductionContext context, (string Name, bool Warn, bool Valid) api)
    // {
    //     // Write error out if no constructors have zero parameters
    //
    //     //TOOD AK: we do n't have any warnings here
    //     context.ReportDiagnostic(Diagnostic.Create(
    //         new DiagnosticDescriptor(
    //             "SG0001",
    //             "Api Classes should have an empty constructor",
    //             "Cannot register {0} class without an empty constructor. Using state {0} class's constructor will create an unintentional singleton",
    //             "Problem",
    //             DiagnosticSeverity.Warning,
    //             true),
    //         Location.None,
    //         api.Name));
    // }
}