using System;
using System.Collections.Generic;
using System.Text;

namespace NitroSharp.Extensions
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
    }
}
