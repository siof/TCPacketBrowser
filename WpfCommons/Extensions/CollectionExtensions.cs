using System.Collections.Generic;

namespace WpfCommons.Extensions
{
    public static class CollectionExtensions
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
    }
}
