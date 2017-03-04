namespace WpfCommons.Extensions
{
    public static class StringExtensions
    {
        public static string ReplaceSafe(this string str, string from, string to)
        {
            return str.IfNotNull(st => st.Replace(from, to), null);
        }

        public static bool IsEmptyOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static bool IsNotEmptyOrWhiteSpace(this string str)
        {
            return !str.IsEmptyOrWhiteSpace();
        }
    }
}
