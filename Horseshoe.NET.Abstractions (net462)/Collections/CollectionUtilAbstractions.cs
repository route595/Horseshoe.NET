using System;
using System.Collections.Generic;
using System.Linq;

namespace Horseshoe.NET.Collections
{
    /// <summary>
    /// A set of utility methods for collections.
    /// </summary>
    public static class CollectionUtilAbstractions
    {
        /// <summary>
        /// Creates a new <c>List&lt;T&gt;</c> from any collection.
        /// </summary>
        /// <typeparam name="T">A collection type.</typeparam>
        /// <param name="collection">A collection to copy into a list.</param>
        /// <returns>A new <c>List&lt;T&gt;</c></returns>
        public static List<T> ToList<T>(IEnumerable<T> collection)
        {
            return collection == null
                ? new List<T>()
                : new List<T>(collection);
        }

        /// <summary>
        /// Casts a collection as <c>List&lt;T&gt;</c> if such a cast is available, otherwise creates 
        /// a new <c>List&lt;T&gt;</c> from the collection.  Returns <c>null</c> if <c>collection</c> is <c>null</c>.
        /// </summary>
        /// <typeparam name="T">A collection type.</typeparam>
        /// <param name="collection">A collection to cast or copy to a list.</param>
        /// <returns>A <c>List&lt;T&gt;</c> or <c>null</c> if <c>collection</c> is <c>null</c></returns>
        public static List<T> AsList<T>(IEnumerable<T> collection)
        {
            if (collection == null) 
                return null;
            return collection is List<T> list
                ? list
                : ToList(collection);
        }

        /// <summary>
        /// Creates a new array from any collection.
        /// </summary>
        /// <typeparam name="T">The array type.</typeparam>
        /// <param name="collection">A collection to copy into the array.</param>
        /// <returns>A new <c>T[]</c></returns>
        public static T[] ToArray<T>(IEnumerable<T> collection)
        {
            return (collection ?? Enumerable.Empty<T>()).ToArray();
        }

        /// <summary>
        /// Casts a collection as <c>T[]</c> if such a cast is available, otherwise creates 
        /// a new array from the collection.  Returns <c>null</c> if <c>collection</c> is <c>null</c>.
        /// </summary>
        /// <typeparam name="T">The array type.</typeparam>
        /// <param name="collection">A collection to cast or copy to the array.</param>
        /// <returns>A <c>T[]</c> or <c>null</c> if <c>collection</c> is <c>null</c></returns>
        public static T[] AsArray<T>(IEnumerable<T> collection)
        {
            if (collection == null) 
                return null;
            return collection is T[] array
                ? array
                : ToArray(collection);
        }

        /// <summary>
        /// Determines if an item is found in a collection.
        /// </summary>
        /// <typeparam name="T">Type of collection.</typeparam>
        /// <param name="collection">The collection to search.</param>
        /// <param name="item">The item to match.</param>
        /// <param name="comparer">Optional, an equality comparer.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool Contains<T>(IEnumerable<T> collection, T item, IEqualityComparer<T> comparer = null)
        {
            if (collection == null)
            {
                return false; 
            }
            return comparer != null
                ? collection.Contains(item, comparer)
                : collection.Contains(item);
        }
    }
}
