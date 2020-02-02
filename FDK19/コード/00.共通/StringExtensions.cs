using System.Linq;

namespace FDK.ExtensionMethods
{
    public static class StringExtensions
    {
        public static bool In(this string str, params string[] param)
        {
            return param.Contains(str);
        }

        public static string ToNullIfEmpty(this string value)
        {
            return string.IsNullOrEmpty(value) ? null : value;
        }
    }
}
