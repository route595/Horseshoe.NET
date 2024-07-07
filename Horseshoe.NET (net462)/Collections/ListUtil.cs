using System;
using System.Collections.Generic;

namespace Horseshoe.NET.Collections
{
    /// <summary>
    /// A collection of <c>List</c> utility methods
    /// </summary>
    public static class ListUtil
    {
        private static List<T> AsList<T>(IEnumerable<T> collection) =>
            CollectionUtilAbstractions.AsList(collection);

        private static List<T> ToList<T>(IEnumerable<T> collection) =>
            CollectionUtilAbstractions.ToList(collection);

        /// <summary>
        /// Inflates a list to the desired target size by padding items at the indicated boundary
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="list">A list</param>
        /// <param name="targetSize">The desired size of the list after padding</param>
        /// <param name="boundary">End (default) or Start</param>
        /// <param name="padWith">Item to use for padding</param>
        /// <param name="cannotExceedTargetSize"><c>true</c> if an exception should be thrown for oversized lists</param>
        /// <param name="keepOriginalListDataSource"><c>true</c> will prevent internally creating a new <c>List&lt;T&gt;</c> if <c>collection</c> is already a <c>List&lt;T&gt;</c> instance - this is to improve performance</param>
        /// <returns>The resized list</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="ValidationException"></exception>
        public static List<T> Pad<T>(List<T> list, int targetSize, CollectionBoundary boundary = default, T padWith = default, bool cannotExceedTargetSize = false, bool keepOriginalListDataSource = false)
        {
            var collection = CollectionUtil.Pad<T>
            (
                list,
                targetSize,
                boundary: boundary,
                padWith: padWith,
                cannotExceedTargetSize: cannotExceedTargetSize,
                keepOriginalListDataSource: keepOriginalListDataSource
            );
            return AsList(collection);
        }

        /// <summary>
        /// Shrinks a list to the desired target size by removing items at the indicated boundary
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="list">A list</param>
        /// <param name="targetSize">The desired size of the list after padding</param>
        /// <param name="boundary">End (default) or Start</param>
        /// <param name="keepOriginalListDataSource"><c>true</c> will prevent internally creating a new <c>List&lt;T&gt;</c> if <c>collection</c> is already a <c>List&lt;T&gt;</c> instance - this is to improve performance</param>
        /// <returns>The resized list</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="ValidationException"></exception>
        public static List<T> Crop<T>(List<T> list, int targetSize, CollectionBoundary boundary = default, bool keepOriginalListDataSource = false)
        {
            var collection = CollectionUtil.Crop<T>
            (
                list,
                targetSize,
                boundary: boundary,
                keepOriginalListDataSource: keepOriginalListDataSource
            );
            return AsList(collection);
        }

        /// <summary>
        /// Inflates or shrinks a list to the desired target size by padding or removing items at the indicated boundary
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="list">A list</param>
        /// <param name="targetSize">The desired size of the list after padding</param>
        /// <param name="cropBoundary">End (default) or Start</param>
        /// <param name="padBoundary">End (default) or Start</param>
        /// <param name="padWith">Item to use for padding</param>
        /// <param name="keepOriginalListDataSource"><c>true</c> will prevent internally creating a new <c>List&lt;T&gt;</c> if <c>collection</c> is already a <c>List&lt;T&gt;</c> instance - this is to improve performance</param>
        /// <returns>The resized list</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="ValidationException"></exception>
        public static List<T> Fit<T>(List<T> list, int targetSize, CollectionBoundary cropBoundary = default, CollectionBoundary padBoundary = default, T padWith = default, bool keepOriginalListDataSource = false)
        {
            var collection = CollectionUtil.Fit<T>
            (
                list,
                targetSize,
                cropBoundary: cropBoundary,
                padBoundary: padBoundary,
                padWith: padWith,
                keepOriginalListDataSource: keepOriginalListDataSource
            );
            return AsList(collection);
        }

        /// <summary>
        /// Combines zero or more lists
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="collections">Collections to combine</param>
        /// <returns>The combined list</returns>
        public static List<T> Combine<T>(params IEnumerable<T>[] collections)
        {
            return new CollectionBuilder<T>()
                .Append(collections)
                .ToList();
        }

        /// <summary>
        /// Combines zero or more lists into a single list of distinct values
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="collections">Collections to combine</param>
        /// <returns>The combined list</returns>
        public static List<T> CombineDistinct<T>(params IEnumerable<T>[] collections)
        {
            return new CollectionBuilder<T>(distinctValues: true)
                .Append(collections)
                .ToList();
        }

        /// <summary>
        /// Combines zero or more lists into a single list of distinct values
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="comparer">The compararer used to determine distinctness</param>
        /// <param name="collections">Collections to combine</param>
        /// <returns>The combined list</returns>
        public static List<T> CombineDistinct<T>(IEqualityComparer<T> comparer, params IEnumerable<T>[] collections)
        {
            return new CollectionBuilder<T>(distinctValues: true, comparer: comparer)
                .Append(collections)
                .ToList();
        }

        /// <summary>
        /// Appends zero or more items to a collection, result is a new <c>List</c>
        /// </summary>
        /// <typeparam name="T">Type of collection</typeparam>
        /// <param name="collection">A collection to which <c>items</c> will be appended</param>
        /// <param name="items">Items to append</param>
        /// <returns>A new <c>List</c></returns>
        public static List<T> Append<T>(IEnumerable<T> collection, params T[] items)
        {
            var list = ToList(collection);
            if (items != null)
            {
                list.AddRange(items);
            }
            return list;
        }

        /// <summary>
        /// Appends zero or more items to a <c>List</c>
        /// </summary>
        /// <typeparam name="T">Type of <c>List</c></typeparam>
        /// <param name="list">A <c>List</c> to which <c>items</c> will be appended</param>
        /// <param name="items">Items to append</param>
        /// <returns>The orig <c>List</c></returns>
        public static List<T> Append<T>(List<T> list, params T[] items)
        {
            list = list ?? new List<T>();
            if (items != null)
            {
                list.AddRange(items);
            }
            return list;
        }

        /// <summary>
        /// Conditionally appends zero or more items to a collection, result is a new <c>List</c>
        /// </summary>
        /// <typeparam name="T">Type of collection</typeparam>
        /// <param name="condition"><c>true</c> or <c>false</c></param>
        /// <param name="collection">A collection to which <c>items</c> will conditionally be appended</param>
        /// <param name="items">Items to append</param>
        /// <returns>A new <c>List</c></returns>
        public static List<T> AppendIf<T>(bool condition, IEnumerable<T> collection, params T[] items)
        {
            var list = ToList(collection);
            if (condition && items != null)
            {
                list.AddRange(items);
            }
            return list;
        }

        /// <summary>
        /// Conditionally appends zero or more items to a <c>List</c>
        /// </summary>
        /// <typeparam name="T">Type of list</typeparam>
        /// <param name="condition"><c>true</c> or <c>false</c></param>
        /// <param name="list">A <c>List</c> to which <c>items</c> will be appended</param>
        /// <param name="items">Items to append</param>
        /// <returns>The orig <c>List</c></returns>
        public static List<T> AppendIf<T>(bool condition, List<T> list, params T[] items)
        {
            list = list ?? new List<T>();
            if (condition && items != null)
            {
                list.AddRange(items);
            }
            return list;
        }

        /// <summary>
        /// Conditionally appends zero or more items to a collection, result is a new <c>List</c>
        /// </summary>
        /// <typeparam name="T">Type of collection</typeparam>
        /// <param name="condition">A function that dictates whether an item gets appended</param>
        /// <param name="collection">A collection to which <c>items</c> will be conditionally appended</param>
        /// <param name="items">Items to conditionally append</param>
        /// <returns>A new <c>List</c></returns>
        public static List<T> AppendIf<T>(Func<T, bool> condition, IEnumerable<T> collection, params T[] items)
        {
            var list = ToList(collection);
            if (items != null)
            {
                foreach (var item in items)
                {
                    if (condition == null || condition.Invoke(item))
                        list.Add(item);
                }
            }
            return list;
        }

        /// <summary>
        /// Conditionally appends zero or more items to a <c>List</c>
        /// </summary>
        /// <typeparam name="T">Type of list</typeparam>
        /// <param name="condition">A function that dictates whether an item gets appended</param>
        /// <param name="list">A <c>List</c> to which <c>items</c> will be conditionally appended</param>
        /// <param name="items">Items to conditionally append</param>
        /// <returns>The orig <c>List</c></returns>
        public static List<T> AppendIf<T>(Func<T, bool> condition, List<T> list, params T[] items)
        {
            list = list ?? new List<T>();
            if (items != null)
            {
                foreach (var item in items)
                {
                    if (condition == null || condition.Invoke(item))
                        list.Add(item);
                }
            }
            return list;
        }

        /// <summary>
        /// Appends zero or more collections to a collection, result is a new <c>List</c>
        /// </summary>
        /// <typeparam name="T">Type of collection</typeparam>
        /// <param name="collection">A collection to which <c>items</c> will be appended</param>
        /// <param name="collections">Collections to append</param>
        /// <returns>A new <c>List</c></returns>
        public static List<T> Append<T>(IEnumerable<T> collection, params IEnumerable<T>[] collections)
        {
            var list = ToList(collection);
            if (collections != null)
            {
                foreach (var coll in collections)
                {
                    if (coll != null)
                        list.AddRange(coll);
                }
            }
            return list;
        }

        /// <summary>
        /// Appends zero or more collections to a <c>List</c>
        /// </summary>
        /// <typeparam name="T">Type of collection</typeparam>
        /// <param name="list">A <c>List</c> to which <c>items</c> will be appended</param>
        /// <param name="collections">Collections to append</param>
        /// <returns>The orig <c>List</c></returns>
        public static List<T> Append<T>(List<T> list, params IEnumerable<T>[] collections)
        {
            list = list ?? new List<T>();
            if (collections != null)
            {
                foreach (var coll in collections)
                {
                    if (coll != null)
                        list.AddRange(coll);
                }
            }
            return list;
        }

        /// <summary>
        /// Conditionally appends zero or more collections to a collection, result is a new <c>List</c>
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="condition"><c>true</c> or <c>false</c></param>
        /// <param name="collection">A collection to which <c>collections</c> will conditionally be appended</param>
        /// <param name="collections">Collections to conditionally append</param>
        /// <returns>A new <c>List</c></returns>
        public static List<T> AppendIf<T>(bool condition, IEnumerable<T> collection, params IEnumerable<T>[] collections)
        {
            var list = ToList(collection);
            if (condition && collections != null)
            {
                foreach (var coll in collections)
                {
                    if (coll != null)
                        list.AddRange(coll);
                }
            }
            return list;
        }

        /// <summary>
        /// Conditionally appends zero or more collections to a <c>List</c>
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="condition"><c>true</c> or <c>false</c></param>
        /// <param name="list">A <c>List</c> to which <c>items</c> will conditionally be appended</param>
        /// <param name="collections">Collections to conditionally append</param>
        /// <returns>The orig <c>List</c></returns>
        public static List<T> AppendIf<T>(bool condition, List<T> list, params IEnumerable<T>[] collections)
        {
            list = list ?? new List<T>();
            if (condition && collections != null)
            {
                foreach (var coll in collections)
                {
                    if (coll != null)
                        list.AddRange(coll);
                }
            }
            return list;
        }

        /// <summary>
        /// Conditionally appends zero or more collections to a collection, result is a new <c>List</c>
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="condition">A function that dictates whether an item gets appended</param>
        /// <param name="collection">A collection to which <c>collections</c> will be conditionally appended</param>
        /// <param name="collections">Collections to conditionaaly append</param>
        /// <returns>A new <c>List</c></returns>
        public static List<T> AppendIf<T>(Func<T, bool> condition, IEnumerable<T> collection, params IEnumerable<T>[] collections)
        {
            var list = ToList(collection);
            if (collections != null)
            {
                foreach (var coll in collections)
                {
                    if (coll != null)
                    {
                        foreach (var item in coll)
                        {
                            if (condition == null || condition.Invoke(item))
                                list.Add(item);
                        }
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Conditionally appends zero or more collections to a <c>List</c>
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="condition">A function that dictates whether an item gets appended</param>
        /// <param name="list">A <c>List</c> to which <c>collections</c> will be conditionally appended</param>
        /// <param name="collections">Collections to conditionaaly append</param>
        /// <returns>The orig <c>List</c></returns>
        public static List<T> AppendIf<T>(Func<T, bool> condition, List<T> list, params IEnumerable<T>[] collections)
        {
            list = list ?? new List<T>();
            if (collections != null)
            {
                foreach (var coll in collections)
                {
                    if (coll != null)
                    {
                        foreach (var item in coll)
                        {
                            if (condition == null || condition.Invoke(item))
                                list.Add(item);
                        }
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Inserts zero or more items into a collection, result is a new <c>List</c>
        /// </summary>
        /// <typeparam name="T">Type of collection</typeparam>
        /// <param name="collection">A collection to which <c>items</c> will be inserted</param>
        /// <param name="index">Where in the collection to insert <c>items</c></param>
        /// <param name="items">Items to insert</param>
        /// <returns>A new <c>List</c></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static List<T> Insert<T>(IEnumerable<T> collection, int index, params T[] items)
        {
            var list = ToList(collection);
            if (items != null)
            {
                foreach (var item in items)
                    list.Insert(index++, item);
            }
            return list;
        }

        /// <summary>
        /// Inserts zero or more items into a <c>List</c>
        /// </summary>
        /// <typeparam name="T">Type of <c>List</c></typeparam>
        /// <param name="list">A <c>List</c> to which <c>items</c> will be inserted</param>
        /// <param name="index">Where in the <c>List</c> to insert <c>items</c></param>
        /// <param name="items">Items to insert</param>
        /// <returns>The orig <c>List</c></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static List<T> Insert<T>(List<T> list, int index, params T[] items)
        {
            list = list ?? new List<T>();
            if (items != null)
            {
                foreach (var item in items)
                    list.Insert(index++, item);
            }
            return list;
        }

        /// <summary>
        /// Conditionally inserts zero or more items into a collection, result is a new <c>List</c>
        /// </summary>
        /// <typeparam name="T">Type of collection</typeparam>
        /// <param name="condition"><c>true</c> or <c>false</c></param>
        /// <param name="collection">A collection to which <c>items</c> will conditionally be inserted</param>
        /// <param name="index">Where in the collection to insert <c>items</c></param>
        /// <param name="items">Items to insert</param>
        /// <returns>A new <c>List</c></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static List<T> InsertIf<T>(bool condition, IEnumerable<T> collection, int index, params T[] items)
        {
            var list = ToList(collection);
            if (condition && items != null)
            {
                foreach (var item in items)
                    list.Insert(index++, item);
            }
            return list;
        }

        /// <summary>
        /// Conditionally inserts zero or more items into a <c>List</c>
        /// </summary>
        /// <typeparam name="T">Type of list</typeparam>
        /// <param name="condition"><c>true</c> or <c>false</c></param>
        /// <param name="list">A <c>List</c> to which <c>items</c> will be inserted</param>
        /// <param name="index">Where in the <c>List</c> to insert <c>items</c></param>
        /// <param name="items">Items to insert</param>
        /// <returns>The orig <c>List</c></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static List<T> InsertIf<T>(bool condition, List<T> list, int index, params T[] items)
        {
            list = list ?? new List<T>();
            if (condition && items != null)
            {
                foreach (var item in items)
                    list.Insert(index++, item);
            }
            return list;
        }

        /// <summary>
        /// Conditionally inserts zero or more items into a collection, result is a new <c>List</c>
        /// </summary>
        /// <typeparam name="T">Type of collection</typeparam>
        /// <param name="condition">A function that dictates whether an item gets inserted</param>
        /// <param name="collection">A collection to which <c>items</c> will be conditionally inserted</param>
        /// <param name="index">Where in the collection to insert <c>items</c></param>
        /// <param name="items">Items to conditionally insert</param>
        /// <returns>A new <c>List</c></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static List<T> InsertIf<T>(Func<T, bool> condition, IEnumerable<T> collection, int index, params T[] items)
        {
            var list = ToList(collection);
            if (items != null)
            {
                foreach (var item in items)
                {
                    if (condition == null || condition.Invoke(item))
                        list.Insert(index++, item);
                }
            }
            return list;
        }

        /// <summary>
        /// Conditionally inserts zero or more items into a <c>List</c>
        /// </summary>
        /// <typeparam name="T">Type of list</typeparam>
        /// <param name="condition">A function that dictates whether an item gets inserted</param>
        /// <param name="list">A <c>List</c> to which <c>items</c> will be conditionally inserted</param>
        /// <param name="index">Where in the <c>List</c> to insert <c>items</c></param>
        /// <param name="items">Items to conditionally insert</param>
        /// <returns>The orig <c>List</c></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static List<T> InsertIf<T>(Func<T, bool> condition, List<T> list, int index, params T[] items)
        {
            list = list ?? new List<T>();
            if (items != null)
            {
                foreach (var item in items)
                {
                    if (condition == null || condition.Invoke(item))
                        list.Insert(index++, item);
                }
            }
            return list;
        }

        /// <summary>
        /// Appends zero or more collections to a collection, result is a new <c>List</c>
        /// </summary>
        /// <typeparam name="T">Type of collection</typeparam>
        /// <param name="collection">A collection to which <c>items</c> will be inserted</param>
        /// <param name="index">Where in the collection to insert <c>items</c></param>
        /// <param name="collections">Collections to insert</param>
        /// <returns>A new <c>List</c></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static List<T> Insert<T>(IEnumerable<T> collection, int index, params IEnumerable<T>[] collections)
        {
            var list = ToList(collection);
            if (collections != null)
            {
                foreach (var coll in collections)
                {
                    if (coll != null)
                    {
                        foreach (var item in coll)
                            list.Insert(index++, item);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Appends zero or more collections to a <c>List</c>
        /// </summary>
        /// <typeparam name="T">Type of collection</typeparam>
        /// <param name="list">A <c>List</c> to which <c>items</c> will be inserted</param>
        /// <param name="index">Where in the <c>List</c> to insert <c>items</c></param>
        /// <param name="collections">Collections to insert</param>
        /// <returns>The orig <c>List</c></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static List<T> Insert<T>(List<T> list, int index, params IEnumerable<T>[] collections)
        {
            list = list ?? new List<T>();
            if (collections != null)
            {
                foreach (var coll in collections)
                {
                    if (coll != null)
                    {
                        foreach (var item in coll)
                            list.Insert(index++, item);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Conditionally inserts zero or more collections to a collection, result is a new <c>List</c>
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="condition"><c>true</c> or <c>false</c></param>
        /// <param name="collection">A collection to which <c>collections</c> will conditionally be inserted</param>
        /// <param name="index">Where in the collection to insert <c>items</c></param>
        /// <param name="collections">Collections to conditionally insert</param>
        /// <returns>A new <c>List</c></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static List<T> InsertIf<T>(bool condition, IEnumerable<T> collection, int index, params IEnumerable<T>[] collections)
        {
            var list = ToList(collection);
            if (condition && collections != null)
            {
                foreach (var coll in collections)
                {
                    if (coll != null)
                    {
                        foreach (var item in coll)
                            list.Insert(index++, item);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Conditionally inserts zero or more collections to a <c>List</c>
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="condition"><c>true</c> or <c>false</c></param>
        /// <param name="list">A <c>List</c> to which <c>items</c> will conditionally be inserted</param>
        /// <param name="index">Where in the <c>List</c> to insert <c>items</c></param>
        /// <param name="collections">Collections to conditionally insert</param>
        /// <returns>The orig <c>List</c></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static List<T> InsertIf<T>(bool condition, List<T> list, int index, params IEnumerable<T>[] collections)
        {
            list = list ?? new List<T>();
            if (condition && collections != null)
            {
                foreach (var coll in collections)
                {
                    if (coll != null)
                    {
                        foreach (var item in coll)
                            list.Insert(index++, item);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Conditionally inserts zero or more collections to a collection, result is a new <c>List</c>
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="condition">A function that dictates whether an item gets inserted</param>
        /// <param name="collection">A collection to which <c>collections</c> will be conditionally inserted</param>
        /// <param name="index">Where in the collection to insert <c>items</c></param>
        /// <param name="collections">Collections to conditionaaly insert</param>
        /// <returns>A new <c>List</c></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static List<T> InsertIf<T>(Func<T, bool> condition, IEnumerable<T> collection, int index, params IEnumerable<T>[] collections)
        {
            var list = ToList(collection);
            if (collections != null)
            {
                foreach (var coll in collections)
                {
                    if (coll != null)
                    {
                        foreach (var item in coll)
                        {
                            if (condition == null || condition.Invoke(item))
                                list.Insert(index++, item);
                        }
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Conditionally inserts zero or more collections to a <c>List</c>
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="condition">A function that dictates whether an item gets inserted</param>
        /// <param name="list">A <c>List</c> to which <c>collections</c> will be conditionally inserted</param>
        /// <param name="index">Where in the <c>List</c> to insert <c>items</c></param>
        /// <param name="collections">Collections to conditionaaly insert</param>
        /// <returns>The orig <c>List</c></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static List<T> InsertIf<T>(Func<T, bool> condition, List<T> list, int index, params IEnumerable<T>[] collections)
        {
            list = list ?? new List<T>();
            if (collections != null)
            {
                foreach (var coll in collections)
                {
                    if (coll != null)
                    {
                        foreach (var item in coll)
                        {
                            if (condition == null || condition.Invoke(item))
                                list.Insert(index++, item);
                        }
                    }
                }
            }
            return list;
        }

        ///// <summary>
        ///// Appends zero or more items to a list
        ///// </summary>
        ///// <typeparam name="T">Type of item</typeparam>
        ///// <param name="list">A list</param>
        ///// <param name="items">Items to append</param>
        ///// <returns>The appended list</returns>
        //public static List<T> Append_KeepOrig<T>(List<T> list, params T[] items)
        //{
        //    var collection = CollectionUtil.Append_KeepOrig(list, items);
        //    return AsList(collection);
        //}

        ///// <summary>
        ///// Conditionally appends zero or more items to a list
        ///// </summary>
        ///// <typeparam name="T">Type of item</typeparam>
        ///// <param name="condition"><c>true</c> or <c>false</c></param>
        ///// <param name="list">A list</param>
        ///// <param name="items">Items to append</param>
        ///// <returns>The appended list</returns>
        //public static List<T> AppendIf_KeepOrig<T>(bool condition, List<T> list, params T[] items)
        //{
        //    var collection = CollectionUtil.AppendIf_KeepOrig(condition, list, items);
        //    return AsList(collection);
        //}

        ///// <summary>
        ///// Conditionally appends zero or more items to a list
        ///// </summary>
        ///// <typeparam name="T">Type of item</typeparam>
        ///// <param name="condition">A required function that returns <c>true</c> or <c>false</c></param>
        ///// <param name="list">A list</param>
        ///// <param name="items">Items to append</param>
        ///// <returns>The appended list</returns>
        //public static List<T> AppendIf_KeepOrig<T>(Func<T, bool> condition, List<T> list, params T[] items)
        //{
        //    var collection = CollectionUtil.AppendIf_KeepOrig(condition, list, items);
        //    return AsList(collection);
        //}

        ///// <summary>
        ///// Appends zero or more collections to a list
        ///// </summary>
        ///// <typeparam name="T">Type of item</typeparam>
        ///// <param name="list">A list</param>
        ///// <param name="collections">Collections to append</param>
        ///// <returns>The appended list</returns>
        //public static List<T> Append_KeepOrig<T>(List<T> list, params IEnumerable<T>[] collections)
        //{
        //    var collection = CollectionUtil.Append_KeepOrig(list, collections);
        //    return AsList(collection);
        //}

        ///// <summary>
        ///// Conditionally appends zero or more collections to a list
        ///// </summary>
        ///// <typeparam name="T">Type of item</typeparam>
        ///// <param name="condition"><c>true</c> or <c>false</c></param>
        ///// <param name="list">A list</param>
        ///// <param name="collections">Collections to append</param>
        ///// <returns>The appended list</returns>
        //public static List<T> AppendIf_KeepOrig<T>(bool condition, List<T> list, params IEnumerable<T>[] collections)
        //{
        //    var collection = CollectionUtil.AppendIf_KeepOrig(condition, list, collections);
        //    return AsList(collection);
        //}

        ///// <summary>
        ///// Conditionally appends zero or more collections to a list
        ///// </summary>
        ///// <typeparam name="T">Type of item</typeparam>
        ///// <param name="condition">A required function that returns <c>true</c> or <c>false</c></param>
        ///// <param name="list">A list</param>
        ///// <param name="collections">Collections to append</param>
        ///// <returns>The appended list</returns>
        //public static List<T> AppendIf_KeepOrig<T>(Func<T, bool> condition, List<T> list, params IEnumerable<T>[] collections)
        //{
        //    var collection = CollectionUtil.AppendIf_KeepOrig(condition, list, collections);
        //    return AsList(collection);
        //}

        /// <summary>
        /// Replace each occurrance of <c>item</c> with <c>replacement</c>
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="list">A list</param>
        /// <param name="item">An item to replace</param>
        /// <param name="replacement">The replacement item</param>
        /// <param name="keepOriginalListDataSource"><c>true</c> will prevent internally creating a new <c>List&lt;T&gt;</c> if <c>collection</c> is already a <c>List&lt;T&gt;</c> instance - this is to improve performance</param>
        /// <returns>The modified list</returns>
        public static List<T> ReplaceAll<T>(List<T> list, T item, T replacement, bool keepOriginalListDataSource = false) where T : IEquatable<T>
        {
            var collection = CollectionUtil.ReplaceAll(list, item, replacement, keepOriginalListDataSource: keepOriginalListDataSource);
            return AsList(collection);
        }

        /// <summary>
        /// Replace each occurrance of <c>item</c> with <c>replacement</c>
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="list">A list</param>
        /// <param name="item">An item to replace</param>
        /// <param name="replacement">The replacement item</param>
        /// <param name="comparer">An equality comparer</param>
        /// <param name="keepOriginalListDataSource"><c>true</c> will prevent internally creating a new <c>List&lt;T&gt;</c> if <c>collection</c> is already a <c>List&lt;T&gt;</c> instance - this is to improve performance</param>
        /// <returns>The modified list</returns>
        public static List<T> ReplaceAll<T>(List<T> list, T item, T replacement, IEqualityComparer<T> comparer, bool keepOriginalListDataSource = false) where T : IEquatable<T>
        {
            var collection = CollectionUtil.ReplaceAll(list, item, replacement, comparer, keepOriginalListDataSource: keepOriginalListDataSource);
            return AsList(collection);
        }

        /// <summary>
        /// Gets and removes an item from a list
        /// </summary>
        /// <typeparam name="T">Type of list</typeparam>
        /// <param name="list">A list</param>
        /// <param name="index">The index to get and remove</param>
        /// <returns>The removed item</returns>
        public static T PopAt<T>(List<T> list, int index)
        {
            T t = list[index];
            list.RemoveAt(index);
            return t;
        }
    }
}
