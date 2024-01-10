using System.Text;
using Common.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Extengerator.Common.CodeAnalysis;

public static class SourceProductionContextExtensions
{
    public static void AddSourceNormalized(this SourceProductionContext context, string hintName, string? sourceText)
    {
        if(sourceText.IsNullOrEmpty())
            return;
        
        var theCode = CSharpSyntaxTree.ParseText(sourceText!).GetRoot().NormalizeWhitespace().ToFullString();
        context.AddSource(hintName, SourceText.From(theCode, Encoding.UTF8));
    }
}