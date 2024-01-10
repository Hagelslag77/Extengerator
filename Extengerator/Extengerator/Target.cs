using System.Collections.Generic;
using System.Linq;

namespace Extengerator;

internal readonly record struct Target(string Class, IEnumerable<string> Interfaces, bool IsValid = true)
{
    internal static readonly Target DefaultError = new("", Enumerable.Empty<string>(), false);

}

