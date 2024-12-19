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
        /// <inheritdoc cref="CollectionUtilAbstractions.ToList{T}(IEnumerable{T})"/>
        public static List<T> ToList<T>(IEnumerable<T> collection)
        {
            return CollectionUtilAbstractions.ToList(collection);
        }

        /// <inheritdoc cref="CollectionUtilAbstractions.AsList{T}(IEnumerable{T})"/>
        public static List<T> AsList<T>(IEnumerable<T> collection)
        {
            return CollectionUtilAbstractions.AsList(collection);
        }

        /// <inheritdoc cref="CollectionUtilAbstractions.ToArray{T}(IEnumerable{T})"/>
        public static T[] ToArray<T>(IEnumerable<T> collection)
        {
            return CollectionUtilAbstractions.ToArray(collection);
        }

        /// <inheritdoc cref="CollectionUtilAbstractions.AsArray{T}(IEnumerable{T})"/>
        public static T[] AsArray<T>(IEnumerable<T> collection)
        {
            return CollectionUtilAbstractions.AsArray(collection);
        }

        /// <summary>
        /// Picks out only the non-<c>null</c> items in a collection, which itself may be <c>null</c>.  
        /// Returns a non-<c>null</c> collection, possibly with 0 elements and/or a filter which has not yet been executed.
        /// <example>
        /// <para>Example</para>
        /// <code>
        /// IEnumerable&lt;string&gt; stringCollection = GetArbitraryStringCollection();
        /// List&lt;string&gt; prunedStrings = CollectionUtil.Prune(stringCollection).ToList();
        /// </code>
        /// </example>
        /// </summary>
        /// <typeparam name="T">Type of element</typeparam>
        /// <param name="collection">A collection</param>
        /// <returns>A non-<c>null</c> collection, possibly with 0 elements</returns>
        public static IEnumerable<T> Prune<T>(IEnumerable<T> collection)
        {
            if (collection == null)
                return Enumerable.Empty<T>();
            return collection
                .Where(a => a != null);
        }

        /// <summary>
        /// Picks out only the non-<c>null</c>, non-blank and non-whitespace <c>string</c>s in a collection, which itself may be <c>null</c>.  
        /// Returns a non-<c>null</c> collection, possibly with 0 elements and/or a filter which has not yet been executed.
        /// <example>
        /// <para>Example</para>
        /// <code>
        /// IEnumerable&lt;string&gt; stringCollection = GetArbitraryStringCollection();
        /// List&lt;string&gt; zappedStrings = CollectionUtil.Zap(stringCollection).ToList();
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="collection">A collection</param>
        /// <returns>A non-<c>null</c> collection, possibly with 0 elements</returns>
        public static IEnumerable<string> Zap(IEnumerable<string> collection)
        {
            if (collection == null)
                return Enumerable.Empty<string>();
            return collection
                .Where(s => Horseshoe.NET.Zap.String(s) != null);
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

        /// <inheritdoc cref="CollectionUtilAbstractions.Contains{T}(IEnumerable{T}, T, IEqualityComparer{T})"/>
        public static bool Contains<T>(IEnumerable<T> collection, T item, IEqualityComparer<T> comparer = null)
        {
            return CollectionUtilAbstractions.Contains(collection, item, comparer: comparer);
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
        /// Compares two collections for equivalency (same size and contents or just same contents, depending).
        /// Collections that are <c>null</c> or empty are considered equivalent to other <c>null</c> or empty collections.
        /// </summary>
        /// <param name="controlCollection">A collection to search</param>
        /// <param name="compareCollection">A collection to compare</param>
        /// <param name="ignoreOrder"><c>true</c> if not an order dependent comparison, default is <c>false</c></param>
        /// <param name="ignoreCollectionSize"><c>true</c> if matching content values not quantites, default is <c>false</c></param>
        /// <param name="referencialEquality"><c>true</c> if only interested in comparing items that are the same instance, default is <c>false</c></param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool IsEquivalent(IEnumerable<object> controlCollection, IEnumerable<object> compareCollection, bool ignoreOrder = false, bool ignoreCollectionSize = false, bool referencialEquality = false)
        {
            if (controlCollection == null || !controlCollection.Any())
            {
                if (compareCollection == null || !compareCollection.Any())
                    return true;
                return false;
            }
            else if (compareCollection == null || !compareCollection.Any())
                return false;

            int controlCollectionCount = controlCollection.Count();
            if (!ignoreCollectionSize && controlCollectionCount != compareCollection.Count())
                return false;

            if (!ignoreOrder && !ignoreCollectionSize)  // default behavior
            {
                for (int i = 0; i < controlCollectionCount; i++)
                {
                    if (referencialEquality)
                    {
                        if (referencialEquality && !ReferenceEquals(controlCollection.ElementAt(i), compareCollection.ElementAt(i)))
                            return false;
                    }
                    else if (!Equals(controlCollection.ElementAt(i), compareCollection.ElementAt(i)))
                        return false;
                }
                return true;
            }

            // if ignoring the order and/or collection size...
            foreach (object controlObj in controlCollection)
            {
                bool foundMatch = false;
                foreach (object compareObj in compareCollection)
                {
                    if (referencialEquality)
                    {
                        if (referencialEquality && ReferenceEquals(controlObj, compareObj))
                        {
                            foundMatch = true;
                            break;
                        }
                    }
                    else if (Equals(controlObj, compareObj))
                    {
                        foundMatch = true;
                        break;
                    }
                }
                if (!foundMatch)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Compares two collections for equivalency (same size and contents or just same contents, depending).
        /// Collections that are <c>null</c> or empty are considered equivalent to other <c>null</c> or empty collections.
        /// </summary>
        /// <param name="controlCollection">A collection to search</param>
        /// <param name="compareCollection">A collection to compare</param>
        /// <param name="ignoreCase"><c>true</c> if not a case-sensitive comparison (when <c>T = System.String</c>), default is <c>false</c></param>
        /// <param name="ignoreOrder"><c>true</c> if not an order dependent comparison, default is <c>false</c></param>
        /// <param name="ignoreCollectionSize"><c>true</c> if matching content values not quantites, default is <c>false</c></param>
        /// <param name="referencialEquality"><c>true</c> if only interested in comparing items that are the same instance, default is <c>false</c></param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool IsEquivalentEq<T>(IEnumerable<T> controlCollection, IEnumerable<T> compareCollection, bool ignoreCase = false, bool ignoreOrder = false, bool ignoreCollectionSize = false, bool referencialEquality = false) where T : IEquatable<T>
        {
            if (controlCollection == null || !controlCollection.Any())
            {
                if (compareCollection == null || !compareCollection.Any())
                    return true;
                return false;
            }
            else if (compareCollection == null || !compareCollection.Any())
                return false;

            int controlCollectionCount = controlCollection.Count();
            if (!ignoreCollectionSize && controlCollectionCount != compareCollection.Count())
                return false;

            if (!ignoreOrder && !ignoreCollectionSize)  // default behavior
            {
                for (int i = 0; i < controlCollectionCount; i++)
                {
                    if (referencialEquality)
                    {
                        if (referencialEquality && !ReferenceEquals(controlCollection.ElementAt(i), compareCollection.ElementAt(i)))
                            return false;
                    }
                    else if (!Equals(controlCollection.ElementAt(i), compareCollection.ElementAt(i)) && !(ignoreCase && typeof(T) == typeof(string) && controlCollection.ElementAt(i) is string controlString && compareCollection.ElementAt(i) is string compareString && string.Equals(controlString, compareString, StringComparison.CurrentCultureIgnoreCase)))
                        return false;
                }
                return true;
            }

            // if ignoring the order and/or collection size...
            foreach (T controlObj in controlCollection)
            {
                bool foundMatch = false;
                foreach (T compareObj in compareCollection)
                {
                    if (referencialEquality)
                    {
                        if (referencialEquality && ReferenceEquals(controlObj, compareObj))
                        {
                            foundMatch = true;
                            break;
                        }
                    }
                    else if (Equals(controlObj, compareObj) || (ignoreCase && typeof(T) == typeof(string) && controlObj is string controlString && compareObj is string compareString && string.Equals(controlString, compareString, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        foundMatch = true;
                        break;
                    }
                }
                if (!foundMatch)
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
        /// Dumps a collection to a single line of text with the specified separator optionally rendering only the
        /// first <c>n</c> items and terminating the result with a remaining count indicator, e.g. "14 more...".
        /// </summary>
        /// <typeparam name="T">The item type</typeparam>
        /// <param name="collection">A collection</param>
        /// <param name="separator">The text that seperates items in the rendered <c>string</c>, default is ", ".</param>
        /// <param name="n">
        /// The max number of items to render.  If <c>n &lt;= 0</c> all items are rendered.  
        /// If <c>n &lt; collection.Count()</c> the output will end with a remaining count indicator, e.g. "14 more...".
        /// </param>
        /// <param name="renderer">An optional way to render each item.  
        /// <para>
        /// For example...
        /// <code>
        /// var dates = new List&lt;DateTime&gt;{ DateTime.MinValue, DateTime.Today, DateTime.MaxValue };
        /// Console.WriteLine(dates.StringDump(renderer: (d) => d.ToString("yyyy-MM-dd")));
        /// // same as
        /// Console.WriteLine(string.Join(", ", dates.Select(d => d.ToString("yyyy-MM-dd"))));
        /// </code>
        /// </para>
        /// </param>
        /// <returns>A single-line <c>string</c> representation of a colleciton</returns>
        public static string StringDump<T>(IEnumerable<T> collection, string separator = ", ", int n = 0, Func<T, string> renderer = null)
        {
            if (collection == null)
                return "[null]";
            if (!collection.Any())
                return "[empty]";
            if (n > 0)
            {
                var partialList = collection.Take(n);
                var strb = new StringBuilder(string.Join(separator, partialList.Select(t => renderer != null ? renderer.Invoke(t) : t?.ToString())));
                if (collection.Count() > n)
                {
                    strb.Append(separator)
                        .Append(collection.Count() - n)
                        .Append(" more...");
                }
                return strb.ToString();
            }
            return string.Join(separator, collection.Select(t => renderer != null ? renderer.Invoke(t) : t?.ToString()));
        }
    }
}
