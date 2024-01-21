using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using Extengerator.Common.CodeAnalysis;
using Extengerator.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

    private static IncrementalValuesProvider<string?> CreateAdditionalTextProvider(Context context)
    {
        return context.AdditionalTextsProvider
            .Where(a => a.Path.EndsWith("Extengerator.settings.yaml"))
            .Select((text, cancellationToken)
                => text.GetText(cancellationToken)?.ToString());
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

        return new Target(symbol.ToDisplayString(), symbol.Interfaces.Select(i => i.ToDisplayString()));
    }

    #endregion

    #region Generation

    private static void GenerateCode(SourceProductionContext context,
        (string? content, ImmutableArray<Target> typeList) arg)
    {
        var typeList = arg.typeList;
        if (typeList.IsDefaultOrEmpty)
            return;

        if (arg.content is null)
        {
            ReportConfigurationWarning(context, "Configuration 'Extengerator.settings.yaml' not found.");
            return;
        }

        var configurations = DeserializeAdditionalText(context, arg.content);
        if (configurations is null)
            return;

        // ReSharper disable once ForCanBeConvertedToForeach
        for (var i = 0; i < configurations.Count; ++i)
            GenerateCodeForConfiguration(context, typeList, configurations[i]);
    }

    private static List<Configuration>? DeserializeAdditionalText(SourceProductionContext context, string content)
    {
        try
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            return deserializer.Deserialize<List<Configuration>>(content);
        }
        catch (Exception e)
        {
            ReportSettingsWarning(context, e.Message);
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
            ReportSettingsWarning(context, "Invalid configuration: {0}", conf);
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
        catch (ArgumentNullException)
        {
            ReportConfigurationWarning(context,
                "Invalid parameter for {0} in configuration {1}",
                nameof(Configuration.Template),
                conf);
            throw;
        }
        catch (FormatException)
        {
            ReportConfigurationWarning(context,
                "Invalid format item '{0}' in configuration {1}.  Did you forgot to escape '{{' with '{{{{' for source code?",
                nameof(Configuration.Template),
                conf);
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

            try
            {
                builder.AppendFormat(replacer, target.Class);
            }
            catch (ArgumentNullException)
            {
                ReportConfigurationWarning(context,
                    "Failed processing of configuration.",
                    "Invalid parameter for {0}. Received: '{1}'",
                    nameof(Configuration.Replacer),
                    replacer);
            }
            catch (FormatException)
            {
                ReportConfigurationWarning(context,
                    "Invalid format item '{0}'. Received: '{1}'. Did you forgot to escape '{{' with '{{{{' for source code?",
                    nameof(Configuration.Replacer),
                    replacer);
            }
            catch (ArgumentOutOfRangeException)
            {
                ReportConfigurationWarning(context,
                    "Applying replacer '{0}' would result in the length of the expanded string exceeding MaxCapacity.",
                    nameof(Configuration.Replacer),
                    replacer);
            }
        }

        return builder.ToString();
    }

    #endregion

    #region Error Reporting

    private static void ReportSettingsWarning(SourceProductionContext context, string message,
        params object?[]? args)
    {
        context.ReportDiagnostic(Diagnostic.Create(
            new DiagnosticDescriptor(
                "EG0001",
                "Error in additional file 'Extengerator.settings.yaml'",
                message,
                "Problem",
                DiagnosticSeverity.Warning,
                true),
            Location.None,
            args));
    }

    private static void ReportConfigurationWarning(SourceProductionContext context, string message,
        params object?[]? args)
    {
        context.ReportDiagnostic(Diagnostic.Create(
            new DiagnosticDescriptor(
                "EG0002",
                "Failed processing of configuration.",
                message,
                "Problem",
                DiagnosticSeverity.Warning,
                true),
            Location.None,
            args));
    }

    #endregion
}