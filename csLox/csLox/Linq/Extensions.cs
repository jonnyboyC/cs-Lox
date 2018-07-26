using System.Linq;
using System.Collections.Generic;

namespace csLox.Linq
{
    public static class Extensions
    {
        public static bool None<T>(this IEnumerable<T> source)
        {
            return !source.Any();
        }
    }
}
