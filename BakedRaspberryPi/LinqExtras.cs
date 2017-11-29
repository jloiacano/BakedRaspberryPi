using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BakedRaspberryPi
{
    public static class LinqExtras
    {
        // Thank you to Thomas Levesque for these functions out of his Linq.Extras library
        public static bool None<TSource>(this IEnumerable<TSource> source)
        {
            return !source.Any();
        }

        public static bool None<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return !source.Any(predicate);
        }

        // another Linq extra provided by Muhammad Hasan Khan
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> list, int parts)
        {
            int i = 0;
            var splits = from item in list
                         group item by i++ % parts into part
                         select part.AsEnumerable();
            return splits;
        }
    }
}