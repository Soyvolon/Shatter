using System.Globalization;

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
