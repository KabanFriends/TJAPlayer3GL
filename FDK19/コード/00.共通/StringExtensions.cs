using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FDK.ExtensionMethods
{
    public static class StringExtensions
    {
        public static bool In(this string str, params string[] param)
        {
            return param.Contains(str);
        }
    }
}
