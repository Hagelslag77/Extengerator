namespace Extengerator;

internal readonly record struct Configuration(
    string InterfaceType,
    string Template,
    string[] Replacer,
    string FileName);