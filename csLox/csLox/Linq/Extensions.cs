using System;
using System.Collections.Generic;
using System.Text;

namespace csLox.Linq
{
    public static class Extensions
    {
        public static IEnumerable<T> Yield<T>(this T item)
        {
            yield return item;
        }
    }
}
