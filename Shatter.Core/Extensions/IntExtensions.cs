namespace Shatter.Core.Extensions
{
	public static class IntExtensions
    {
        public static string ToMoney(this int val)
        {
            return val.ToString("N0") + " Gems :small_orange_diamond:";
        }
    }
}
