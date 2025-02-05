using System;
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
        internal readonly List<T> _list;

        /// <summary>
        /// A list sorter
        /// </summary>
        public ListSorter<T> Sorter { get; }

        /// <summary>
        /// An optional equality comparer to use in conjunction with <c>DistinctValues</c>
        /// </summary>
        public IEqualityComparer<T> Comparer { get; }

        /// <summary>
        /// If <c>true</c> reduces the result to only unique values, default is <c>false</c>
        /// </summary>
        public bool DistinctValues { get; }

        /// <summary>
        /// Creates a new <c>CollectionBuilder</c>
        /// </summary>
        /// <param name="sorter">An optional sorter</param>
        /// <param name="comparer">An optional equality comparer to use in conjunction with <c>distinctValues = true</c></param>
        /// <param name="distinctValues">If <c>true</c> reduces the result to only unique values, default is <c>false</c></param>
        public CollectionBuilder(ListSorter<T> sorter = null, IEqualityComparer<T> comparer = null, bool distinctValues = false)
        {
            _list = new List<T>();
            Sorter = sorter;
            Comparer = comparer;
            DistinctValues = distinctValues;
        }

        /// <summary>
        /// Appends zero or more items to the current builder
        /// </summary>
        /// <param name="items">Items to append</param>
        /// <returns>The current <c>CollectionBuilder</c></returns>
        public CollectionBuilder<T> Append(IEnumerable<T> items)
        {
            CollectionUtil.Append(_list, items);
            return this;
        }

        /// <summary>
        /// Conditionally appends zero or more items to the current builder
        /// </summary>
        /// <param name="condition">Whether or not to append the item(s)</param>
        /// <param name="items">Items to append</param>
        /// <returns>The current <c>CollectionBuilder</c></returns>
        public CollectionBuilder<T> AppendIf(bool condition, IEnumerable<T> items)
        {
            CollectionUtil.AppendIf(_list, condition, items);
            return this;
        }

        /// <summary>
        /// Appends zero or more items to the current builder
        /// </summary>
        /// <param name="items">Items to append</param>
        /// <returns>The current <c>CollectionBuilder</c></returns>
        public CollectionBuilder<T> Append(params T[] items)
        {
            CollectionUtil.Append(_list, items);
            return this;
        }

        /// <summary>
        /// Conditionally appends zero or more items to the current builder
        /// </summary>
        /// <param name="condition">Whether or not to append the item(s)</param>
        /// <param name="items">Items to append</param>
        /// <returns>The current <c>CollectionBuilder</c></returns>
        public CollectionBuilder<T> AppendIf(bool condition, params T[] items)
        {
            CollectionUtil.AppendIf(_list, condition, items);
            return this;
        }

        /// <summary>
        /// Appends zero or more collections to the current builder
        /// </summary>
        /// <param name="collections">Collections to append</param>
        /// <returns>The current <c>CollectionBuilder</c></returns>
        public CollectionBuilder<T> Append(IEnumerable<IEnumerable<T>> collections)
        {
            CollectionUtil.Append(_list, collections);
            return this;
        }

        /// <summary>
        /// Conditionally appends zero or more collections to the current builder
        /// </summary>
        /// <param name="condition">Whether or not to append the collection(s)</param>
        /// <param name="collections">Collections to append</param>
        /// <returns>The current <c>CollectionBuilder</c></returns>
        public CollectionBuilder<T> AppendIf(bool condition, IEnumerable<IEnumerable<T>> collections)
        {
            CollectionUtil.AppendIf(_list, condition, collections);
            return this;
        }

        /// <summary>
        /// Appends zero or more collections to the current builder
        /// </summary>
        /// <param name="collections">Collections to append</param>
        /// <returns>The current <c>CollectionBuilder</c></returns>
        public CollectionBuilder<T> Append(params IEnumerable<T>[] collections)
        {
            CollectionUtil.Append(_list, collections);
            return this;
        }

        /// <summary>
        /// Conditionally appends zero or more collections to the current builder
        /// </summary>
        /// <param name="condition">Whether or not to append the collection(s)</param>
        /// <param name="collections">Collections to append</param>
        /// <returns>The current <c>CollectionBuilder</c></returns>
        public CollectionBuilder<T> AppendIf(bool condition, params IEnumerable<T>[] collections)
        {
            CollectionUtil.AppendIf(_list, condition, collections);
            return this;
        }

        /// <summary>
        /// Inserts zero or more items into the current builder
        /// </summary>
        /// <param name="index">The position in the collection to insert the item(s)</param>
        /// <param name="items">Items to insert</param>
        /// <returns>The current <c>CollectionBuilder</c></returns>
        public CollectionBuilder<T> Insert(int index, IEnumerable<T> items)
        {
            CollectionUtil.Insert(_list, index, items);
            return this;
        }

        /// <summary>
        /// Conditionally inserts zero or more items into the current builder
        /// </summary>
        /// <param name="index">The position in the collection to insert the item(s)</param>
        /// <param name="condition">Whether or not to insert the item(s)</param>
        /// <param name="items">Items to insert</param>
        /// <returns>The current <c>CollectionBuilder</c></returns>
        public CollectionBuilder<T> InsertIf(bool condition, int index, IEnumerable<T> items)
        {
            CollectionUtil.InsertIf(_list, condition, index, items);
            return this;
        }

        /// <summary>
        /// Inserts zero or more items into the current builder
        /// </summary>
        /// <param name="index">The position in the collection to insert the item(s)</param>
        /// <param name="items">Items to insert</param>
        /// <returns>The current <c>CollectionBuilder</c></returns>
        public CollectionBuilder<T> Insert(int index, params T[] items)
        {
            CollectionUtil.Insert(_list, index, items);
            return this;
        }

        /// <summary>
        /// Conditionally inserts zero or more items into the current builder
        /// </summary>
        /// <param name="condition">Whether or not to insert the item(s)</param>
        /// <param name="index">The position in the collection to insert the item(s)</param>
        /// <param name="items">Items to insert</param>
        /// <returns>The current <c>CollectionBuilder</c></returns>
        public CollectionBuilder<T> InsertIf(bool condition, int index, params T[] items)
        {
            CollectionUtil.InsertIf(_list, condition, index, items);
            return this;
        }

        /// <summary>
        /// Inserts zero or more collections to the current builder
        /// </summary>
        /// <param name="index">The position in the collection to insert the collections(s)</param>
        /// <param name="collections">Collections to insert</param>
        /// <returns>The current <c>CollectionBuilder</c></returns>
        public CollectionBuilder<T> Insert(int index, IEnumerable<IEnumerable<T>> collections)
        {
            CollectionUtil.Insert(_list, index, collections);
            return this;
        }

        /// <summary>
        /// Conditionally inserts zero or more collections to the current builder
        /// </summary>
        /// <param name="condition">Whether or not to insert the collection(s)</param>
        /// <param name="index">The position in the collection to insert the collections(s)</param>
        /// <param name="collections">Collections to insert</param>
        /// <returns>The current <c>CollectionBuilder</c></returns>
        public CollectionBuilder<T> InsertIf(bool condition, int index, IEnumerable<IEnumerable<T>> collections)
        {
            CollectionUtil.InsertIf(_list, condition, index, collections);
            return this;
        }

        /// <summary>
        /// Inserts zero or more collections into the current builder
        /// </summary>
        /// <param name="index">The position in the collection to insert the collections(s)</param>
        /// <param name="collections">Collections to insert</param>
        /// <returns>The current <c>CollectionBuilder</c></returns>
        public CollectionBuilder<T> Insert(int index, params IEnumerable<T>[] collections)
        {
            CollectionUtil.Insert(_list, index, collections);
            return this;
        }

        /// <summary>
        /// Conditionally inserts zero or more collections into the current builder
        /// </summary>
        /// <param name="condition">Whether or not to insert the collection(s)</param>
        /// <param name="index">The position in the collection to insert the collections(s)</param>
        /// <param name="collections">Collections to insert</param>
        /// <returns>The current <c>CollectionBuilder</c></returns>
        public CollectionBuilder<T> InsertIf(bool condition, int index, params IEnumerable<T>[] collections)
        {
            CollectionUtil.InsertIf(_list, condition, index, collections);
            return this;
        }

        /// <summary>
        /// Prepends zero or more items to the current builder
        /// </summary>
        /// <param name="items">Items to prepend</param>
        /// <returns>The current <c>CollectionBuilder</c></returns>
        public CollectionBuilder<T> Prepend(IEnumerable<T> items)
        {
            CollectionUtil.Prepend(_list, items);
            return this;
        }

        /// <summary>
        /// Conditionally prepends zero or more items to the current builder
        /// </summary>
        /// <param name="condition">Whether or not to prepend the item(s)</param>
        /// <param name="items">Items to prepend</param>
        /// <returns>The current <c>CollectionBuilder</c></returns>
        public CollectionBuilder<T> PrependIf(bool condition, IEnumerable<T> items)
        {
            CollectionUtil.PrependIf(_list, condition, items);
            return this;
        }

        /// <summary>
        /// Prepends zero or more items to the current builder
        /// </summary>
        /// <param name="items">Items to prepend</param>
        /// <returns>The current <c>CollectionBuilder</c></returns>
        public CollectionBuilder<T> Prepend(params T[] items)
        {
            CollectionUtil.Prepend(_list, items);
            return this;
        }

        /// <summary>
        /// Conditionally prepends zero or more items to the current builder
        /// </summary>
        /// <param name="condition">Whether or not to prepend the item(s)</param>
        /// <param name="items">Items to prepend</param>
        /// <returns>The current <c>CollectionBuilder</c></returns>
        public CollectionBuilder<T> PrependIf(bool condition, params T[] items)
        {
            CollectionUtil.PrependIf(_list, condition, items);
            return this;
        }

        /// <summary>
        /// Prepends zero or more collections to the current builder
        /// </summary>
        /// <param name="collections">Collections to prepend</param>
        /// <returns>The current <c>CollectionBuilder</c></returns>
        public CollectionBuilder<T> Prepend(IEnumerable<IEnumerable<T>> collections)
        {
            CollectionUtil.Prepend(_list, collections);
            return this;
        }

        /// <summary>
        /// Conditionally prepends zero or more collections to the current builder
        /// </summary>
        /// <param name="condition">Whether or not to prepend the collection(s)</param>
        /// <param name="collections">Collections to prepend</param>
        /// <returns>The current <c>CollectionBuilder</c></returns>
        public CollectionBuilder<T> PrependIf(bool condition, IEnumerable<IEnumerable<T>> collections)
        {
            CollectionUtil.PrependIf(_list, condition, collections);
            return this;
        }

        /// <summary>
        /// Prepends zero or more collections to the current builder
        /// </summary>
        /// <param name="collections">Collections to prepend</param>
        /// <returns>The current <c>CollectionBuilder</c></returns>
        public CollectionBuilder<T> Prepend(params IEnumerable<T>[] collections)
        {
            CollectionUtil.Prepend(_list, collections);
            return this;
        }

        /// <summary>
        /// Conditionally prepends zero or more collections to the current builder
        /// </summary>
        /// <param name="condition">Whether or not to prepend the collection(s)</param>
        /// <param name="collections">Collections to prepend</param>
        /// <returns>The current <c>CollectionBuilder</c></returns>
        public CollectionBuilder<T> PrependIf(bool condition, params IEnumerable<T>[] collections)
        {
            CollectionUtil.PrependIf(_list, condition, collections);
            return this;
        }

        /// <summary>
        /// Returns the final collection as a <c>List&lt;T&gt;</c>
        /// </summary>
        /// <returns>The final collection</returns>
        public List<T> RenderToList() 
        {
            IEnumerable<T> collection = CollectionUtil.ToList(_list);
            if (Sorter != null)
                collection = Sorter.Sort(collection);
            if (DistinctValues)
                collection = Comparer != null
                    ? collection.Distinct(Comparer)
                    : collection.Distinct();
            return collection.ToList(); 
        }

        /// <summary>
        /// Returns the final collection as an array
        /// </summary>
        /// <returns>The final collection</returns>
        public T[] RenderToArray() 
        {
            IEnumerable<T> collection = CollectionUtil.ToList(_list);
            if (Sorter != null)
                collection = Sorter.Sort(collection);
            if (DistinctValues)
                collection = Comparer != null
                    ? collection.Distinct(Comparer)
                    : collection.Distinct();
            return collection.ToArray();
        }

        /// <summary>
        /// Searches the final collection for at least one matching item.
        /// Returns <c>false</c> if <c>items == null</c> or is empty.
        /// </summary>
        /// <param name="items">Items to search for</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public bool ContainsAny(IEnumerable<T> items)
        {
            if (items == null || !items.Any())
                return false;
            var list = DistinctValues
                ? _list.Distinct().ToList()
                : _list;
            return Comparer != null
                ? items.Any(i => list.Contains(i, Comparer))
                : items.Any(i => list.Contains(i));
        }

        /// <summary>
        /// Searches the final collection for at least one matching item.
        /// Returns <c>false</c> if <c>items == null</c> or is empty.
        /// </summary>
        /// <param name="items">Items to search for</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public bool ContainsAny(params T[] items)
        {
            return ContainsAny(items as IEnumerable<T>);
        }

        /// <summary>
        /// Searches the final collection for all matching items. 
        /// Returns <c>false</c> if <c>items == null</c> or is empty.
        /// </summary>
        /// <param name="items">Items to search for</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public bool ContainsAll(IEnumerable<T> items)
        {
            if (items == null || !items.Any())
                return false;
            var list = DistinctValues
                ? _list.Distinct().ToList()
                : _list;
            return Comparer != null
                ? items.All(i => list.Contains(i, Comparer))
                : items.All(i => list.Contains(i));
        }

        /// <summary>
        /// Searches the final collection for all matching items. 
        /// Returns <c>false</c> if <c>items == null</c> or is empty.
        /// </summary>
        /// <param name="items">Items to search for</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public bool ContainsAll(params T[] items)
        {
            return ContainsAll(items as IEnumerable<T>);
        }
    }
}
