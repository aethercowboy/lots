using System;
using System.Collections.Generic;
using System.Linq;

namespace lots.BusinessLogic.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> Bracketify<T>(this IEnumerable<T> source, int size = 2)
        {
            if (size <= 0)
            {
                throw new ArgumentException("You cannot have a group size less than 1");
            }

            int i = 0;
            var query = from s in source
                        let num = i++
                        group s by num / size into g
                        select g;

            return query;

            //using (var e = source.GetEnumerator())
            //{
            //    while (e.MoveNext())
            //    {
            //        yield return e.Bracketify(size);
            //    }
            //}
        }

        private static IEnumerable<T> Bracketify<T>(this IEnumerator<T> enumerator, int size)
        {
            do
            {
                yield return enumerator.Current;
                size--;
            } while (size > 0 && enumerator.MoveNext());
        }
    }
}
