using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Extengerator.Tests.Utils;

public class TestAdditionalFile(string path, string? text) : AdditionalText
{
    private readonly SourceText? _text = text is null ? null : SourceText.From(text);
    
    public override string Path { get; } = path;

    public override SourceText? GetText(CancellationToken cancellationToken = new()) => _text;
}