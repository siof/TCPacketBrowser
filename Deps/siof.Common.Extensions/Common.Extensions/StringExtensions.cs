using System.Linq;

namespace siof.Common.Extensions
{
    public static class StringExtensions
    {
        public static string ReplaceSafe(this string str, char from, char to)
        {
            return str.IfNotNull(st => st.Replace(from, to), null);
        }

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

        public static string TrimSafe(this string str, params char[] trimChars)
        {
            return str.IfNotNull(strVal => { return strVal.Trim(trimChars); }, null);
        }

        public static string TrimStartSafe(this string str, params char[] trimChars)
        {
            return str.IfNotNull(strVal => { return strVal.TrimStart(trimChars); }, null);
        }

        public static string TrimEndSafe(this string str, params char[] trimChars)
        {
            return str.IfNotNull(strVal => { return strVal.TrimEnd(trimChars); }, null);
        }

        public static bool IsAlfaNumeric(this string str)
        {
            return str.IfNotNull(strVal => strVal.All(char.IsLetterOrDigit), false);
        }
    }
}
