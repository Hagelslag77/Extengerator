namespace Extengerator.Common.Helper;

public static class StringExtensions
{
    public static string RemoveLineBreaks( this string value )
    {
        return value.Replace( "\r", "").Replace( "\n", "" );
    }

    public static string ReplaceLineBreaks( this string oldValue, string newValue )
    {
        return oldValue.Replace( "\r\n", newValue )
            .Replace( "\r", newValue )
            .Replace( "\n", newValue );
    }
}