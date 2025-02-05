using System;
using System.Collections.Generic;
using System.Linq;

namespace Horseshoe.NET.Collections
{
    /// <summary>
    /// A collection of <c>List&lt;T&gt;</c> and <c>List&lt;string&gt;</c> utility methods
    /// </summary>
    public static class ListUtil
    {

        /* * * * * * * * * * * * * * 
         *         GROUP 1
         * * * * * * * * * * * * * */

        /// <summary>
        /// Removes leading and trailing whitespace characters from each <c>string</c> in the collection.
        /// Optimizations are as follows.
        /// <list type="number">
        /// <item>
        /// If the source collection is of type <c>List&lt;string&gt;</c> that list is reused behind the scenes instead of instantiating a new <c>List&lt;string&gt;</c>.
        /// </item>
        /// <item>
        /// An index-based list scan is used in lieu of Linq-style collection transforms.
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="collection">A <c>string</c> collection</param>
        /// <returns>The collection as a <c>string</c> list</returns>
        public static List<string> TrimAll(IEnumerable<string> collection)
        {
            var list = CollectionUtil.ToList(collection, optimize: Optimization.ReuseCollection);
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = list[i]?.Trim();
            }
            return list;
        }

        /// <summary>
        /// Removes leading and trailing whitespace characters from each <c>string</c> in <c>collection</c> 
        /// converting blank values to <c>null</c>. 
        /// Additionally, depending on <c>pruneOptions</c>, some or all <c>null</c> values may be removed.
        /// Optimizations are as follows.
        /// <list type="number">
        /// <item>
        /// If the source collection is of type <c>List&lt;string&gt;</c> that list is reused behind the scenes instead of instantiating a new <c>List&lt;string&gt;</c>.
        /// </item>
        /// <item>
        /// Index-based list scans (2 or 4 depending on <c>pruneOptions</c>) are used in lieu of Linq-style collection filtering.
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="collection">A <c>string</c> collection</param>
        /// <param name="pruneOptions">Pruning options for <c>null</c> values (e.g. <c>Leading</c>, <c>Trailing</c>, <c>All</c>..., default is <c>None</c>)</param>
        /// <returns>The modified collection as <c>List&lt;string&gt;</c></returns>
        public static List<string> Zap(IEnumerable<string> collection, PruneOptions pruneOptions = default)
        {
            // first scan
            var trimmed = TrimAll(collection);

            // second scan - nullify (from beginning to end)
            for (int i = 0; i < trimmed.Count; i++)
            {
                if (string.IsNullOrEmpty(trimmed[i]))
                    trimmed[i] = null;
            }

            // third & fourth scan - prune null items
            PruneInternal(trimmed, pruneOptions: pruneOptions);
            return trimmed;
        }

        /// <summary>
        /// Removes some or all <c>null</c> values from <c>collection</c>.
        /// Optimizations are as follows.
        /// <list type="number">
        /// <item>
        /// If the source collection is of type <c>List&lt;T&gt;</c> that list is reused behind the scenes instead of instantiating a new <c>List&lt;T&gt;</c>.
        /// </item>
        /// <item>
        /// Index-based list scans (0 or 2 depending on <c>pruneOptions</c>) are used in lieu of Linq-style collection filtering.
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">Type of collection</typeparam>
        /// <param name="collection">A collection</param>
        /// <param name="pruneOptions">Pruning options for <c>null</c> values (e.g. <c>Leading</c>, <c>Trailing</c>..., default is <c>All</c>)</param>
        /// <returns>The modified collection as <c>List&lt;T&gt;</c></returns>
        public static List<T> Prune<T>(IEnumerable<T> collection, PruneOptions pruneOptions = PruneOptions.All)
        {
            var list = CollectionUtil.ToList(collection, optimize: Optimization.ReuseCollection);
            PruneInternal(list, pruneOptions: pruneOptions);
            return list;
        }

        // Index-based list scans (0 or 2 depending on <c>pruneOptions</c>).
        private static void PruneInternal<T>(List<T> list, PruneOptions pruneOptions)
        {
            if (pruneOptions == PruneOptions.None)
                return;

            int firstNonNullIndex = 0;
            bool foundNonNull = false;

            // first scan (partial) - find first non-null index
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] != null)
                {
                    firstNonNullIndex = i;
                    break;
                }
            }

            // second scan - prune by position (from end to beginning)
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i] == null)
                {
                    if (foundNonNull)
                    {
                        if (i < firstNonNullIndex)
                        {
                            if ((pruneOptions & PruneOptions.Leading) == PruneOptions.Leading)  // last to go
                                list.RemoveAt(i);
                        }
                        else if ((pruneOptions & PruneOptions.Inner) == PruneOptions.Inner)     // second to go
                            list.RemoveAt(i);
                    }
                    else if ((pruneOptions & PruneOptions.Trailing) == PruneOptions.Trailing)   // first to go
                        list.RemoveAt(i);
                }
                else
                    foundNonNull = true;
            }
        }

        /* * * * * * * * * * * * * * 
         *         GROUP 2
         * * * * * * * * * * * * * */

        /// <summary>
        /// Inflates a collection to the desired target size by padding items at the indicated boundary
        /// Optimizations are as follows.
        /// <list type="number">
        /// <item>
        /// If the source collection is of type <c>List&lt;T&gt;</c> that list is reused under the hood instead of instantiating a new one.
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="collection">A collection</param>
        /// <param name="targetSize">The desired size of the list after padding</param>
        /// <param name="boundary">End (default) or Start</param>
        /// <param name="padWith">Item to use for padding</param>
        /// <param name="cannotExceedTargetSize"><c>true</c> if an exception should be thrown for oversized lists</param>
        /// <returns>The resized list</returns>
        /// <exception cref="CollectionException"></exception>
        /// <exception cref="ValidationException"></exception>
        public static List<T> Pad<T>(IEnumerable<T> collection, int targetSize, CollectionBoundary boundary = default, T padWith = default, bool cannotExceedTargetSize = false)
        {
            if (collection == null)
                collection = new List<T>();

            if (targetSize < 0)
                throw new ValidationException(nameof(targetSize) + " cannot be a negative number");

            if (collection.Count() > targetSize && cannotExceedTargetSize)
                throw new CollectionException("source collection already exceeds target size");

            var list = CollectionUtil.ToList(collection, optimize: Optimization.ReuseCollection);

            while (list.Count < targetSize)
            {
                switch (boundary)
                {
                    case CollectionBoundary.Start:
                    default:
                        list.Insert(0, padWith);
                        break;
                    case CollectionBoundary.End:
                        list.Add(padWith);
                        break;
                }
            }
            return list;
        }

        /// <summary>
        /// Shrinks a collection to the desired target size by removing items at the indicated boundary
        /// Optimizations are as follows.
        /// <list type="number">
        /// <item>
        /// If the source collection is of type <c>List&lt;T&gt;</c> that list is reused under the hood instead of instantiating a new one.
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="collection">A collection</param>
        /// <param name="targetSize">The desired size of the list after padding</param>
        /// <param name="boundary">End (default) or Start</param>
        /// <returns>The resized list</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="ValidationException"></exception>
        public static List<T> Crop<T>(IEnumerable<T> collection, int targetSize, CollectionBoundary boundary = default)
        {
            if (targetSize < 0)
                throw new ValidationException(nameof(targetSize) + " cannot be a negative number");

            var list = CollectionUtil.ToList(collection, optimize: Optimization.ReuseCollection);

            while (list.Count > targetSize)
            {
                switch (boundary)
                {
                    case CollectionBoundary.Start:
                    default:
                        list.RemoveAt(0);
                        break;
                    case CollectionBoundary.End:
                        list.RemoveAt(list.Count - 1);
                        break;
                }
            }
            return list;
        }

        /// <summary>
        /// Inflates or shrinks a collection to the desired target size by padding or removing items at the indicated boundary
        /// Optimizations are as follows.
        /// <list type="number">
        /// <item>
        /// If the source collection is of type <c>List&lt;T&gt;</c> that list is reused under the hood instead of instantiating a new one.
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="collection">A collection</param>
        /// <param name="targetSize">The desired size of the list after padding</param>
        /// <param name="cropBoundary">End (default) or Start</param>
        /// <param name="padBoundary">End (default) or Start</param>
        /// <param name="padWith">Item to use for padding</param>
        /// <returns>The resized list</returns>
        /// <exception cref="CollectionException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="ValidationException"></exception>
        public static List<T> Fit<T>(IEnumerable<T> collection, int targetSize, CollectionBoundary cropBoundary = default, CollectionBoundary padBoundary = default, T padWith = default)
        {
            if (collection.Count() > targetSize)
            {
                return Crop(collection, targetSize, boundary: cropBoundary);
            }
            else
            {
                return Pad(collection, targetSize, boundary: padBoundary, padWith: padWith, cannotExceedTargetSize: false);
            }
        }

        /* * * * * * * * * * * * * * 
         *         GROUP 3
         * * * * * * * * * * * * * */

        /// <summary>
        /// Appends zero or more items to a collection. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If there are no items to append returns the original <c>List&lt;T&gt;</c> if applicable.
        /// </item>
        /// <item>
        /// In the case that items are appended, an instance of <c>List&lt;T&gt;</c> will be 
        /// created and used as the underlying collection unless the source collection is 
        /// already a <c>List&lt;T&gt;</c> in which case it will be reused and returned.
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">List type</typeparam>
        /// <param name="collection">The collection to which <c>items</c> will be appended</param>
        /// <param name="items">Items to append</param>
        /// <returns>A <c>List&lt;T&gt;</c></returns>
        public static List<T> Append<T>(IEnumerable<T> collection, IEnumerable<T> items)
        {
            if (items == null || !items.Any()) // optimization 1
                return CollectionUtil.ToList(collection, optimize: Optimization.ReuseCollection);
            var list = CollectionUtil.ToList(collection, optimize: Optimization.ReuseCollection); // optimization 2
            list.AddRange(items);  // prefer this over _AppendCollectionInternal(list, items) to prevent double validation
            return list;
        }

        /// <summary>
        /// Appends zero or more items to a collection. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If there are no items to append returns the original <c>List&lt;T&gt;</c> if applicable.
        /// </item>
        /// <item>
        /// In the case that items are appended, an instance of <c>List&lt;T&gt;</c> will be 
        /// created and used as the underlying collection unless the source collection is 
        /// already a <c>List&lt;T&gt;</c> in which case it will be reused and returned.
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">Collection type</typeparam>
        /// <param name="collection">The collection to which <c>items</c> will be appended</param>
        /// <param name="items">Items to append</param>
        /// <returns>A <c>List&lt;T&gt;</c></returns>
        public static List<T> Append<T>(IEnumerable<T> collection, params T[] items)
        {
            return Append(collection, items as IEnumerable<T>);
        }

        /// <summary>
        /// Conditionally appends zero or more items to a collection. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If <c>condition == false</c> or there are no items to append returns the original <c>List&lt;T&gt;</c> if applicable.
        /// </item>
        /// <item>
        /// In the case that items are appended, an instance of <c>List&lt;T&gt;</c> will be 
        /// created and used as the underlying collection unless the source collection is 
        /// already a <c>List&lt;T&gt;</c> in which case it will be reused and returned.
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">Collection type</typeparam>
        /// <param name="collection">The collection to which <c>items</c> will be appended</param>
        /// <param name="condition">Whether or not to append the item(s)</param>
        /// <param name="items">Items to append</param>
        /// <returns>A <c>List&lt;T&gt;</c></returns>
        public static List<T> AppendIf<T>(IEnumerable<T> collection, bool condition, IEnumerable<T> items)
        {
            if (!condition)
                return CollectionUtil.ToList(collection, optimize: Optimization.ReuseCollection);
            return Append(collection, items);
        }

        /// <summary>
        /// Conditionally appends zero or more items to a collection. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If <c>condition == false</c> or there are no items to append returns the original <c>List&lt;T&gt;</c> if applicable.
        /// </item>
        /// <item>
        /// In the case that items are appended, an instance of <c>List&lt;T&gt;</c> will be 
        /// created and used as the underlying collection unless the source collection is 
        /// already a <c>List&lt;T&gt;</c> in which case it will be reused and returned.
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">Collection type</typeparam>
        /// <param name="collection">The collection to which <c>items</c> will be appended</param>
        /// <param name="condition">Whether or not to append the item(s)</param>
        /// <param name="items">Items to append</param>
        /// <returns>A <c>List&lt;T&gt;</c></returns>
        public static List<T> AppendIf<T>(IEnumerable<T> collection, bool condition, params T[] items)
        {
            return AppendIf(collection, condition, items as IEnumerable<T>);
        }

        /// <summary>
        /// Appends zero or more collections to a source collection. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If there are no items to append returns the original <c>List&lt;T&gt;</c> if applicable.
        /// </item>
        /// <item>
        /// In the case that collections are appended, an instance of <c>List&lt;T&gt;</c> will be 
        /// created and used as the underlying collection unless the source collection is 
        /// already a <c>List&lt;T&gt;</c> in which case it will be reused and returned.
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">Collection type</typeparam>
        /// <param name="collection">The collection to which <c>collections</c> will be appended</param>
        /// <param name="collections">Collections to append</param>
        /// <returns>A <c>List&lt;T&gt;</c></returns>
        public static List<T> Append<T>(IEnumerable<T> collection, IEnumerable<IEnumerable<T>> collections)
        {
            if (collections == null || !collections.Any()) // optimization 1
                return CollectionUtil.ToList(collection, optimize: Optimization.ReuseCollection);
            var list = CollectionUtil.ToList(collection, optimize: Optimization.ReuseCollection); // optimization 2
            foreach (var _collection in collections)
                _AppendCollectionInternal(list, _collection);
            return list;
        }

        /// <summary>
        /// Appends zero or more collections to a source collection. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If there are no items to append returns the original <c>List&lt;T&gt;</c> if applicable.
        /// </item>
        /// <item>
        /// In the case that collections are appended, an instance of <c>List&lt;T&gt;</c> will be 
        /// created and used as the underlying collection unless the source collection is 
        /// already a <c>List&lt;T&gt;</c> in which case it will be reused and returned.
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">Collection type</typeparam>
        /// <param name="collection">The collection to which <c>collections</c> will be appended</param>
        /// <param name="collections">Collections to append</param>
        /// <returns>A <c>List&lt;T&gt;</c></returns>
        public static List<T> Append<T>(IEnumerable<T> collection, params IEnumerable<T>[] collections)
        {
            return Append(collection, collections as IEnumerable<IEnumerable<T>>);
        }

        /// <summary>
        /// Conditionally appends zero or more collections to a source collection. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If <c>condition == false</c> or there are no items to append returns the original <c>List&lt;T&gt;</c> if applicable.
        /// </item>
        /// <item>
        /// In the case that collections are appended, an instance of <c>List&lt;T&gt;</c> will be 
        /// created and used as the underlying collection unless the source collection is 
        /// already a <c>List&lt;T&gt;</c> in which case it will be reused and returned.
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">Collection type</typeparam>
        /// <param name="collection">The collection to which <c>collections</c> will be appended</param>
        /// <param name="condition">Whether or not to append the collection(s)</param>
        /// <param name="collections">Collections to append</param>
        /// <returns>A <c>List&lt;T&gt;</c></returns>
        public static List<T> AppendIf<T>(IEnumerable<T> collection, bool condition, IEnumerable<IEnumerable<T>> collections)
        {
            if (!condition)
                return CollectionUtil.ToList(collection, optimize: Optimization.ReuseCollection);
            return Append(collection, collections);
        }

        /// <summary>
        /// Conditionally appends zero or more collections to a source collection. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If <c>condition == false</c> or there are no items to append returns the original <c>List&lt;T&gt;</c> if applicable.
        /// </item>
        /// <item>
        /// In the case that collections are appended, an instance of <c>List&lt;T&gt;</c> will be 
        /// created and used as the underlying collection unless the source collection is 
        /// already a <c>List&lt;T&gt;</c> in which case it will be reused and returned.
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">Collection type</typeparam>
        /// <param name="collection">The collection to which <c>collections</c> will be appended</param>
        /// <param name="condition">Whether or not to append the collection(s)</param>
        /// <param name="collections">Collections to append</param>
        /// <returns>A <c>List&lt;T&gt;</c></returns>
        public static List<T> AppendIf<T>(IEnumerable<T> collection, bool condition, params IEnumerable<T>[] collections)
        {
            return AppendIf(collection, condition, collections as IEnumerable<IEnumerable<T>>);
        }

        private static void _AppendCollectionInternal<T>(List<T> list, IEnumerable<T> collection)
        {
            if (collection != null && collection.Any())
                list.AddRange(collection);
        }

        /// <summary>
        /// Inserts zero or more items into a collection. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If there are no items to insert returns the original <c>List&lt;T&gt;</c> if applicable.
        /// </item>
        /// <item>
        /// In the case that items are inserted, an instance of <c>List&lt;T&gt;</c> will be 
        /// created and used as the underlying collection unless the source collection is 
        /// already a <c>List&lt;T&gt;</c> in which case it will be reused and returned.
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">Collection type</typeparam>
        /// <param name="collection">The collection into which <c>items</c> will be inserted</param>
        /// <param name="index">The position in the collection to insert the item(s)</param>
        /// <param name="items">Items to insert</param>
        /// <returns>A <c>List&lt;T&gt;</c></returns>
        public static List<T> Insert<T>(IEnumerable<T> collection, int index, IEnumerable<T> items)
        {
            if (items == null || !items.Any()) // optimization 1
                return CollectionUtil.ToList(collection, optimize: Optimization.ReuseCollection);
            var list = CollectionUtil.ToList(collection, optimize: Optimization.ReuseCollection); // optimization 2
            foreach (var item in items)
                list.Insert(index++, item);
            return list;
        }

        /// <summary>
        /// Inserts zero or more items into a collection. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If there are no items to insert returns the original <c>List&lt;T&gt;</c> if applicable.
        /// </item>
        /// <item>
        /// In the case that items are inserted, an instance of <c>List&lt;T&gt;</c> will be 
        /// created and used as the underlying collection unless the source collection is 
        /// already a <c>List&lt;T&gt;</c> in which case it will be reused and returned.
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">Collection type</typeparam>
        /// <param name="collection">The collection into which <c>items</c> will be inserted</param>
        /// <param name="index">The position in the collection to insert the item(s)</param>
        /// <param name="items">Items to insert</param>
        /// <returns>A <c>List&lt;T&gt;</c></returns>
        public static List<T> Insert<T>(IEnumerable<T> collection, int index, params T[] items)
        {
            return Insert(collection, index, items as IEnumerable<T>);
        }

        /// <summary>
        /// Conditionally inserts zero or more items into a collection. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If <c>condition == false</c> or there are no items to insert returns the original <c>List&lt;T&gt;</c> if applicable.
        /// </item>
        /// <item>
        /// In the case that items are inserteded, an instance of <c>List&lt;T&gt;</c> will be 
        /// created and used as the underlying collection unless the source collection is 
        /// already a <c>List&lt;T&gt;</c> in which case it will be reused and returned.
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">Collection type</typeparam>
        /// <param name="collection">The collection into which <c>items</c> will be inserted</param>
        /// <param name="condition">Whether or not to insert the item(s)</param>
        /// <param name="index">The position in the collection to insert the item(s)</param>
        /// <param name="items">Items to insert</param>
        /// <returns>A <c>List&lt;T&gt;</c></returns>
        public static List<T> InsertIf<T>(IEnumerable<T> collection, bool condition, int index, IEnumerable<T> items)
        {
            if (!condition)
                return CollectionUtil.ToList(collection, optimize: Optimization.ReuseCollection);
            return Insert(collection, index, items);
        }

        /// <summary>
        /// Conditionally inserts zero or more items into a collection. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If <c>condition == false</c> or there are no items to insert returns the original <c>List&lt;T&gt;</c> if applicable.
        /// </item>
        /// <item>
        /// In the case that items are inserted, an instance of <c>List&lt;T&gt;</c> will be 
        /// created and used as the underlying collection unless the source collection is 
        /// already a <c>List&lt;T&gt;</c> in which case it will be reused and returned.
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">Collection type</typeparam>
        /// <param name="collection">The collection into which <c>items</c> will be inserted</param>
        /// <param name="condition">Whether or not to insert the item(s)</param>
        /// <param name="index">The position in the collection to insert the item(s)</param>
        /// <param name="items">Items to insert</param>
        /// <returns>A <c>List&lt;T&gt;</c></returns>
        public static List<T> InsertIf<T>(IEnumerable<T> collection, bool condition, int index, params T[] items)
        {
            return InsertIf(collection, condition, index, items as IEnumerable<T>);
        }

        /// <summary>
        /// Inserts zero or more collections into a source collection. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If there are no items to insert returns the original <c>List&lt;T&gt;</c> if applicable.
        /// </item>
        /// <item>
        /// In the case that collections are inserted, an instance of <c>List&lt;T&gt;</c> will be 
        /// created and used as the underlying collection unless the source collection is 
        /// already a <c>List&lt;T&gt;</c> in which case it will be reused and returned.
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">Collection type</typeparam>
        /// <param name="collection">The collection into which <c>collections</c> will be inserted</param>
        /// <param name="index">The position in the collection to insert the collections(s)</param>
        /// <param name="collections">Collections to insert</param>
        /// <returns>A <c>List&lt;T&gt;</c></returns>
        public static List<T> Insert<T>(IEnumerable<T> collection, int index, IEnumerable<IEnumerable<T>> collections)
        {
            if (collections == null || !collections.Any()) // optimization 1
                return CollectionUtil.ToList(collection, optimize: Optimization.ReuseCollection);
            var list = CollectionUtil.ToList(collection, optimize: Optimization.ReuseCollection); // optimization 2
            foreach (var _collection in collections)
            {
                _InsertCollectionInternal(list, index, _collection, out int lastIndex);
                index = lastIndex;
            }
            return list;
        }

        /// <summary>
        /// Inserts zero or more collections into a source collection. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If there are no items to insert returns the original <c>List&lt;T&gt;</c> if applicable.
        /// </item>
        /// <item>
        /// In the case that collections are inserted, an instance of <c>List&lt;T&gt;</c> will be 
        /// created and used as the underlying collection unless the source collection is 
        /// already a <c>List&lt;T&gt;</c> in which case it will be reused and returned.
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">Collection type</typeparam>
        /// <param name="collection">The collection into which <c>collections</c> will be inserted</param>
        /// <param name="index">The position in the collection to insert the collections(s)</param>
        /// <param name="collections">Collections to insert</param>
        /// <returns>A <c>List&lt;T&gt;</c></returns>
        public static List<T> Insert<T>(IEnumerable<T> collection, int index, params IEnumerable<T>[] collections)
        {
            return Insert(collection, index, collections as IEnumerable<IEnumerable<T>>);
        }

        /// <summary>
        /// Conditionally inserts zero or more collections into a source collection. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If <c>condition == false</c> or there are no items to insert returns the original <c>List&lt;T&gt;</c> if applicable.
        /// </item>
        /// <item>
        /// In the case that collections are inserted, an instance of <c>List&lt;T&gt;</c> will be 
        /// created and used as the underlying collection unless the source collection is 
        /// already a <c>List&lt;T&gt;</c> in which case it will be reused and returned.
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">Collection type</typeparam>
        /// <param name="collection">The collection into which <c>collections</c> will be inserted</param>
        /// <param name="condition">Whether or not to insert the collection(s)</param>
        /// <param name="index">The position in the collection to insert the collections(s)</param>
        /// <param name="collections">Collections to insert</param>
        /// <returns>A <c>List&lt;T&gt;</c></returns>
        public static List<T> InsertIf<T>(IEnumerable<T> collection, bool condition, int index, IEnumerable<IEnumerable<T>> collections)
        {
            if (!condition)
                return CollectionUtil.ToList(collection, optimize: Optimization.ReuseCollection);
            return Insert(collection, index, collections);
        }

        /// <summary>
        /// Conditionally inserts zero or more collections into a source collection. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If <c>condition == false</c> or there are no items to insert returns the original <c>List&lt;T&gt;</c> if applicable.
        /// </item>
        /// <item>
        /// In the case that collections are inserted, an instance of <c>List&lt;T&gt;</c> will be 
        /// created and used as the underlying collection unless the source collection is 
        /// already a <c>List&lt;T&gt;</c> in which case it will be reused and returned.
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">Collection type</typeparam>
        /// <param name="collection">The collection into which <c>collections</c> will be inserted</param>
        /// <param name="condition">Whether or not to insert the collection(s)</param>
        /// <param name="index">The position in the collection to insert the collections(s)</param>
        /// <param name="collections">Collections to insert</param>
        /// <returns>A <c>List&lt;T&gt;</c></returns>
        public static List<T> InsertIf<T>(IEnumerable<T> collection, bool condition, int index, params IEnumerable<T>[] collections)
        {
            return InsertIf(collection, condition, index, collections as IEnumerable<IEnumerable<T>>);
        }

        private static void _InsertCollectionInternal<T>(List<T> list, int index, IEnumerable<T> collection, out int lastIndex)
        {
            if (collection == null || !collection.Any())
            {
                lastIndex = index;
                return;
            }
            foreach (var item in collection)
                list.Insert(index++, item);
            lastIndex = index;
        }

        /// <summary>
        /// Prepends zero or more items to a collection. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If there are no items to prepend returns the original <c>List&lt;T&gt;</c> if applicable.
        /// </item>
        /// <item>
        /// In the case that items are prepended, an instance of <c>List&lt;T&gt;</c> will be 
        /// created and used as the underlying collection unless the source collection is 
        /// already a <c>List&lt;T&gt;</c> in which case it will be reused and returned.
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">Collection type</typeparam>
        /// <param name="collection">The collection to which <c>items</c> will be prepended</param>
        /// <param name="items">Items to prepend</param>
        /// <returns>A <c>List&lt;T&gt;</c></returns>
        public static List<T> Prepend<T>(IEnumerable<T> collection, IEnumerable<T> items)
        {
            return Insert(collection, 0, items);
        }

        /// <summary>
        /// Prepends zero or more items to a collection. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If there are no items to prepend returns the original <c>List&lt;T&gt;</c> if applicable.
        /// </item>
        /// <item>
        /// In the case that items are prepended, an instance of <c>List&lt;T&gt;</c> will be 
        /// created and used as the underlying collection unless the source collection is 
        /// already a <c>List&lt;T&gt;</c> in which case it will be reused and returned.
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">Collection type</typeparam>
        /// <param name="collection">The collection to which <c>items</c> will be prepended</param>
        /// <param name="items">Items to prepend</param>
        /// <returns>A <c>List&lt;T&gt;</c></returns>
        public static List<T> Prepend<T>(IEnumerable<T> collection, params T[] items)
        {
            return Insert(collection, 0, items);
        }

        /// <summary>
        /// Conditionally prepends zero or more items to a collection. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If <c>condition == false</c> or there are no items to prepend returns the original <c>List&lt;T&gt;</c> if applicable.
        /// </item>
        /// <item>
        /// In the case that items are prepended, an instance of <c>List&lt;T&gt;</c> will be 
        /// created and used as the underlying collection unless the source collection is 
        /// already a <c>List&lt;T&gt;</c> in which case it will be reused and returned.
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">Collection type</typeparam>
        /// <param name="collection">The collection to which <c>items</c> will be prepended</param>
        /// <param name="condition">Whether or not to prepend the item(s)</param>
        /// <param name="items">Items to prepend</param>
        /// <returns>A <c>List&lt;T&gt;</c></returns>
        public static List<T> PrependIf<T>(IEnumerable<T> collection, bool condition, IEnumerable<T> items)
        {
            return InsertIf(collection, condition, 0, items);
        }

        /// <summary>
        /// Conditionally prepends zero or more items to a collection. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If <c>condition == false</c> or there are no items to prepend returns the original <c>List&lt;T&gt;</c> if applicable.
        /// </item>
        /// <item>
        /// In the case that items are prepended, an instance of <c>List&lt;T&gt;</c> will be 
        /// created and used as the underlying collection unless the source collection is 
        /// already a <c>List&lt;T&gt;</c> in which case it will be reused and returned.
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">Collection type</typeparam>
        /// <param name="collection">The collection to which <c>items</c> will be prepended</param>
        /// <param name="condition">Whether or not to prepend the item(s)</param>
        /// <param name="items">Items to prepend</param>
        /// <returns>A <c>List&lt;T&gt;</c></returns>
        public static List<T> PrependIf<T>(IEnumerable<T> collection, bool condition, params T[] items)
        {
            return InsertIf(collection, condition, 0, items);
        }

        /// <summary>
        /// Prepends zero or more collections to a source collection. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If there are no items to prepend returns the original <c>List&lt;T&gt;</c> if applicable.
        /// </item>
        /// <item>
        /// In the case that collections are prepended, an instance of <c>List&lt;T&gt;</c> will be 
        /// created and used as the underlying collection unless the source collection is 
        /// already a <c>List&lt;T&gt;</c> in which case it will be reused and returned.
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">Collection type</typeparam>
        /// <param name="collection">The collection to which <c>collections</c> will be prepend</param>
        /// <param name="collections">Collections to prepend</param>
        /// <returns>A <c>List&lt;T&gt;</c></returns>
        public static List<T> Prepend<T>(IEnumerable<T> collection, IEnumerable<IEnumerable<T>> collections)
        {
            return Insert(collection, 0, collections);
        }

        /// <summary>
        /// Prepends zero or more collections to a source collection. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If there are no items to prepend returns the original <c>List&lt;T&gt;</c> if applicable.
        /// </item>
        /// <item>
        /// In the case that collections are prepended, an instance of <c>List&lt;T&gt;</c> will be 
        /// created and used as the underlying collection unless the source collection is 
        /// already a <c>List&lt;T&gt;</c> in which case it will be reused and returned.
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">Collection type</typeparam>
        /// <param name="collection">The collection to which <c>collections</c> will be prepended</param>
        /// <param name="collections">Collections to prepend</param>
        /// <returns>A <c>List&lt;T&gt;</c></returns>
        public static List<T> Prepend<T>(IEnumerable<T> collection, params IEnumerable<T>[] collections)
        {
            return Insert(collection, 0, collections);
        }

        /// <summary>
        /// Conditionally prepends zero or more collections to a source collection. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If <c>condition == false</c> or there are no items to prepend returns the original <c>List&lt;T&gt;</c> if applicable.
        /// </item>
        /// <item>
        /// In the case that collections are prepended, an instance of <c>List&lt;T&gt;</c> will be 
        /// created and used as the underlying collection unless the source collection is 
        /// already a <c>List&lt;T&gt;</c> in which case it will be reused and returned.
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">Collection type</typeparam>
        /// <param name="collection">The collection to which <c>collections</c> will be prepended</param>
        /// <param name="condition">Whether or not to prepend the item(s)</param>
        /// <param name="collections">Collections to prepend</param>
        /// <returns>A <c>List&lt;T&gt;</c></returns>
        public static List<T> PrependIf<T>(IEnumerable<T> collection, bool condition, IEnumerable<IEnumerable<T>> collections)
        {
            return InsertIf(collection, condition, 0, collections);
        }

        /// <summary>
        /// Conditionally prepends zero or more collections to a source collection. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If <c>condition == false</c> or there are no items to prepend returns the original <c>List&lt;T&gt;</c> if applicable.
        /// </item>
        /// <item>
        /// In the case that collections are prepended, an instance of <c>List&lt;T&gt;</c> will be 
        /// created and used as the underlying collection unless the source collection is 
        /// already a <c>List&lt;T&gt;</c> in which case it will be reused and returned.
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">Collection type</typeparam>
        /// <param name="collection">The collection to which <c>collections</c> will be prepended</param>
        /// <param name="condition">Whether or not to prepend the item(s)</param>
        /// <param name="collections">Collections to prepend</param>
        /// <returns>A <c>List&lt;T&gt;</c></returns>
        public static List<T> PrependIf<T>(IEnumerable<T> collection, bool condition, params IEnumerable<T>[] collections)
        {
            return InsertIf(collection, condition, 0, collections);
        }

        /* * * * * * * * * * * * * * 
         *         GROUP 4
         * * * * * * * * * * * * * */

        /// <summary>
        /// Gets and removes an item from the end of a list
        /// </summary>
        /// <typeparam name="T">Type of list</typeparam>
        /// <param name="list">A list</param>
        /// <returns>The removed item</returns>
        public static T Pop<T>(List<T> list)
        {
            if (list == null || list.Count == 0)
                return default;
            T t = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            return t;
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

        /// <summary>
        /// Replace each occurrance of <c>item</c> with <c>replacement</c>.
        /// A <c>null</c> source list results in an empty <c>List&lt;T&gt;</c>.
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="list">A list</param>
        /// <param name="item">An item to replace</param>
        /// <param name="replacement">The replacement item</param>
        /// <returns>The modified list</returns>
        public static List<T> ReplaceAll<T>(List<T> list, T item, T replacement)
        {
            if (list == null)
                return new List<T>();
            for (int i = 0; i < list.Count; i++)
            {
                if (Equals(list[i], item))
                    list[i] = replacement;
            }
            return list;
        }

        /// <summary>
        /// Replace each occurrance of <c>item</c> with <c>replacement</c>.
        /// A <c>null</c> source list results in an empty <c>List&lt;T&gt;</c>.
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="list">A list</param>
        /// <param name="item">An item to replace</param>
        /// <param name="replacement">The replacement item</param>
        /// <param name="comparer">An equality comparer</param>
        /// <returns>The modified list</returns>
        public static List<T> ReplaceAll<T>(List<T> list, T item, T replacement, IEqualityComparer<T> comparer)
        {
            if (list == null)
                return new List<T>();
            for (int i = 0; i < list.Count; i++)
            {
                if (comparer.Equals(list[i], item))
                    list[i] = replacement;
            }
            return list;
        }

        /// <summary>
        /// Replace each occurrance of <c>item</c> with <c>replacement</c> using case-insensitive comparison.
        /// A <c>null</c> source list results in an empty <c>List&lt;string&gt;</c>.
        /// </summary>
        /// <param name="list">A list of <c>string</c></param>
        /// <param name="item">A <c>string</c> to replace</param>
        /// <param name="replacement">The replacement <c>string</c></param>
        /// <returns>The modified list</returns>
        public static List<string> ReplaceAllIgnoreCase(List<string> list, string item, string replacement)
        {
            if (list == null)
                return new List<string>();
            for (int i = 0; i < list.Count; i++)
            {
                if (string.Equals(list[i], item, StringComparison.OrdinalIgnoreCase))
                    list[i] = replacement;
            }
            return list;
        }

        /// <summary>
        /// Compares two lists for equality. Empty and <c>null</c> lists are considered identical.
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="controlList">A list to search</param>
        /// <param name="compareList">A list to compare</param>
        /// <param name="optimize">
        /// Optimization options:
        /// <list type="bullet">
        /// <item>
        /// Distinct - use this option to shrink the source lists down to distinct values prior to searching.
        /// </item>
        /// </list>
        /// </param>
        /// <param name="ignoreOrder"><c>true</c> if not an order dependent comparison</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool IsIdentical<T>(List<T> controlList, List<T> compareList, Optimization optimize = default, bool ignoreOrder = false)
        {
            if (controlList == null || !controlList.Any())
            {
                if (compareList == null || !compareList.Any())
                    return true;
                return false;
            }
            else if (compareList == null || !compareList.Any())
                return false;

            if ((optimize & Optimization.Distinct) == Optimization.Distinct)
            {
                var cache = new List<T>();  // looping in lieu of IEnumerable.Distinct() in an attempt to optimize 
                for (int i = controlList.Count - 1; i >= 0; i--)
                {
                    if (cache.Contains(controlList[i]))
                        controlList.RemoveAt(i);
                    else
                        cache.Add(controlList[i]);
                }
                cache.Clear();
                for (int i = compareList.Count - 1; i >= 0; i--)
                {
                    if (cache.Contains(compareList[i]))
                        compareList.RemoveAt(i);
                    else
                        cache.Add(compareList[i]);
                }

                if (controlList.Count != compareList.Count)
                    throw new CollectionException("Distinct lists are not the same length");
            }
            else if (controlList.Count != compareList.Count)
                throw new CollectionException("Lists are not the same length");

            if (ignoreOrder)
            {
                var compareListCopy = CollectionUtil.ToList(compareList);
                bool itemRemoved;
                foreach (var item in controlList)
                {
                    itemRemoved = false;
                    for (var i = 0; i < compareListCopy.Count; i++)
                    {
                        if (Equals(compareListCopy[i], item))
                        {
                            compareListCopy.RemoveAt(i);
                            itemRemoved = true;
                            break;
                        }
                    }
                    if (!itemRemoved)
                        return false;
                }
                return true;
            }
            else
            {
                for (int i = 0; i < controlList.Count; i++)
                {
                    if (!Equals(controlList[i], compareList[i]))
                        return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Compares two lists for equality. Empty and <c>null</c> lists are considered identical.
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="controlList">A list to search</param>
        /// <param name="compareList">A list to compare</param>
        /// <param name="comparer">An optional equality comparer</param>
        /// <param name="optimize">
        /// Optimization options:
        /// <list type="bullet">
        /// <item>
        /// Distinct - use this option to shrink the source lists down to distinct values prior to searching.
        /// </item>
        /// </list>
        /// </param>
        /// <param name="ignoreOrder"><c>true</c> if not an order dependent comparison</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool IsIdentical<T>(List<T> controlList, List<T> compareList, IEqualityComparer<T> comparer, Optimization optimize = default, bool ignoreOrder = false)
        {
            if (controlList == null || !controlList.Any())
            {
                if (compareList == null || !compareList.Any())
                    return true;
                return false;
            }
            else if (compareList == null || !compareList.Any())
                return false;

            if ((optimize & Optimization.Distinct) == Optimization.Distinct)
            {
                var cache = new List<T>();  // looping in lieu of IEnumerable.Distinct() in an attempt to optimize
                for (int i = controlList.Count - 1; i >= 0; i--)
                {
                    if (cache.Contains(controlList[i], comparer))
                        controlList.RemoveAt(i);
                    else
                        cache.Add(controlList[i]);
                }
                cache.Clear();
                for (int i = compareList.Count - 1; i >= 0; i--)
                {
                    if (cache.Contains(compareList[i], comparer))
                        compareList.RemoveAt(i);
                    else
                        cache.Add(compareList[i]);
                }

                if (controlList.Count != compareList.Count)
                    throw new CollectionException("Distinct lists are not the same length");
            }
            else if (controlList.Count != compareList.Count)
                throw new CollectionException("Lists are not the same length");

            if (ignoreOrder)
            {
                var compareListCopy = CollectionUtil.ToList(compareList);
                bool itemRemoved;
                foreach (var item in controlList)
                {
                    itemRemoved = false;
                    for (var i = 0; i < compareListCopy.Count; i++)
                    {
                        if (comparer.Equals(compareListCopy[i], item))
                        {
                            compareListCopy.RemoveAt(i);
                            itemRemoved = true;
                            break;
                        }
                    }
                    if (!itemRemoved)
                        return false;
                }
                return true;
            }
            else
            {
                for (int i = 0; i < controlList.Count; i++)
                {
                    if (!comparer.Equals(controlList[i], compareList[i]))
                        return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Compares two <c>string</c> lists for equality. Empty and <c>null</c> lists are considered identical.
        /// </summary>
        /// <param name="controlList">A <c>string</c> list to search</param>
        /// <param name="compareList">A <c>string</c> list to compare</param>
        /// <param name="optimize">
        /// Optimization options:
        /// <list type="bullet">
        /// <item>
        /// Distinct - use this option to shrink the source lists down to distinct values prior to searching.
        /// </item>
        /// </list>
        /// </param>
        /// <param name="ignoreOrder"><c>true</c> if not an order dependent comparison</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool IsIdenticalIgnoreCase(List<string> controlList, List<string> compareList, Optimization optimize = default, bool ignoreOrder = false)
        {
            if (controlList == null || !controlList.Any())
            {
                if (compareList == null || !compareList.Any())
                    return true;
                return false;
            }
            else if (compareList == null || !compareList.Any())
                return false;

            if ((optimize & Optimization.Distinct) == Optimization.Distinct)
            {
                var cache = new List<string>();  // looping in lieu of IEnumerable.Distinct() in an attempt to optimize
                for (int i = controlList.Count - 1; i >= 0; i--)
                {
                    if (cache.Contains(controlList[i].ToUpper()))
                        controlList.RemoveAt(i);
                    else
                        cache.Add(controlList[i].ToUpper());
                }
                cache.Clear();
                for (int i = compareList.Count - 1; i >= 0; i--)
                {
                    if (cache.Contains(compareList[i].ToUpper()))
                        compareList.RemoveAt(i);
                    else
                        cache.Add(compareList[i].ToUpper());
                }

                if (controlList.Count != compareList.Count)
                    throw new CollectionException("Distinct lists are not the same length");
            }
            else if (controlList.Count != compareList.Count)
                throw new CollectionException("Lists are not the same length");

            if (ignoreOrder)
            {
                var compareListCopy = CollectionUtil.ToList(compareList);
                bool itemRemoved;
                foreach (var item in controlList)
                {
                    itemRemoved = false;
                    for (var i = 0; i < compareListCopy.Count; i++)
                    {
                        if (string.Equals(compareListCopy[i], item, StringComparison.OrdinalIgnoreCase))
                        {
                            compareListCopy.RemoveAt(i);
                            itemRemoved = true;
                            break;
                        }
                    }
                    if (!itemRemoved)
                        return false;
                }
                return true;
            }
            else
            {
                for (int i = 0; i < controlList.Count; i++)
                {
                    if (!string.Equals(controlList[i], compareList[i], StringComparison.OrdinalIgnoreCase))
                        return false;
                }
                return true;
            }
        }
    }
}
