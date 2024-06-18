using System.Collections.Generic;

namespace Horseshoe.NET.Collections
{
    /// <summary>
    /// A utility for building a <c>List</c> or an array by combining collections and/or individual items
    /// </summary>
    /// <typeparam name="T">The collection type</typeparam>
    public class CollectionBuilder<T>
    {
        private readonly List<T> list;

        /// <summary>
        /// A list sorter
        /// </summary>
        public ListSorter<T> Sorter { get; }

        /// <summary>
        /// Creates a new <c>ConstructionBuilder</c>
        /// </summary>
        /// <param name="sorter">An optional sorter</param>
        public CollectionBuilder(ListSorter<T> sorter = null)
        {
            list = new List<T>();
            Sorter = sorter;
        }

        /// <summary>
        /// Creates a new <c>ConstructionBuilder</c> from an existing collection
        /// </summary>
        /// <param name="collection">A collection</param>
        /// <param name="sorter">An optional sorter</param>
        public CollectionBuilder(IEnumerable<T> collection, ListSorter<T> sorter = null) : this(sorter: sorter)
        {
            if (collection != null)
                list.AddRange(collection);
        }

        /// <summary>
        /// Adds one or more items to the final collection
        /// </summary>
        /// <param name="items">One or more items to append</param>
        /// <returns>The current collection builder</returns>
        public CollectionBuilder<T> Add(params T[] items)
        {
            if (items != null)
                list.AddRange(items);
            return this;
        }

        /// <summary>
        /// Appends one or more collections to the current collection
        /// </summary>
        /// <param name="collections">One or more collections to append</param>
        /// <returns>The current collection builder</returns>
        public CollectionBuilder<T> Append(params IEnumerable<T>[] collections)
        {
            if (collections != null)
            {
                foreach (var collection in collections)
                {
                    if (collection != null)
                        list.AddRange(collection);
                }
            }
            return this;
        }

        /// <summary>
        /// Returns the final collection as a <c>List</c>
        /// </summary>
        /// <returns>The final collection</returns>
        public List<T> ToList() 
        { 
            if (Sorter != null)
                return Sorter.Sort(list);
            return list; 
        }

        /// <summary>
        /// Returns the final collection as an array
        /// </summary>
        /// <returns>The final collection</returns>
        public T[] ToArray() 
        { 
            return ToList().ToArray();
        }
    }
}
