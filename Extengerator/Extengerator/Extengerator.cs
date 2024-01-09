using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Extengerator;

[Generator]
public class Extengerator : IIncrementalGenerator
{


    //TODO AK: ImmutableArray<string> Interfaces Replacer?
    private readonly record struct Configuration(string InterfaceType, string Template, string[] Replacer, string FileName);
    private readonly record struct Target(string Class, IEnumerable<string> Interfaces, bool IsValid = true);


    private static readonly Target DefaultError = new Target("", Enumerable.Empty<string>(), false);
    

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        //config reading
        var namesAndContents = context.AdditionalTextsProvider
            .Where(a => a.Path.EndsWith("Extengerator.settings.yaml"))
            .Select((text, cancellationToken)
                => (name: Path.GetFileNameWithoutExtension(text.Path),
                    content: text.GetText(cancellationToken)?.ToString()))
            .Where(tuple => tuple.content is not null);
        

        //creating code
        var apiClasses = context.SyntaxProvider
            .CreateSyntaxProvider(predicate: static (s, _) => IsTarget(s),
                transform: (n, cn) => GetTarget(n, cn))
            .Where(static m => m.IsValid)
            .Collect();
        
        context.RegisterSourceOutput(namesAndContents.Combine(apiClasses), Execute!);

    }

    private void Execute(SourceProductionContext context, ((string name, string? content) file, ImmutableArray<Target> typeList) arg)
    {
        var file = arg.file;
        var typeList = arg.typeList;
        
        //TODO AK: error hanlding
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        var configurations = deserializer.Deserialize<List<Configuration>>(file.content);
        
        if (typeList.IsDefaultOrEmpty || !typeList.Any())
            return;

        foreach (var configuration in configurations)
            CreateCodeForConfiguration(context, typeList, configuration);
    }
    
    private static void CreateCodeForConfiguration(SourceProductionContext context, ImmutableArray<Target> typeList,
        Configuration conf)
    {
        context.CancellationToken.ThrowIfCancellationRequested();
        
        //TODO AK: clean up
        var replaced = new object[conf.Replacer.Length];
        for (var i = 0; i < conf.Replacer.Length; i++)
        {
            StringBuilder builder = new();
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var j = 0; j < typeList.Length; j++)
            {
                var clazz = typeList[j];
                if(!clazz.Interfaces.Contains(conf.InterfaceType))
                    continue;
            
                //TODO AK: error handling
                builder.AppendFormat(conf.Replacer[i], clazz.Class);
            }

            replaced[i] = builder.ToString();
        }

        //TODO AK: error handling
        var theCode = string.Format(conf.Template, replaced);
        
        theCode = CSharpSyntaxTree.ParseText(theCode).GetRoot().NormalizeWhitespace().ToFullString();
        context.AddSource($"{conf.FileName}.g.cs", SourceText.From(theCode, Encoding.UTF8));
    }


    private static bool IsTarget(SyntaxNode node) => node is ClassDeclarationSyntax;
    
    
    private static Target GetTarget(GeneratorSyntaxContext ctx, CancellationToken cn)
    {
        
        var syntax = (ClassDeclarationSyntax)ctx.Node;
        var symbol = ctx.SemanticModel.GetDeclaredSymbol(syntax, cn);

        if (symbol is null ||symbol.IsAbstract || !symbol.AllInterfaces.Any()) 
            return DefaultError;

        return new Target(symbol.ToDisplayString(), symbol.AllInterfaces.Select(i => i.ToDisplayString()));
        
    }



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