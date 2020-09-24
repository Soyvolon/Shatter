using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace NitroSharp.Extensions
{
    public static class IntExtensions
    {
        public static string ToMoney(this int val, string culture)
        {
            return val.ToString("C", new CultureInfo(culture));
        }
    }
}
