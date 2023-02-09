using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Horseshoe.NET.Text;

namespace Horseshoe.NET.Collections
{
    /// <summary>
    /// A set of utility methods for collections.
    /// </summary>
    public static class CollectionUtil
    {
        /// <summary>
        /// Creates a new <c>List&lt;T&gt;</c> from any collection.
        /// </summary>
        /// <typeparam name="T">A collection type.</typeparam>
        /// <param name="collection">A collection to copy into the list.</param>
        /// <returns>A new <c>List&lt;T&gt;</c></returns>
        public static List<T> ToList<T>(IEnumerable<T> collection)
        {
            return new List<T>(collection ?? Enumerable.Empty<T>());
        }

        /// <summary>
        /// Casts a collection as <c>List&lt;T&gt;</c> if such a cast is available, otherwise creates a new <c>List&lt;T&gt;</c> from the collection.
        /// </summary>
        /// <typeparam name="T">A collection type.</typeparam>
        /// <param name="collection">A collection to cast to a list.</param>
        /// <returns>A collection as a <c>List&lt;T&gt;</c></returns>
        public static List<T> AsList<T>(IEnumerable<T> collection)
        {
            return collection is List<T> list
                ? list
                : new List<T>(collection ?? Enumerable.Empty<T>());
        }

        /// <summary>
        /// Inflates a list to the desired target size by padding items at the indicated boundary
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="collection">A collection</param>
        /// <param name="targetSize">The desired size of the list after padding</param>
        /// <param name="boundary">End (default) or Start</param>
        /// <param name="padWith">Item to use for padding</param>
        /// <param name="cannotExceedTargetSize"><c>true</c> if an exception should be thrown for oversized lists</param>
        /// <param name="keepOriginalListDataSource"><c>true</c> will prevent internally creating a new <c>List&lt;T&gt;</c> if <c>collection</c> is already a <c>List&lt;T&gt;</c> instance - this is to improve performance</param>
        /// <returns>The resized collection</returns>
        /// <exception cref="ValidationException"></exception>
        public static IEnumerable<T> Pad<T>(IEnumerable<T> collection, int targetSize, CollectionBoundary boundary = default, T padWith = default, bool cannotExceedTargetSize = false, bool keepOriginalListDataSource = false)
        {
            if (targetSize < 0)
                throw new ValidationException("targetSize cannot be a negative number");
            if (collection.Count() > targetSize && cannotExceedTargetSize)
                throw new ValidationException("source collection already exceeds target size");

            var list = keepOriginalListDataSource
                ? AsList(collection)
                : ToList(collection);

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
        /// Shrinks a list to the desired target size by removing items at the indicated boundary
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="collection">A collection</param>
        /// <param name="targetSize">The desired size of the list after padding</param>
        /// <param name="boundary">End (default) or Start</param>
        /// <param name="keepOriginalListDataSource"><c>true</c> will prevent internally creating a new <c>List&lt;T&gt;</c> if <c>collection</c> is already a <c>List&lt;T&gt;</c> instance - this is to improve performance</param>
        /// <returns>The resized collection</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="ValidationException"></exception>
        public static IEnumerable<T> Crop<T>(IEnumerable<T> collection, int targetSize, CollectionBoundary boundary = default, bool keepOriginalListDataSource = false)
        {
            if (targetSize < 0)
                throw new ValidationException("targetSize cannot be a negative number");

            var list = keepOriginalListDataSource
                ? AsList(collection)
                : ToList(collection);

            while (list.Count > targetSize)
            {
                switch (boundary)
                {
                    case CollectionBoundary.Start:
                    default:
                        list.RemoveAt(0);
                        break;
                    case CollectionBoundary.End:
                        list.RemoveAt(targetSize);
                        break;
                }
            }
            return list;
        }

        /// <summary>
        /// Inflates or shrinks a list to the desired target size by padding or removing items at the indicated boundary
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="collection">A collection</param>
        /// <param name="targetSize">The desired size of the list after padding</param>
        /// <param name="cropBoundary">End (default) or Start</param>
        /// <param name="padBoundary">End (default) or Start</param>
        /// <param name="padWith">Item to use for padding</param>
        /// <param name="keepOriginalListDataSource"><c>true</c> will prevent internally creating a new <c>List&lt;T&gt;</c> if <c>collection</c> is already a <c>List&lt;T&gt;</c> instance - this is to improve performance</param>
        /// <returns>The resized collection</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="ValidationException"></exception>
        public static IEnumerable<T> Fit<T>(IEnumerable<T> collection, int targetSize, CollectionBoundary cropBoundary = default, CollectionBoundary padBoundary = default, T padWith = default, bool keepOriginalListDataSource = false)
        {
            if (collection.Count() > targetSize)
            {
                collection = Crop(collection, targetSize, boundary: cropBoundary, keepOriginalListDataSource: keepOriginalListDataSource);
            }
            else if (collection.Count() < targetSize)
            {
                collection = Pad(collection, targetSize, boundary: padBoundary, padWith: padWith, cannotExceedTargetSize: false, keepOriginalListDataSource: keepOriginalListDataSource);
            }
            return collection;
        }

        /// <summary>
        /// Combines zero or more collections
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="collections">Collections to combine</param>
        /// <returns>The combined collection</returns>
        public static IEnumerable<T> Combine<T>(params IEnumerable<T>[] collections)
        {
            var list = new List<T>();
            foreach (var collection in collections ?? Array.Empty<IEnumerable<T>>())
            {
                list.AddRange(collection ?? Enumerable.Empty<T>());
            }
            return list;
        }

        /// <summary>
        /// Combines zero or more collections into a single list of distinct values
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="collections">Collections to combine</param>
        /// <returns>The combined collection</returns>
        public static IEnumerable<T> CombineDistinct<T>(params IEnumerable<T>[] collections)
        {
            var list = new List<T>();
            foreach (var collection in collections ?? Array.Empty<IEnumerable<T>>())
            {
                list.AddRange(collection?.Distinct() ?? Enumerable.Empty<T>());
            }
            return list
                .Distinct()
                .ToList();
        }

        /// <summary>
        /// Combines zero or more collections into a single list of distinct values
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="comparer">The compararer used to determine distinctness</param>
        /// <param name="collections">Collections to combine</param>
        /// <returns>The combined collection</returns>
        public static IEnumerable<T> CombineDistinct<T>(IEqualityComparer<T> comparer, params IEnumerable<T>[] collections)
        {
            var list = new List<T>();
            foreach (var collection in collections ?? Array.Empty<IEnumerable<T>>())
            {
                list.AddRange(collection?.Distinct(comparer) ?? Enumerable.Empty<T>());
            }
            return list
                .Distinct(comparer)
                .ToList();
        }

        /// <summary>
        /// Appends zero or more items to a list
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="collection">A collection</param>
        /// <param name="items">Items to append</param>
        /// <returns>The appended collection</returns>
        public static IEnumerable<T> Append<T>(IEnumerable<T> collection, params T[] items)
        {
            return AppendIf(true, collection, items);
        }

        /// <summary>
        /// Conditionally appends zero or more items to a list
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="condition"><c>true</c> or <c>false</c></param>
        /// <param name="collection">A collection</param>
        /// <param name="items">Items to append</param>
        /// <returns>The appended collection</returns>
        public static IEnumerable<T> AppendIf<T>(bool condition, IEnumerable<T> collection, params T[] items)
        {
            var list = ToList(collection);
            if (condition)
            {
                foreach (T item in items ?? Array.Empty<T>())
                {
                    list.Add(item);
                }
            }
            return list;
        }

        /// <summary>
        /// Appends zero or more collections to a list
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="collection">A collection</param>
        /// <param name="collections">Collections to append</param>
        /// <returns>The appended collection</returns>
        public static IEnumerable<T> Append<T>(IEnumerable<T> collection, params IEnumerable<T>[] collections)
        {
            return AppendIf(true, collection, collections);
        }

        /// <summary>
        /// Conditionally appends zero or more collections to a list
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="condition"><c>true</c> or <c>false</c></param>
        /// <param name="collection">A collection</param>
        /// <param name="collections">Collections to append</param>
        /// <returns>The appended collection</returns>
        public static IEnumerable<T> AppendIf<T>(bool condition, IEnumerable<T> collection, params IEnumerable<T>[] collections)
        {
            var list = ToList(collection);
            if (condition)
            {
                foreach (var _collection in collections)
                {
                    list.AddRange(_collection ?? Enumerable.Empty<T>());
                }
            }
            return list;
        }

        /// <summary>
        /// Conditionally appends zero or more collections to a list
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="condition">A required function that returns <c>true</c> or <c>false</c></param>
        /// <param name="collection">A collection</param>
        /// <param name="collections">Collections to append</param>
        /// <returns>The appended collection</returns>
        public static IEnumerable<T> AppendIf<T>(Func<T, bool> condition, IEnumerable<T> collection, params IEnumerable<T>[] collections)
        {
            var list = ToList(collection);
            foreach (var _collection in collections ?? Enumerable.Empty<IEnumerable<T>>())
            {
                foreach (var item in _collection ?? Enumerable.Empty<T>())
                {
                    if (condition(item))
                    {
                        list.Add(item);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Appends zero or more items to a list
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="collection">A collection</param>
        /// <param name="items">Items to append</param>
        /// <returns>The appended collection</returns>
        public static IEnumerable<T> Append_KeepOrig<T>(IEnumerable<T> collection, params T[] items)
        {
            return AppendIf_KeepOrig(true, collection, items);
        }

        /// <summary>
        /// Conditionally appends zero or more items to a list
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="condition"><c>true</c> or <c>false</c></param>
        /// <param name="collection">A collection</param>
        /// <param name="items">Items to append</param>
        /// <returns>The appended collection</returns>
        public static IEnumerable<T> AppendIf_KeepOrig<T>(bool condition, IEnumerable<T> collection, params T[] items)
        {
            var list = AsList(collection);
            if (condition)
            {
                foreach (T item in items ?? Array.Empty<T>())
                {
                    list.Add(item);
                }
            }
            return list;
        }

        /// <summary>
        /// Conditionally appends zero or more items to a list
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="condition">A required function that returns <c>true</c> or <c>false</c></param>
        /// <param name="collection">A collection</param>
        /// <param name="items">Items to append</param>
        /// <returns>The appended collection</returns>
        public static IEnumerable<T> AppendIf_KeepOrig<T>(Func<T, bool> condition, IEnumerable<T> collection, params T[] items)
        {
            var list = AsList(collection);
            foreach (T item in items ?? Array.Empty<T>())
            {
                if (condition(item))
                {
                    list.Add(item);
                }
            }
            return list;
        }

        /// <summary>
        /// Appends zero or more collections to a list
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="collection">A collection</param>
        /// <param name="collections">Collections to append</param>
        /// <returns>The appended collection</returns>
        public static IEnumerable<T> Append_KeepOrig<T>(IEnumerable<T> collection, params IEnumerable<T>[] collections)
        {
            return AppendIf_KeepOrig(true, collection, collections);
        }

        /// <summary>
        /// Conditionally appends zero or more collections to a list
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="condition"><c>true</c> or <c>false</c></param>
        /// <param name="collection">A collection</param>
        /// <param name="collections">Collections to append</param>
        /// <returns>The appended collection</returns>
        public static IEnumerable<T> AppendIf_KeepOrig<T>(bool condition, IEnumerable<T> collection, params IEnumerable<T>[] collections)
        {
            var list = AsList(collection);
            if (condition)
            {
                foreach (var _collection in collections)
                {
                    list.AddRange(_collection ?? Enumerable.Empty<T>());
                }
            }
            return list;
        }

        /// <summary>
        /// Conditionally appends zero or more collections to a list
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="condition">A required function that returns <c>true</c> or <c>false</c></param>
        /// <param name="collection">A collection</param>
        /// <param name="collections">Collections to append</param>
        /// <returns>The appended collection</returns>
        public static IEnumerable<T> AppendIf_KeepOrig<T>(Func<T, bool> condition, IEnumerable<T> collection, params IEnumerable<T>[] collections)
        {
            var list = AsList(collection);
            foreach (var _collection in collections ?? Enumerable.Empty<IEnumerable<T>>())
            {
                foreach (var item in _collection ?? Enumerable.Empty<T>())
                {
                    if (condition(item))
                    {
                        list.Add(item);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Replace each occurrance of <c>item</c> with <c>replacement</c>
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="collection">A list</param>
        /// <param name="item">An item to replace</param>
        /// <param name="replacement">The replacement item</param>
        /// <param name="keepOriginalListDataSource"><c>true</c> will prevent internally creating a new <c>List&lt;T&gt;</c> if <c>collection</c> is already a <c>List&lt;T&gt;</c> instance - this is to improve performance</param>
        /// <returns>The modified collection</returns>
        public static IEnumerable<T> ReplaceAll<T>(IEnumerable<T> collection, T item, T replacement, bool keepOriginalListDataSource = false) where T : IEquatable<T>
        {
            var list = keepOriginalListDataSource
                ? AsList(collection)
                : ToList(collection);

            for (int i = 0; i < list.Count; i++)
            {
                if (Equals(list[i], item))
                {
                    list[i] = replacement;
                }
            }
            return list;
        }

        /// <summary>
        /// Replace each occurrance of <c>item</c> with <c>replacement</c>
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="collection">A list</param>
        /// <param name="item">An item to replace</param>
        /// <param name="replacement">The replacement item</param>
        /// <param name="comparer">An equality comparer</param>
        /// <param name="keepOriginalListDataSource"><c>true</c> will prevent internally creating a new <c>List&lt;T&gt;</c> if <c>collection</c> is already a <c>List&lt;T&gt;</c> instance - this is to improve performance</param>
        /// <returns>The modified collection</returns>
        public static IEnumerable<T> ReplaceAll<T>(IEnumerable<T> collection, T item, T replacement, IEqualityComparer<T> comparer, bool keepOriginalListDataSource = false) where T : IEquatable<T>
        {
            var list = keepOriginalListDataSource
                ? AsList(collection)
                : ToList(collection);

            for (int i = 0; i < list.Count; i++)
            {
                if (comparer != null)
                {
                    if (comparer.Equals(list[i], item))
                    {
                        list[i] = replacement;
                    }
                }
                else
                {
                    if (Equals(list[i], item))
                    {
                        list[i] = replacement;
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Tests a collection for contents - <c>collection</c>, if null, returns <c>false</c> and <c>items</c>, if omitted, returns <c>collection.Any()</c>.
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="collection">A collection</param>
        /// <param name="items">Items to search for (optional, returns <c>collection.Any()</c> if omitted)</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAny<T>(IEnumerable<T> collection, params T[] items) where T : IEquatable<T>
        {
            if (collection == null)
                return false;
            return items == null || items.Length == 0
                ? collection.Any()
                : collection.Any(t => items.Contains(t));
        }

        /// <summary>
        /// Tests a collection for contents - <c>collection</c>, if null, returns <c>false</c> and <c>items</c>, if omitted, returns <c>collection.Any()</c>.
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="collection">A collection</param>
        /// <param name="comparer">A comparer</param>
        /// <param name="items">Items to search for (optional, returns <c>collection.Any()</c> if omitted)</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAny<T>(IEnumerable<T> collection, IEqualityComparer<T> comparer, params T[] items) where T : IEquatable<T>
        {
            if (collection == null)
                return false;
            return items == null || items.Length == 0
                ? collection.Any()
                : collection.Any(t => items.Contains(t, comparer));
        }

        /// <summary>
        /// Tests a <c>string</c> collection for contents - <c>collection</c>, if null, returns <c>false</c> and <c>items</c>, if omitted, also returns <c>false</c>.
        /// </summary>
        /// <param name="collection">A collection of <c>string</c></param>
        /// <param name="items">Items to search for (optional, but returns <c>false</c> if omitted)</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAnyIgnoreCase(IEnumerable<string> collection, params string[] items)
        {
            if (collection == null || items == null)
                return false;
            foreach(string item in items.Distinct())
            {
                foreach(string _item in collection.Distinct())
                {
                    if (string.Equals(item, _item, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Tests a collection for contents - either <c>collection</c>, if omitted, returns <c>false</c>.
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="collection">A collection to search</param>
        /// <param name="items">Items to find</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAll<T>(IEnumerable<T> collection, params T[] items) where T : IEquatable<T>
        {
            return ContainsAll(collection, items as IEnumerable<T>);
        }

        /// <summary>
        /// Tests a collection for contents - either <c>collection</c>, if omitted, returns <c>false</c>.
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="controlCollection">A collection to search</param>
        /// <param name="compareCollection">A collection to find</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAll<T>(IEnumerable<T> controlCollection, IEnumerable<T> compareCollection) where T : IEquatable<T>
        {
            if (!ContainsAny(controlCollection) || !ContainsAny(compareCollection))
                return false;

            compareCollection = compareCollection
                .Distinct()
                .ToList();

            foreach (var controlItem in controlCollection.Distinct())
            {
                if (!compareCollection.Contains(controlItem))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Tests a collection for contents - either <c>collection</c>, if omitted, returns <c>false</c>.
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="controlCollection">A collection to search</param>
        /// <param name="compareCollection">A collection to find</param>
        /// <param name="comparer">An equality comparer</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAll<T>(IEnumerable<T> controlCollection, IEnumerable<T> compareCollection, IEqualityComparer<T> comparer) where T : IEquatable<T>
        {
            if (!ContainsAny(controlCollection) || !ContainsAny(compareCollection))
                return false;

            compareCollection = compareCollection
                .Distinct(comparer)
                .ToList();

            foreach (var controlItem in controlCollection.Distinct(comparer))
            {
                if (!compareCollection.Contains(controlItem, comparer))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Tests a <c>string</c> collection for contents - either <c>collection</c>, if omitted, returns <c>false</c>.
        /// </summary>
        /// <param name="controlCollection">A collection to search</param>
        /// <param name="compareCollection">A collection to find</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAllIgnoreCase(IEnumerable<string> controlCollection, IEnumerable<string> compareCollection)
        {
            if (!ContainsAny(controlCollection) || !ContainsAny(compareCollection))
                return false;

            compareCollection = compareCollection
                .Distinct()
                .ToList();
            bool found;

            foreach (var controlItem in controlCollection.Distinct())
            {
                found = false;
                foreach (var compareItem in compareCollection.Distinct())
                {
                    found = (controlItem == null && compareItem == null) || string.Equals(controlItem, compareItem, StringComparison.CurrentCultureIgnoreCase);
                    if (found) 
                        break;
                }
                if (!found)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Compares two collections for equality - a <c>null</c> and an empty <c>collection</c> are considered identical in this method
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="controlCollection">A collection to search</param>
        /// <param name="compareCollection">A collection to compare</param>
        /// <param name="ignoreOrder"><c>true</c> if not an order dependent comparison</param>
        /// <param name="compareDistinctValuesOnly"><c>true</c> if only considering distinct values</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool IsIdentical<T>(IEnumerable<T> controlCollection, IEnumerable<T> compareCollection, bool ignoreOrder = false, bool compareDistinctValuesOnly = false) where T : IEquatable<T>
        {
            try
            {
                _IsIdentical_Init(ref controlCollection, ref compareCollection, null, ignoreOrder, compareDistinctValuesOnly);

                if (ignoreOrder)
                {
                    foreach (var controlItem in controlCollection)
                    {
                        if (!compareCollection.Contains(controlItem))
                            return false;
                    }
                    if (!compareDistinctValuesOnly)
                    {
                        foreach (var compareItem in compareCollection)
                        {
                            if (!controlCollection.Contains(compareItem))
                                return false;
                        }
                    }
                }
                else
                {
                    var _controlCollection = AsList(controlCollection);
                    var _compareCollection = AsList(compareCollection);
                    for (int i = 0; i < _controlCollection.Count; i++)
                    {
                        if (!Equals(_controlCollection[i], _compareCollection[i]))
                            return false;
                    }
                }
                return true;
            }
            catch (CollectionUtilMessage msg)
            {
                return msg.IsIdentical_ReturnValue;
            }
        }

        /// <summary>
        /// Compares two collections for equality - a <c>null</c> and an empty <c>collection</c> are considered identical in this method
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="controlCollection">A collection to search</param>
        /// <param name="compareCollection">A collection to compare</param>
        /// <param name="comparer">An equality comparer</param>
        /// <param name="ignoreOrder"><c>true</c> if not an order dependent comparison</param>
        /// <param name="compareDistinctValuesOnly"><c>true</c> if only considering distinct values</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool IsIdentical<T>(IEnumerable<T> controlCollection, IEnumerable<T> compareCollection, IEqualityComparer<T> comparer, bool ignoreOrder = false, bool compareDistinctValuesOnly = false) where T : IEquatable<T>
        {
            try
            {
                _IsIdentical_Init(ref controlCollection, ref compareCollection, comparer, ignoreOrder, compareDistinctValuesOnly);

                if (ignoreOrder)
                {
                    foreach (var controlItem in controlCollection)
                    {
                        if (!compareCollection.Contains(controlItem, comparer))
                            return false;
                    }
                    if (!compareDistinctValuesOnly)
                    {
                        foreach (var compareItem in compareCollection)
                        {
                            if (!controlCollection.Contains(compareItem, comparer))
                                return false;
                        }
                    }
                }
                else
                {
                    var _controlCollection = AsList(controlCollection);
                    var _compareCollection = AsList(compareCollection);
                    for (int i = 0; i < _controlCollection.Count; i++)
                    {
                        if (comparer != null)
                        {
                            if (!comparer.Equals(_controlCollection[i], _compareCollection[i]))
                                return false;
                        }
                        else
                        {
                            if (!Equals(_controlCollection[i], _compareCollection[i]))
                                return false;
                        }
                    }
                }
                return true;
            }
            catch (CollectionUtilMessage msg)
            {
                return msg.IsIdentical_ReturnValue;
            }
        }

        /// <summary>
        /// Compares two <c>string</c> collections for equality - a <c>null</c> and an empty <c>collection</c> are considered identical in this method
        /// </summary>
        /// <param name="controlCollection">A collection to search</param>
        /// <param name="compareCollection">A collection to compare</param>
        /// <param name="ignoreOrder"><c>true</c> if not an order dependent comparison</param>
        /// <param name="compareDistinctValuesOnly"><c>true</c> if only considering distinct values</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool IsIdenticalIgnoreCase(IEnumerable<string> controlCollection, IEnumerable<string> compareCollection, bool ignoreOrder = false, bool compareDistinctValuesOnly = false)
        {
            try
            {
                _IsIdentical_Init(ref controlCollection, ref compareCollection, null, ignoreOrder, compareDistinctValuesOnly);

                if (ignoreOrder)
                {
                    bool found;
                    foreach (var controlItem in controlCollection)
                    {
                        found = false;
                        foreach (var compareItem in compareCollection)
                        {
                            found = (controlItem == null && compareItem == null) || string.Equals(controlItem, compareItem, StringComparison.CurrentCultureIgnoreCase);
                            if (found)
                                break;
                        }
                        if (!found)
                            return false;
                    }
                    if (!compareDistinctValuesOnly)
                    {
                        foreach (var compareItem in compareCollection)
                        {
                            found = false;
                            foreach (var controlItem in controlCollection)
                            {
                                found = (controlItem == null && compareItem == null) || string.Equals(controlItem, compareItem, StringComparison.CurrentCultureIgnoreCase);
                                if (found)
                                    break;
                            }
                            if (!found)
                                return false;
                        }
                    }
                }
                else
                {
                    var _controlCollection = AsList(controlCollection);
                    var _compareCollection = AsList(compareCollection);
                    for (int i = 0; i < _controlCollection.Count; i++)
                    {
                        if (!string.Equals(_controlCollection[i], _compareCollection[i], StringComparison.CurrentCultureIgnoreCase))
                            return false;
                    }
                }
                return true;
            }
            catch (CollectionUtilMessage msg)
            {
                return msg.IsIdentical_ReturnValue;
            }
        }

        private static void _IsIdentical_Init<T>(ref IEnumerable<T> controlCollection, ref IEnumerable<T> compareCollection, IEqualityComparer<T> comparer, bool ignoreOrder, bool compareDistinctValuesOnly) where T : IEquatable<T>
        {
            if (!ContainsAny(controlCollection))
                throw new CollectionUtilMessage { IsIdentical_ReturnValue = !ContainsAny(compareCollection) };
            else if (!ContainsAny(compareCollection))
                throw new CollectionUtilMessage { IsIdentical_ReturnValue = !ContainsAny(controlCollection) };

            var _controlCollection = compareDistinctValuesOnly
                ? controlCollection
                    .Distinct(comparer)
                    .ToList()
                : AsList(controlCollection);
            var _compareCollection = compareDistinctValuesOnly
                ? compareCollection
                    .Distinct(comparer)
                    .ToList()
                : AsList(compareCollection);

            if (_controlCollection.Count != _compareCollection.Count)
                throw new CollectionUtilMessage { IsIdentical_ReturnValue = false };

            controlCollection = _controlCollection;
            compareCollection = _compareCollection;
        }

        /// <summary>
        /// Displays the object arrays in <c>string</c> format.
        /// </summary>
        /// <param name="objectArrays">A collection of <c>object[]</c>.</param>
        /// <param name="columnNames">Optional. The names of the corresponding columns.</param>
        /// <returns>A <c>string</c> representation of the collection.</returns>
        /// <exception cref="UtilityException"></exception>
        public static string Dump(IEnumerable<object[]> objectArrays, string[] columnNames = null)
        {
            var sb = new StringBuilder();
            var colWidths = new int[objectArrays.Max(a => a?.Length ?? 0)];
            int _width;

            if (columnNames != null)
            {
                if (columnNames.Length > colWidths.Length)
                {
                    throw new UtilityException("The supplied columns exceed the width of the data: " + columnNames.Length + " / " + colWidths.Length);
                }

                // prep widths - column names
                for (int i = 0; i < columnNames.Length; i++)
                {
                    colWidths[i] = columnNames[i].Length;
                }
            }

            // prep widths - data values
            foreach (var array in objectArrays)
            {
                if (array == null)
                {
                    continue;
                }

                for (int i = 0; i < array.Length; i++)
                {
                    _width = TextUtil.DumpDatum(array[i]).Length;
                    if (_width > colWidths[i])
                    {
                        colWidths[i] = _width;
                    }
                }
            }

            if (columnNames != null)
            {
                // build column headers
                for (int i = 0; i < colWidths.Length; i++)
                {
                    if (i > 0)
                    {
                        sb.Append(" ");
                    }
                    sb.Append(columnNames[i].PadRight(colWidths[i]));
                }
                sb.AppendLine();

                // build separators
                for (int i = 0; i < colWidths.Length; i++)
                {
                    if (i > 0)
                    {
                        sb.Append(" ");
                    }
                    sb.Append("".PadRight(colWidths[i], '-'));
                }
                sb.AppendLine();
            }

            // build data rows
            foreach (var array in objectArrays)
            {
                if (array == null)
                {
                    sb.AppendLine();
                    continue;
                }

                for (int i = 0; i < array.Length; i++)
                {
                    if (i > 0)
                    {
                        sb.Append(" ");
                    }
                    sb.Append(TextUtil.DumpDatum(array[i]).PadRight(colWidths[i]));
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
