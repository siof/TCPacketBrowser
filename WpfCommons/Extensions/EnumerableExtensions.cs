using System;
using System.Collections.Generic;
using System.Text;

namespace WpfCommons.Extensions
{
    public static class EnumerableExtensions
    {
        public static string ToString<T>(this IEnumerable<T> collection, char separator)
        {
            StringBuilder builder = new StringBuilder();

            foreach (var element in collection)
                builder.Append(element.ToString() + separator);

            return builder.ToString();
        }

        public static string ToString<T>(this IEnumerable<T> collection, string separator)
        {
            StringBuilder builder = new StringBuilder();

            foreach (var element in collection)
                builder.Append(element.ToString() + separator);

            return builder.ToString();
        }

        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var element in collection)
                action.Invoke(element);
        }
    }
}
