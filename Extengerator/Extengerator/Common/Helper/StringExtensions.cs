// ReSharper disable once CheckNamespace
namespace Common.Helper;

public static class StringExtensions
{
    public static string ReplaceLineBreaks( this string oldValue, string newValue )
    {
        return oldValue.Replace( "\r\n", newValue )
            .Replace( "\r", newValue )
            .Replace( "\n", newValue );
    }
}