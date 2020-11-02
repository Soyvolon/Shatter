using System;
using System.Collections.Generic;

namespace Shatter.Core.Extensions
{
    public static class ListExtensions
    {
        private static readonly Random r = new Random();

        /// <summary>
        /// Returns a random value from a list of items.
        /// </summary>
        /// <typeparam name="T">Type of items</typeparam>
        /// <param name="list">List to pick a value from.</param>
        /// <returns>A Random Item or default if there are no items to return.</returns>
        public static T Random<T>(this List<T> list)
        {
            if (list is null || list.Count <= 0) return default;

            return list[r.Next(list.Count)];
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = r.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
