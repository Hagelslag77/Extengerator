using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Common.Collections.Generic;

public static class EnumerableExtensions
{
    private static bool IsEmpty<T>(this IEnumerable<T> collection)
        => !collection.Any();
    
    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source)
    {
        return source is null || source.IsEmpty();
    }
    
    public static bool IsNullOrEmpty<T>(this T[]? source)
    {
        return source is null || source.Length == 0;
    }
}