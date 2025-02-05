using System;
using System.Collections.Generic;
using System.Linq;

namespace Horseshoe.NET.Collections
{
    /// <summary>
    /// A collection of <c>Collection</c> extension methods
    /// </summary>
    public static class CollectionBuilderExtensions
    {
        /// <summary>
        /// Searches the final <c>string</c> collection for at least one matching item (not case-sensitive).
        /// Returns <c>false</c> if <c>items == null</c> or is empty.
        /// </summary>
        /// <param name="builder">A collection builder</param>
        /// <param name="items">Items to search for</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAnyIgnoreCase(this CollectionBuilder<string> builder, IEnumerable<string> items)
        {
            if (builder == null || items == null || !items.Any())
                return false;
            return items.Any(i => builder._list.Any(l => string.Equals(i, l, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Searches the final <c>string</c> collection for all matching items (not case-sensitive).
        /// Returns <c>false</c> if <c>items == null</c> or is empty.
        /// </summary>
        /// <param name="builder">A collection builder</param>
        /// <param name="items">Items to search for</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAllIgnoreCase(this CollectionBuilder<string> builder, IEnumerable<string> items)
        {
            if (builder == null || items == null || !items.Any())
                return false;
            return items.All(i => builder._list.Any(l => string.Equals(i, l, StringComparison.OrdinalIgnoreCase)));
        }
    }
}
