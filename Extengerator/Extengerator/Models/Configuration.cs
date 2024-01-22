using Common.Collections.Generic;
using Common.Helper;

namespace Extengerator.Models;

internal readonly record struct Configuration(
    string? InterfaceType,
    string? Template,
    string[]? Replacer,
    string? FileName)
{
    public bool IsValid()
    {
        return !InterfaceType.IsNullOrEmpty()
               && !Template.IsNullOrEmpty()
               && !Replacer.IsNullOrEmpty()
               && !FileName.IsNullOrEmpty();
    }

    public override string ToString()
    {
        var replacer = Replacer is null ? null : $"{{{string.Join(",", Replacer).ReplaceLineBreaks(" ")}}}";
        return
            $"Configuration {{ InterfaceType = {InterfaceType}, Template = {Template?.ReplaceLineBreaks(" ")}, Replacer = {replacer}, FileName = {FileName}}}";
    }
}