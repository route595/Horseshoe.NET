using System.Collections.Generic;
using System.Linq;

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
        /// If <c>true</c> reduces the result to only unique values, default is <c>false</c>
        /// </summary>
        public bool DistinctValues { get; }

        /// <summary>
        /// An optional equality comparer to use in conjunction with <c>DistinctValues</c>
        /// </summary>
        public IEqualityComparer<T> Comparer { get; }

        /// <summary>
        /// Creates a new <c>CollectionBuilder</c>
        /// </summary>
        /// <param name="sorter">An optional sorter</param>
        /// <param name="distinctValues">If <c>true</c> reduces the result to only unique values, default is <c>false</c></param>
        /// <param name="comparer">An optional equality comparer to use in conjunction with <c>distinctValues = true</c></param>
        public CollectionBuilder(ListSorter<T> sorter = null, bool distinctValues = false, IEqualityComparer<T> comparer = null) : this(null, sorter: sorter, distinctValues: distinctValues, comparer: comparer)
        {
        }

        /// <summary>
        /// Creates a new <c>CollectionBuilder</c> from an existing collection
        /// </summary>
        /// <param name="collection">A collection</param>
        /// <param name="sorter">An optional sorter</param>
        /// <param name="distinctValues">If <c>true</c> reduces the result to only unique values, default is <c>false</c></param>
        /// <param name="comparer">An optional equality comparer to use in conjunction with <c>distinctValues = true</c></param>
        public CollectionBuilder(IEnumerable<T> collection, ListSorter<T> sorter = null, bool distinctValues = false, IEqualityComparer<T> comparer = null)
        {
            list = CollectionUtil.ToList(collection);
            Sorter = sorter;
            DistinctValues = distinctValues;
            Comparer = comparer;
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
        /// <param name="sorter">An optional sorter</param>
        /// <param name="distinctValues">If <c>true</c> reduces the result to only unique values, default is <c>false</c></param>
        /// <param name="comparer">An optional equality comparer to use in conjunction with <c>distinctValues = true</c></param>
        /// <returns>The final collection</returns>
        public List<T> ToList(ListSorter<T> sorter = null, bool distinctValues = false, IEqualityComparer<T> comparer = null) 
        { 
            IEnumerable<T> collection = list;
            if ((sorter ?? Sorter) != null)
                collection = (sorter ?? Sorter)._Sort(collection);
            if (distinctValues || DistinctValues)
                collection = (comparer ?? Comparer) == null
                    ? collection.Distinct()
                    : collection.Distinct(comparer ?? Comparer);
            return CollectionUtil.AsList(collection); 
        }

        /// <summary>
        /// Returns the final collection as an array
        /// </summary>
        /// <param name="sorter">An optional sorter</param>
        /// <param name="distinctValues">If <c>true</c> reduces the result to only unique values, default is <c>false</c></param>
        /// <param name="comparer">An optional equality comparer to use in conjunction with <c>distinctValues = true</c></param>
        /// <returns>The final collection</returns>
        public T[] ToArray(ListSorter<T> sorter = null, bool distinctValues = false, IEqualityComparer<T> comparer = null) 
        {
            IEnumerable<T> collection = list;
            if ((sorter ?? Sorter) != null)
                collection = (sorter ?? Sorter)._Sort(collection);
            if (distinctValues || DistinctValues)
                collection = (comparer ?? Comparer) == null
                    ? collection.Distinct()
                    : collection.Distinct(comparer ?? Comparer);
            return collection.ToArray();
        }
    }

    /// <summary>
    /// Static methods for generating <c>CollectionBuilder</c> instances
    /// </summary>
    public static class CollectionBuilder
    {
        /// <summary>
        /// Creates a new <c>CollectionBuilder</c>
        /// </summary>
        /// <typeparam name="T">The collection type</typeparam>
        /// <param name="sorter">An optional sorter</param>
        /// <param name="distinctValues">If <c>true</c> reduces the result to only unique values, default is <c>false</c></param>
        /// <param name="comparer">An optional equality comparer to use in conjunction with <c>distinctValues = true</c></param>
        /// <returns>A new <c>CollectionBuilder</c> instance</returns>
        public static CollectionBuilder<T> Build<T>(ListSorter<T> sorter = null, bool distinctValues = false, IEqualityComparer<T> comparer = null)
            => new CollectionBuilder<T>(sorter: sorter, distinctValues: distinctValues, comparer: comparer);

        /// <summary>
        /// Creates a new <c>CollectionBuilder</c> from an existing collection
        /// </summary>
        /// <typeparam name="T">The collection type</typeparam>
        /// <param name="collection">A collection</param>
        /// <param name="sorter">An optional sorter</param>
        /// <param name="distinctValues">If <c>true</c> reduces the result to only unique values, default is <c>false</c></param>
        /// <param name="comparer">An optional equality comparer to use in conjunction with <c>distinctValues = true</c></param>
        /// <returns>A new <c>CollectionBuilder</c> instance</returns>
        public static CollectionBuilder<T> Build<T>(IEnumerable<T> collection, ListSorter<T> sorter = null, bool distinctValues = false, IEqualityComparer<T> comparer = null)
            => new CollectionBuilder<T>(collection, sorter: sorter, distinctValues: distinctValues, comparer: comparer);
    }
}
