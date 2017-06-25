using System;
using System.Text.RegularExpressions;

namespace siof.Common.Extensions
{
    public static class RegexExtensions
    {
        public static bool IsValidRegex(this string pattern)
        {
            if (pattern.IsEmptyOrWhiteSpace())
                return false;

            try
            {
                Regex.Match("", pattern);
            }
            catch (ArgumentException)
            {
                return false;
            }

            return true;
        }
    }
}
