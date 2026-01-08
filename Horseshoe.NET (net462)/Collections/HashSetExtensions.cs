using System.Collections.Generic;

namespace Horseshoe.NET.Collections
{
    /// <summary>
    /// A collection of <c>List</c> extension methods
    /// </summary>
    public static class HashSetExtensions
    {
        /// <summary>
        /// Adds a range of items to a <c>HashSet</c>
        /// </summary>
        /// <typeparam name="T">Type of collection</typeparam>
        /// <param name="hashSet">The <see cref="HashSet{T}"/> to which the item will be added.</param>
        /// <param name="items">The items to add to the <see cref="HashSet{T}"/>.</param>
        public static void AddRange<T>(this HashSet<T> hashSet, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                hashSet.Add(item);
            }
        }

        /// <summary>
        /// Adds the specified item to the <see cref="HashSet{T}"/> if it is not already present.
        /// </summary>
        /// <remarks>This method checks for the presence of the item in the <see cref="HashSet{T}"/>
        /// before adding it, ensuring that the collection remains unique.</remarks>
        /// <typeparam name="T">Type of collection</typeparam>
        /// <param name="hashSet">The <see cref="HashSet{T}"/> to which the item will be added.</param>
        /// <param name="item">The item to add to the <see cref="HashSet{T}"/>.</param>
        public static void AddIfUnique<T>(this HashSet<T> hashSet, T item)
        {
            if (!hashSet.Contains(item))
            {
                hashSet.Add(item);
            }
        }
    }
}
