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
