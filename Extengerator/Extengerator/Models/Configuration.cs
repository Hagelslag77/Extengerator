using Common.Collections.Generic;

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
}