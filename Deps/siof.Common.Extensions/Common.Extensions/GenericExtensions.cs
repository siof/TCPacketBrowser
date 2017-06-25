namespace siof.Common.Extensions
{
    public static class GenericExtensions
    {
        public static T ValueOrDefault<T>(this T val, T defaultValue)
        {
            return val.IfNotNull(value => value, defaultValue);
        }

        public static bool IsOneOf<T>(this T value, params T[] paramList)
        {
            if (value != null && paramList != null)
            {
                foreach (T elem in paramList)
                {
                    if (value.Equals(elem))
                        return true;
                }
            }

            return false;
        }

        public static bool IsNotAnyOf<T>(this T value, params T[] paramList)
        {
            if (value != null && paramList != null)
            {
                foreach (T elem in paramList)
                {
                    if (value.Equals(elem))
                        return false;
                }
            }

            return true;
        }
    }
}
