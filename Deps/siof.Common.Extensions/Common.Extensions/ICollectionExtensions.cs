using System;
using System.Collections.Generic;
using System.Linq;

namespace siof.Common.Extensions
{
    public static class ICollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> toAdd)
        {
            foreach (var element in toAdd)
                collection.Add(element);
        }

        public static void RemoveRange<T>(this ICollection<T> collection, IEnumerable<T> toRemove)
        {
            foreach (var element in toRemove)
                collection.Remove(element);
        }

        public static ICollection<T> RemoveIf<T>(this ICollection<T> collection, Func<T, bool> predicate)
        {
            return collection.IfNotNull(col =>
            {
                var toRemove = col.Where(predicate).ToList();

                if (toRemove.Count > 0)
                    col.RemoveRange(toRemove);

                return col;
            }, null);
        }
    }
}
