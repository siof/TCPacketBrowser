using System;
using System.Collections.Generic;

namespace siof.Common.Extensions
{
    public static class ArrayExtensions
    {
        public static bool ArraysEqual<T>(this T[] array1, T[] array2)
        {
            if (array1 == null && array2 == null)
                return true;

            if (array1 == null || array2 == null)
                return false;

            if (ReferenceEquals(array1, array2))
                return true;

            if (array1.Length != array2.Length)
                return false;

            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < array1.Length; i++)
            {
                if (!comparer.Equals(array1[i], array2[i]))
                    return false;
            }

            return true;
        }

        public static void Clear<T>(this T[] array)
        {
            if (array != null && array.Length > 0)
                Array.Clear(array, 0, array.Length);
        }
    }
}
