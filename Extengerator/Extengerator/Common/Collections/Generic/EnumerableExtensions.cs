using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Common.Collections.Generic;

public static class EnumerableExtensions
{
    public static string StringJoin(this IEnumerable<string> source, string separator)
    {
        return string.Join(separator, source.ToArray());
    }

    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> source)
        => source.SelectMany(s => s);

    public static bool IsEmpty<T>(this IEnumerable<T> collection)
        => !collection.Any();
        
    public static IEnumerable<T> OrEmptyIfNull<T>(this IEnumerable<T> source) => source ?? Enumerable.Empty<T>();

    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source)
    {
        return source is null || source.IsEmpty();
    }
    
    public static bool IsNullOrEmpty<T>(this T[]? source)
    {
        return source is null || source.Length == 0;
    }
    
    public static bool IsNullOrEmpty<T>(this IList<T>? source)
    {
        return source is null || source.Count == 0;
    }
    

    public static int GetSequenceEqualHashCode<T>(this IEnumerable<T>? source)
    {
        unchecked
        {
            if (source is null)
                return 0;
                
            return 397 * source.Aggregate(0, (acc, curr)
                => (acc * 397) ^ (curr?.GetHashCode() ?? 0));
        } 
    }

}