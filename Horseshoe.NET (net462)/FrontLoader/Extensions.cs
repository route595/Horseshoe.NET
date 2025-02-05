using System;
using System.Collections.Generic;
using System.Linq;

namespace Horseshoe.NET.FrontLoader
{
    /// <summary>
    /// Front loading extensions methods
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Creates a <c>ValueFrontLoader</c> to front load settings for future <c>object</c> comparison
        /// </summary>
        /// <typeparam name="T">A runtime <c>object</c> type</typeparam>
        /// <param name="obj">A value</param>
        /// <param name="comparer">An optional <c>IEqualityComparer&lt;T&gt;</c></param>
        /// <returns>The current <c>ObjectFrontLoader</c></returns>
        public static ValueFrontLoader<T> Load<T>(this T obj, IEqualityComparer<T> comparer = null)
        {
            return new ValueFrontLoader<T> { Value = obj, Comparer = comparer };
        }

        /// <summary>
        /// Inspired by SQL, checks to see if an item equal to the front loaded value is found in the argument item collection
        /// </summary>
        /// <typeparam name="T">A runtime <c>object</c> type</typeparam>
        /// <param name="frontLoader">The current <c>ObjectFrontLoader</c></param>
        /// <param name="items">A collection of items to search</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool In<T>(this ValueFrontLoader<T> frontLoader, params T[] items)
        {
            return In(frontLoader, items as IEnumerable<T>);
        }

        /// <summary>
        /// Inspired by SQL, checks to see if an item equal to the front loaded value is found in the argument item collection
        /// </summary>
        /// <typeparam name="T">A runtime <c>object</c> type</typeparam>
        /// <param name="frontLoader">The current <c>ObjectFrontLoader</c></param>
        /// <param name="items">A collection of items to search</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool In<T>(this ValueFrontLoader<T> frontLoader, IEnumerable<T> items)
        {
            if (items == null || !items.Any())
                return false;
            return items.Contains(frontLoader.Value, frontLoader.Comparer);
        }

        ///// <summary>
        ///// Creates a <c>StringFrontLoader</c> to front load settings for future <c>string</c> comparison
        ///// </summary>
        ///// <param name="str">A <c>string</c> value</param>
        ///// <param name="ignoreCase">If <c>true</c>, strings are compared without case-sensitivity</param>
        ///// <returns>The current <c>ObjectFrontLoader</c></returns>
        //public static StringFrontLoader Load(this string str, bool ignoreCase = false)
        //{
        //    return new StringFrontLoader { Value = str, IgnoreCase = ignoreCase };
        //}

        /// <summary>
        /// Inspired by SQL, checks to see if a <c>string</c> equal to the front loaded value is found in the argument item collection
        /// </summary>
        /// <param name="frontLoader">The current <c>StringFrontLoader</c></param>
        /// <param name="items">A collection of items to search</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool InIgnoreCase(this ValueFrontLoader<string> frontLoader, params string[] items)
        {
            return InIgnoreCase(frontLoader, items as IEnumerable<string>);
        }

        /// <summary>
        /// Inspired by SQL, checks to see if a <c>string</c> equal to the front loaded value is found in the argument item collection
        /// </summary>
        /// <param name="frontLoader">The current <c>StringFrontLoader</c></param>
        /// <param name="items">A collection of items to search</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool InIgnoreCase(this ValueFrontLoader<string> frontLoader, IEnumerable<string> items)
        {
            if (items == null)
                return false;
            return items.Any(s => string.Equals(s, frontLoader.Value, StringComparison.OrdinalIgnoreCase));
        }
    }
}
