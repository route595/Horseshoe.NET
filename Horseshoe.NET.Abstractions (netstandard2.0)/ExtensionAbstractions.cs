using System.Collections.Generic;
using System.Linq;

using Horseshoe.NET.Collections;
using Horseshoe.NET.Text;

namespace Horseshoe.NET
{
    /// <summary>
    /// Extension methods for Horseshoe.NET and consumers
    /// </summary>
    public static class ExtensionAbstractions
    {
        /// <summary>
        /// Inspired by SQL, determines if an item is one of a supplied array of values
        /// </summary>
        /// <typeparam name="T">Type of collection</typeparam>
        /// <param name="obj">The item to locate</param>
        /// <param name="collection">The collection to search</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool In<T>(this T obj, params T[] collection)
        {
            return In(obj, collection as IEnumerable<T>);
        }

        /// <summary>
        /// Inspired by SQL, determines if an item is one of a supplied array of values
        /// </summary>
        /// <typeparam name="T">Type of collection</typeparam>
        /// <param name="obj">The item to locate</param>
        /// <param name="comparer">Optional, an equality comparer</param>
        /// <param name="collection">The collection to search</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool In<T>(this T obj, IEqualityComparer<T> comparer, params T[] collection)
        {
            return In(obj, collection, comparer: comparer);
        }

        /// <inheritdoc cref="CollectionUtilAbstractions.Contains{T}(IEnumerable{T}, T, IEqualityComparer{T})"/>
        public static bool In<T>(this T obj, IEnumerable<T> collection, IEqualityComparer<T> comparer = null)
        {
            return CollectionUtilAbstractions.Contains(collection, obj, comparer);
        }

        /// <inheritdoc cref="TextUtilAbstractions.In(string, bool, string[])"/>
        public static bool InIgnoreCase(this string text, params string[] collection)
        {
            return TextUtilAbstractions.In(text, true, collection);
        }

        /// <summary>
        /// Inspired by SQL, determines if a string is found in a collection of strings (not case-sensitive).
        /// </summary>
        /// <param name="text">The <c>string</c> to search match.</param>
        /// <param name="collection">A <c>string</c> collection to search.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool InIgnoreCase(this string text, IEnumerable<string> collection)
        {
            return InIgnoreCase(text, collection is string[] array ? array : collection.ToArray());
        }
    }
}
