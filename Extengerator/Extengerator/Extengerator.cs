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
        if(configurations is null)
            return;
        
        // ReSharper disable once ForCanBeConvertedToForeach
        for (var i = 0; i < configurations.Count; ++i)
            GenerateCodeForConfiguration(context, typeList, configurations[i]);
    }

    private static List<Configuration>? DeserializeAdditionalText(string content)
    {
        try
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            return deserializer.Deserialize<List<Configuration>>(content);
        }
        catch (YamlException e)
        {
            //TODO: error handling
            Console.WriteLine(e);
            return null;
        }
        catch (Exception e)
        {
            //TODO: error handling
            Console.WriteLine(e);
            return null;
        }
    }

    private static void GenerateCodeForConfiguration(SourceProductionContext context,
        ImmutableArray<Target> typeList,
        Configuration conf)
    {
        context.CancellationToken.ThrowIfCancellationRequested();

        if (!conf.IsValid())
        {
            //TODO AK: error handling
            return;
        }

        var replaced = new object[conf.Replacer!.Length];
        for (var i = 0; i < conf.Replacer.Length; ++i)
            replaced[i] = ApplyReplacer(context, typeList, conf.Replacer[i], conf.InterfaceType!);

        try
        {
            var sourceCode = string.Format(conf.Template!, replaced);
            context.AddSourceNormalized($"{conf.FileName}.g.cs", sourceCode);
        }
        catch (ArgumentNullException e)
        {
            //TODO AK: error handling
            //conf.Template is null
            Console.WriteLine(e);
            throw;
        }
        catch (FormatException e)
        {
            //TODO AK: error handling
            //The format item in conf.Template is invalid. or The index of a format item is not zero.
            Console.WriteLine(e);
            throw;
        }
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
            if (!target.Interfaces.Contains(interfaceType))
                continue;

            //TODO AK: error handling
            try
            {
                builder.AppendFormat(replacer, target.Class);
            }
            catch (ArgumentNullException e)
            {
                //TODO AK: error handling
                //replacer is null
                Console.WriteLine(e);
                throw;
            }
            catch (FormatException e)
            {
                //TODO AK: error handling
                //The format item in replacer is invalid. or The index of a format item is not zero.
                Console.WriteLine(e);
                throw;
            }
            catch (ArgumentOutOfRangeException e)
            {
                //TODO AK: error handling
                //The length of the expanded string would exceed MaxCapacity.
                Console.WriteLine(e);
                throw;
            }
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