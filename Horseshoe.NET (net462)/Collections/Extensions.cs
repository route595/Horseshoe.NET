﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Horseshoe.NET.Iterator;
using Horseshoe.NET.Text.TextGrid;

namespace Horseshoe.NET.Collections
{
    /// <summary>
    /// A collection of extension methods for connection several types of <c>Collection</c> with <c>Horseshoe.NET</c> collection utilities
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Casts a collection as <c>List&lt;T&gt;</c> if such a cast is available, otherwise creates a new <c>List&lt;T&gt;</c> from the collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns>A collection as a <c>List&lt;T&gt;</c></returns>
        public static List<T> AsList<T>(this IEnumerable<T> collection) =>
            CollectionUtil.AsList(collection);

        #region IEnumerable methods

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
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="ValidationException"></exception>
        public static IEnumerable<T> Pad<T>(this IEnumerable<T> collection, int targetSize, CollectionBoundary boundary = default, T padWith = default, bool cannotExceedTargetSize = false, bool keepOriginalListDataSource = false) =>
            CollectionUtil.Pad(collection, targetSize, boundary: boundary, padWith: padWith, cannotExceedTargetSize: cannotExceedTargetSize, keepOriginalListDataSource: keepOriginalListDataSource);

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
        public static IEnumerable<T> Crop<T>(this IEnumerable<T> collection, int targetSize, CollectionBoundary boundary = default, bool keepOriginalListDataSource = false) =>
            CollectionUtil.Crop(collection, targetSize, boundary: boundary, keepOriginalListDataSource: keepOriginalListDataSource);

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
        public static IEnumerable<T> Fit<T>(this IEnumerable<T> collection, int targetSize, CollectionBoundary cropBoundary = default, CollectionBoundary padBoundary = default, T padWith = default, bool keepOriginalListDataSource = false) =>
            CollectionUtil.Fit(collection, targetSize, cropBoundary: cropBoundary, padBoundary: padBoundary, padWith: padWith, keepOriginalListDataSource: keepOriginalListDataSource);

        /// <summary>
        /// Appends zero or more items to a list
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="collection">A collection</param>
        /// <param name="items">Items to append</param>
        /// <returns>The appended collection</returns>
        public static IEnumerable<T> Append<T>(this IEnumerable<T> collection, params T[] items) =>
            CollectionUtil.Append(collection, items);

        /// <summary>
        /// Conditionally appends zero or more items to a list
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="collection">A collection</param>
        /// <param name="condition"><c>true</c> or <c>false</c></param>
        /// <param name="items">Items to append</param>
        /// <returns>The appended collection</returns>
        public static IEnumerable<T> AppendIf<T>(this IEnumerable<T> collection, bool condition, params T[] items) =>
            CollectionUtil.AppendIf(condition, collection, items);

        /// <summary>
        /// Conditionally appends zero or more items to a list
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="collection">A collection</param>
        /// <param name="condition">A required function that returns <c>true</c> or <c>false</c></param>
        /// <param name="items">Items to append</param>
        /// <returns>The appended collection</returns>
        public static IEnumerable<T> AppendIf<T>(this IEnumerable<T> collection, Func<T, bool> condition, params T[] items) =>
            CollectionUtil.AppendIf(condition, collection, items);

        /// <summary>
        /// Appends zero or more collections to a list
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="collection">A collection</param>
        /// <param name="collections">Collections to append</param>
        /// <returns>The appended collection</returns>
        public static IEnumerable<T> Append<T>(this IEnumerable<T> collection, params IEnumerable<T>[] collections) =>
            CollectionUtil.Append(collection, collections);

        /// <summary>
        /// Conditionally appends zero or more collections to a list
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="collection">A collection</param>
        /// <param name="condition"><c>true</c> or <c>false</c></param>
        /// <param name="collections">Collections to append</param>
        /// <returns>The appended collection</returns>
        public static IEnumerable<T> AppendIf<T>(this IEnumerable<T> collection, bool condition, params IEnumerable<T>[] collections) =>
            CollectionUtil.AppendIf(condition, collection, collections);

        /// <summary>
        /// Conditionally appends zero or more collections to a list
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="collection">A collection</param>
        /// <param name="condition">A required function that returns <c>true</c> or <c>false</c></param>
        /// <param name="collections">Collections to append</param>
        /// <returns>The appended collection</returns>
        public static IEnumerable<T> AppendIf<T>(this IEnumerable<T> collection, Func<T, bool> condition, params IEnumerable<T>[] collections) =>
            CollectionUtil.AppendIf(condition, collection, collections);

        /// <summary>
        /// Replace each occurrance of <c>item</c> with <c>replacement</c>
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="collection">A list</param>
        /// <param name="item">An item to replace</param>
        /// <param name="replacement">The replacement item</param>
        /// <returns>The modified collection</returns>
        public static IEnumerable<T> ReplaceAll<T>(this IEnumerable<T> collection, T item, T replacement) where T : IEquatable<T> =>
            CollectionUtil.ReplaceAll(collection, item, replacement);

        /// <summary>
        /// Replace each occurrance of <c>item</c> with <c>replacement</c>
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="collection">A list</param>
        /// <param name="item">An item to replace</param>
        /// <param name="replacement">The replacement item</param>
        /// <param name="comparer">An equality comparer</param>
        /// <returns>The modified collection</returns>
        public static IEnumerable<T> ReplaceAll<T>(this IEnumerable<T> collection, T item, T replacement, IEqualityComparer<T> comparer) where T : IEquatable<T> =>
            CollectionUtil.ReplaceAll(collection, item, replacement, comparer);

        /// <summary>
        /// Tests a collection for contents - <c>collection</c>, if null, returns <c>false</c> and <c>items</c>, if omitted, returns <c>collection.Any()</c>.
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="collection">A collection</param>
        /// <param name="items">Items to search for (optional, returns <c>collection.Any()</c> if omitted)</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAny<T>(this IEnumerable<T> collection, params T[] items) where T : IEquatable<T> =>
            CollectionUtil.ContainsAny(collection, items);

        /// <summary>
        /// Tests a collection for contents - <c>collection</c>, if null, returns <c>false</c> and <c>items</c>, if omitted, returns <c>collection.Any()</c>.
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="collection">A collection</param>
        /// <param name="comparer">A comparer</param>
        /// <param name="items">Items to search for (optional, returns <c>collection.Any()</c> if omitted)</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAny<T>(this IEnumerable<T> collection, IEqualityComparer<T> comparer, params T[] items) where T : IEquatable<T> =>
            CollectionUtil.ContainsAny(collection, comparer, items);

        /// <summary>
        /// Tests a <c>string</c> collection for contents - <c>collection</c>, if null, returns <c>false</c> and <c>items</c>, if omitted, also returns <c>false</c>.
        /// </summary>
        /// <param name="collection">A collection of <c>string</c></param>
        /// <param name="items">Items to search for (optional, but returns <c>false</c> if omitted)</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAnyIgnoreCase(this IEnumerable<string> collection, params string[] items) =>
            CollectionUtil.ContainsAnyIgnoreCase(collection, items);

        /// <summary>
        /// Tests a collection for contents - either <c>collection</c>, if omitted, returns <c>false</c>.
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="collection">A collection to search</param>
        /// <param name="items">Items to find</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAll<T>(this IEnumerable<T> collection, params T[] items) where T : IEquatable<T> =>
            CollectionUtil.ContainsAll(collection, items);

        /// <summary>
        /// Tests a collection for contents - either <c>collection</c>, if omitted, returns <c>false</c>.
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="controlCollection">A collection to search</param>
        /// <param name="compareCollection">A collection to find</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAll<T>(this IEnumerable<T> controlCollection, IEnumerable<T> compareCollection) where T : IEquatable<T> =>
            CollectionUtil.ContainsAll(controlCollection, compareCollection);

        /// <summary>
        /// Tests a collection for contents - either <c>collection</c>, if omitted, returns <c>false</c>.
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="controlCollection">A collection to search</param>
        /// <param name="compareCollection">A collection to find</param>
        /// <param name="comparer">An equality comparer</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAll<T>(this IEnumerable<T> controlCollection, IEnumerable<T> compareCollection, IEqualityComparer<T> comparer) where T : IEquatable<T> =>
            CollectionUtil.ContainsAll(controlCollection, compareCollection, comparer);

        /// <summary>
        /// Tests a <c>string</c> collection for contents - either <c>collection</c>, if omitted, returns <c>false</c>.
        /// </summary>
        /// <param name="controlCollection">A collection to search</param>
        /// <param name="compareCollection">A collection to find</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAllIgnoreCase(this IEnumerable<string> controlCollection, IEnumerable<string> compareCollection) =>
            CollectionUtil.ContainsAllIgnoreCase(controlCollection, compareCollection);

        /// <summary>
        /// Compares two collections for equality - a <c>null</c> and an empty <c>collection</c> are considered identical in this method
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="controlCollection">A collection to search</param>
        /// <param name="compareCollection">A collection to compare</param>
        /// <param name="ignoreOrder"><c>true</c> if not an order dependent comparison</param>
        /// <param name="compareDistinctValuesOnly"><c>true</c> if only considering distinct values</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool IsIdentical<T>(this IEnumerable<T> controlCollection, IEnumerable<T> compareCollection, bool ignoreOrder = false, bool compareDistinctValuesOnly = false) where T : IEquatable<T> =>
            CollectionUtil.IsIdentical(controlCollection, compareCollection, ignoreOrder: ignoreOrder, compareDistinctValuesOnly: compareDistinctValuesOnly);

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
        public static bool IsIdentical<T>(this IEnumerable<T> controlCollection, IEnumerable<T> compareCollection, IEqualityComparer<T> comparer, bool ignoreOrder = false, bool compareDistinctValuesOnly = false) where T : IEquatable<T> =>
            CollectionUtil.IsIdentical(controlCollection, compareCollection, comparer, ignoreOrder: ignoreOrder, compareDistinctValuesOnly: compareDistinctValuesOnly);

        /// <summary>
        /// Compares two <c>string</c> collections for equality - a <c>null</c> and an empty <c>collection</c> are considered identical in this method
        /// </summary>
        /// <param name="controlCollection">A collection to search</param>
        /// <param name="compareCollection">A collection to compare</param>
        /// <param name="ignoreOrder"><c>true</c> if not an order dependent comparison</param>
        /// <param name="compareDistinctValuesOnly"><c>true</c> if only considering distinct values</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool IsIdenticalIgnoreCase(this IEnumerable<string> controlCollection, IEnumerable<string> compareCollection, bool ignoreOrder = false, bool compareDistinctValuesOnly = false) =>
            CollectionUtil.IsIdenticalIgnoreCase(controlCollection, compareCollection, ignoreOrder: ignoreOrder, compareDistinctValuesOnly: compareDistinctValuesOnly);

        /// <summary>
        /// Locates the index of an item in the collection
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="collection">A collection</param>
        /// <param name="item">An item</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static int IndexOf<T>(this IEnumerable<T> collection, T item)
        {
            int index = -1;
            if (collection != null && collection.Contains(item))
            {
                collection.Iterate
                (
                    (e, iteration) =>
                    {
                        if (Equals(e, item))
                        {
                            index = iteration.Index;
                            iteration.Exit();
                        }
                    }
                );
            }
            return index;
        }

        /// <summary>
        /// Locates the last index of an item in the collection
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="collection">A collection</param>
        /// <param name="item">An item</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static int LastIndexOf<T>(this IEnumerable<T> collection, T item)
        {
            int index = -1;
            if (collection != null && collection.Contains(item))
            {
                collection.Iterate
                (
                    (e, m) =>
                    {
                        if (Equals(e, item))
                        {
                            index = m.Index;
                        }
                    }
                );
            }
            return index;
        }

        /// <summary>
        /// Checks if the supplied item is the first of the collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">A collection</param>
        /// <param name="item">An item</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool StartsWith<T>(this IEnumerable<T> collection, T item)
        {
            return IndexOf(collection, item) == 0;
        }

        /// <summary>
        /// Inspired by SQL, determines if an item is one of a supplied array of values
        /// </summary>
        /// <typeparam name="T">Type of collection</typeparam>
        /// <param name="obj">The item to locate</param>
        /// <param name="collection">The collection to search</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool In<T>(this T obj, params T[] collection)
        {
            return In(obj, collection as IEnumerable<T>);
        }

        /// <summary>
        /// Inspired by SQL, determines if an item is one of a supplied array of values
        /// </summary>
        /// <typeparam name="T">Type of collection</typeparam>
        /// <param name="obj">The item to locate</param>
        /// <param name="comparer">Optional, an equality comparer</param>
        /// <param name="collection">The collection to search</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool In<T>(this T obj, IEqualityComparer<T> comparer, params T[] collection)
        {
            return In(obj, collection, comparer: comparer);
        }

        /// <summary>
        /// Inspired by SQL, determines if an item is found in a collection
        /// </summary>
        /// <typeparam name="T">Type of collection</typeparam>
        /// <param name="obj">The item to locate</param>
        /// <param name="collection">The collection to search</param>
        /// <param name="comparer">Optional, an equality comparer</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool In<T>(this T obj, IEnumerable<T> collection, IEqualityComparer<T> comparer = null)
        {
            if (collection == null) return false;
            return comparer != null
                ? collection.Contains(obj, comparer)
                : collection.Contains(obj);
        }

        /// <summary>
        /// Inspired by SQL, determines if a string is one of a supplied array of values (not case-sensitive)
        /// </summary>
        /// <param name="text">The string to locate</param>
        /// <param name="collection">The string collection to search</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool InIgnoreCase(this string text, params string[] collection)
        {
            return InIgnoreCase(text, collection as IEnumerable<string>);
        }

        /// <summary>
        /// Inspired by SQL, determines if a string is found in a collection (not case-sensitive)
        /// </summary>
        /// <param name="text">The string to locate</param>
        /// <param name="collection">The string collection to search</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool InIgnoreCase(this string text, IEnumerable<string> collection)
        {
            if (collection == null) return false;
            foreach (var s in collection)
            {
                if (string.Equals(s, text, StringComparison.OrdinalIgnoreCase)) return true;
            }
            return false;
        }

        /// <summary>
        /// Renders a collection of arrays to a multiline string
        /// </summary>
        /// <param name="objectArrays">a collection of arrays</param>
        /// <param name="separator"></param>
        /// <param name="displayNullAs"></param>
        /// <returns></returns>
        public static string Render(this IEnumerable<object[]> objectArrays, string separator = ",", string displayNullAs = "[null]")
        {
            if (objectArrays == null)
                return "[null-collection]";
            if (!objectArrays.Any())
                return "[empty-collection]";
            var strb = new StringBuilder();
            foreach (var array in objectArrays)
            {
                if (array == null)
                {
                    strb.AppendLine("[null-array]");
                }
                else if (!array.Any())
                {
                    strb.AppendLine("[empty-array]");
                }
                else
                {
                    strb.AppendLine(string.Join(separator, array.Select(o => o?.ToString() ?? displayNullAs)));
                }
            }
            return strb.ToString();
        }

        #endregion

        #region List methods

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
        public static List<T> Pad<T>(this List<T> list, int targetSize, CollectionBoundary boundary = default, T padWith = default, bool cannotExceedTargetSize = false, bool keepOriginalListDataSource = false) =>
            ListUtil.Pad(list, targetSize, boundary: boundary, padWith: padWith, cannotExceedTargetSize: cannotExceedTargetSize, keepOriginalListDataSource: keepOriginalListDataSource);

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
        public static List<T> Crop<T>(this List<T> list, int targetSize, CollectionBoundary boundary = default, bool keepOriginalListDataSource = false) =>
            ListUtil.Crop(list, targetSize, boundary: boundary, keepOriginalListDataSource: keepOriginalListDataSource);

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
        public static List<T> Fit<T>(this List<T> list, int targetSize, CollectionBoundary cropBoundary = default, CollectionBoundary padBoundary = default, T padWith = default, bool keepOriginalListDataSource = false) =>
            ListUtil.Fit(list, targetSize, cropBoundary: cropBoundary, padBoundary: padBoundary, padWith: padWith, keepOriginalListDataSource: keepOriginalListDataSource);

        /// <summary>
        /// Appends zero or more items to a list
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="list">A list</param>
        /// <param name="items">Items to append</param>
        /// <returns>The appended list</returns>
        public static List<T> Append<T>(this List<T> list, params T[] items) =>
            ListUtil.Append_KeepOrig(list, items);

        /// <summary>
        /// Conditionally appends zero or more items to a list
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="condition"><c>true</c> or <c>false</c></param>
        /// <param name="list">A list</param>
        /// <param name="items">Items to append</param>
        /// <returns>The appended list</returns>
        public static List<T> AppendIf<T>(this List<T> list, bool condition, params T[] items) =>
            ListUtil.AppendIf_KeepOrig(condition, list, items);

        /// <summary>
        /// Conditionally appends zero or more items to a list
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="condition">A required function that returns <c>true</c> or <c>false</c></param>
        /// <param name="list">A list</param>
        /// <param name="items">Items to append</param>
        /// <returns>The appended list</returns>
        public static List<T> AppendIf<T>(this List<T> list, Func<T, bool> condition, params T[] items) =>
            ListUtil.AppendIf_KeepOrig(condition, list, items);

        /// <summary>
        /// Appends zero or more collections to a list
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="list">A list</param>
        /// <param name="collections">Collections to append</param>
        /// <returns>The appended list</returns>
        public static List<T> Append<T>(this List<T> list, params IEnumerable<T>[] collections) =>
            ListUtil.Append_KeepOrig(list, collections);

        /// <summary>
        /// Conditionally appends zero or more collections to a list
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="condition"><c>true</c> or <c>false</c></param>
        /// <param name="list">A list</param>
        /// <param name="collections">Collections to append</param>
        /// <returns>The appended list</returns>
        public static List<T> AppendIf<T>(this List<T> list, bool condition, params IEnumerable<T>[] collections) =>
            ListUtil.AppendIf_KeepOrig(condition, list, collections);

        /// <summary>
        /// Conditionally appends zero or more collections to a list
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="condition">A required function that returns <c>true</c> or <c>false</c></param>
        /// <param name="list">A list</param>
        /// <param name="collections">Collections to append</param>
        /// <returns>The appended list</returns>
        public static List<T> AppendIf<T>(this List<T> list, Func<T, bool> condition, params IEnumerable<T>[] collections) =>
            ListUtil.AppendIf_KeepOrig(condition, list, collections);

        /// <summary>
        /// Replace each occurrance of <c>item</c> with <c>replacement</c>
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="list">A list</param>
        /// <param name="item">An item to replace</param>
        /// <param name="replacement">The replacement item</param>
        /// <param name="keepOriginalListDataSource"><c>true</c> will prevent internally creating a new <c>List&lt;T&gt;</c> if <c>collection</c> is already a <c>List&lt;T&gt;</c> instance - this is to improve performance</param>
        /// <returns>The modified list</returns>
        public static List<T> ReplaceAll<T>(this List<T> list, T item, T replacement, bool keepOriginalListDataSource = false) where T : IEquatable<T> => 
            ListUtil.ReplaceAll(list, item, replacement, keepOriginalListDataSource: keepOriginalListDataSource);

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
        public static List<T> ReplaceAll<T>(this List<T> list, T item, T replacement, IEqualityComparer<T> comparer, bool keepOriginalListDataSource = false) where T : IEquatable<T> =>
            ListUtil.ReplaceAll(list, item, replacement, comparer, keepOriginalListDataSource: keepOriginalListDataSource);

        #endregion

        #region Array methods

        /// <summary>
        /// Inflates an array to the desired target size by padding items at the indicated boundary
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="array">An array</param>
        /// <param name="targetSize">The desired size of the list after padding</param>
        /// <param name="boundary">End (default) or Start</param>
        /// <param name="padWith">Item to use for padding</param>
        /// <param name="cannotExceedTargetSize"><c>true</c> if an exception should be thrown for oversized lists</param>
        /// <returns>The resized array</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="ValidationException"></exception>
        public static T[] Pad<T>(this T[] array, int targetSize, CollectionBoundary boundary = default, T padWith = default, bool cannotExceedTargetSize = false) =>
            ArrayUtil.Pad(array, targetSize, boundary: boundary, padWith: padWith, cannotExceedTargetSize: cannotExceedTargetSize);

        /// <summary>
        /// Shrinks an array to the desired target size by removing items at the indicated boundary
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="array">An array</param>
        /// <param name="targetSize">The desired size of the list after padding</param>
        /// <param name="boundary">End (default) or Start</param>
        /// <returns>The resized array</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="ValidationException"></exception>
        public static T[] Crop<T>(this T[] array, int targetSize, CollectionBoundary boundary = default) =>
            ArrayUtil.Crop(array, targetSize, boundary: boundary);

        /// <summary>
        /// Inflates or shrinks an array to the desired target size by padding or removing items at the indicated boundary
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="array">An array</param>
        /// <param name="targetSize">The desired size of the list after padding</param>
        /// <param name="cropBoundary">End (default) or Start</param>
        /// <param name="padBoundary">End (default) or Start</param>
        /// <param name="padWith">Item to use for padding</param>
        /// <returns>The resized array</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="ValidationException"></exception>
        public static T[] Fit<T>(this T[] array, int targetSize, CollectionBoundary cropBoundary = default, CollectionBoundary padBoundary = default, T padWith = default) =>
            ArrayUtil.Fit(array, targetSize, cropBoundary: cropBoundary, padBoundary: padBoundary, padWith: padWith);

        /// <summary>
        /// Appends zero or more items to an array
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="array">An array</param>
        /// <param name="items">Items to append</param>
        /// <returns>The appended array</returns>
        public static T[] Append<T>(this T[] array, params T[] items) =>
            ArrayUtil.Append(array, items);

        /// <summary>
        /// Conditionally appends zero or more items to an array
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="array">An array</param>
        /// <param name="condition"><c>true</c> or <c>false</c></param>
        /// <param name="items">Items to append</param>
        /// <returns>The appended array</returns>
        public static T[] AppendIf<T>(this T[] array, bool condition, params T[] items) =>
            ArrayUtil.AppendIf(condition, array, items);

        /// <summary>
        /// Adds zero or more items to the beginning an array
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="array">An array</param>
        /// <param name="items">Items to prepend</param>
        /// <returns>The appended array</returns>
        public static T[] Prepend<T>(this T[] array, params T[] items) =>
            ArrayUtil.Prepend(array, items);

        /// <summary>
        /// Conditionally adds zero or more items to the beginning of an array
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="array">An array</param>
        /// <param name="condition"><c>true</c> or <c>false</c></param>
        /// <param name="items">Items to prepend</param>
        /// <returns>The appended array</returns>
        public static T[] PrependIf<T>(this T[] array, bool condition, params T[] items) =>
            ArrayUtil.PrependIf(condition, array, items);

        /// <summary>
        /// Appends zero or more collections to an array
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="array">An array</param>
        /// <param name="collections">Collections to append</param>
        /// <returns>The appended array</returns>
        public static T[] Append<T>(this T[] array, params IEnumerable<T>[] collections) =>
            ArrayUtil.Append(array, collections);

        /// <summary>
        /// Conditionally appends zero or more collections to an array
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="array">An array</param>
        /// <param name="condition"><c>true</c> or <c>false</c></param>
        /// <param name="collections">Collections to append</param>
        /// <returns>The appended array</returns>
        public static T[] AppendIf<T>(this T[] array, bool condition, params IEnumerable<T>[] collections) =>
            ArrayUtil.AppendIf(condition, array, collections);

        /// <summary>
        /// Replace each occurrance of <c>item</c> with <c>replacement</c>
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="array">An array</param>
        /// <param name="item">An item to replace</param>
        /// <param name="replacement">The replacement item</param>
        /// <returns>The modified array</returns>
        public static T[] ReplaceAll<T>(this T[] array, T item, T replacement) where T : IEquatable<T> =>
            ArrayUtil.ReplaceAll(array, item, replacement);

        /// <summary>
        /// Replace each occurrance of <c>item</c> with <c>replacement</c>
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="array">An array</param>
        /// <param name="item">An item to replace</param>
        /// <param name="replacement">The replacement item</param>
        /// <param name="comparer">An equality comparer</param>
        /// <returns>The modified array</returns>
        public static T[] ReplaceAll<T>(this T[] array, T item, T replacement, IEqualityComparer<T> comparer) where T : IEquatable<T> =>
            ArrayUtil.ReplaceAll(array, item, replacement, comparer);

        /// <summary>
        /// Trims the items in a <c>string[]</c>
        /// </summary>
        /// <param name="array">A <c>string[]</c></param>
        /// <returns>The original <c>string[]</c> with its items trimmed</returns>
        public static string[] Trim(this string[] array)
        {
            if (array == null)
                return Array.Empty<string>();

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = array[i]?.Trim();
            }
            return array;
        }

        /// <summary>
        /// Zaps the items in a <c>string[]</c>
        /// </summary>
        /// <param name="array">A <c>string[]</c></param>
        /// <returns>The original <c>string[]</c> with its items zapped</returns>
        public static string[] Zap(this string[] array)
        {
            if (array == null)
                return Array.Empty<string>();

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = Horseshoe.NET.Zap.String(array[i]);
            }
            return array;
        }

        /// <summary>
        /// Zaps the items in a <c>string[]</c> and removes any <c>null</c>s
        /// </summary>
        /// <param name="array">A <c>string[]</c></param>
        /// <returns>The original <c>string[]</c> with trimmed items if zapping resulted in zero <c>null</c>s, otherwise a new, shorter <c>string[]</c> with trimmed items</returns>
        public static string[] ZapAndPrune(this string[] array)
        {
            array = Zap(array);
            if (array.Any(s => s == null))
            {
                array = array
                    .Where(s => s != null)
                    .ToArray();
            }
            return array;
        }

        #endregion

        #region Dictionary methods

        /// <summary>
        /// Removes and returns a value from a dictionary, like <c>Array.pop()</c> in JavaScript
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">A non-null dictionary</param>
        /// <param name="key">The key to search</param>
        /// <param name="item">The extracted item</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool Extract<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, out TValue item) =>
            DictionaryUtil.Extract(dictionary, key, out item);

        /// <summary>
        /// Combines multiple dictionaries into one, merges identical keys right-to-left (right-most replaces left-most)
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">A dictionary</param>
        /// <param name="dictionariesToAppend">Dictionaries to append</param>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> Append<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, params IDictionary<TKey, TValue>[] dictionariesToAppend) =>
            DictionaryUtil.Append(dictionary, dictionariesToAppend);

        /// <summary>
        /// Combines multiple dictionaries into one, merges identical keys right-to-left (right-most replaces left-most)
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">A dictionary</param>
        /// <param name="mergeFunc">The function that merges left/right values when identical keys are encountered</param>
        /// <param name="dictionariesToAppend">Dictionaries to append</param>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> Append<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Func<TValue, TValue, TValue> mergeFunc, params IDictionary<TKey, TValue>[] dictionariesToAppend) =>
            DictionaryUtil.Append(mergeFunc, dictionary, dictionariesToAppend);

        /// <summary>
        /// Appends zero or more dictionaries to a bas, merges identical keys left-to-right (left-most replaces right-most)
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">A dictionary</param>
        /// <param name="dictionariesToAppend">Dictionaries to append</param>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> AppendMergeLeftToRight<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, params IDictionary<TKey, TValue>[] dictionariesToAppend) =>
            DictionaryUtil.AppendMergeLeftToRight(dictionary, dictionariesToAppend);

        /// <summary>
        /// Displays a dictionary's contents as a string
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">A dictionary</param>
        /// <param name="equals">Equality operator</param>
        /// <param name="separator">Separator</param>
        /// <returns></returns>
        public static string StringDump<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, string equals = " = ", string separator = ", ")
        {
            return string.Join(separator, dictionary.Select(kvp => kvp.Key + equals + kvp.Value));
        }

        /// <summary>
        /// Displays a dictionary's contents as a grid
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">A dictionary</param>
        /// <param name="borderPolicy">The border policy</param>
        /// <returns></returns>
        public static string StringDumpToGrid<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, BorderPolicy borderPolicy = default)
        {
            var textGrid = new TextGrid { BorderPolicy = borderPolicy };
            var col1 = new Column<TKey> { Title = typeof(TKey).Name };
            var col2 = new Column<TValue> { Title = typeof(TValue).Name };
            textGrid.Columns.Add(col1);
            textGrid.Columns.Add(col2);
            foreach (var kvp in dictionary)
            {
                col1.Add(kvp.Key);
                col2.Add(kvp.Value);
            }
            return textGrid.Render();
        }

        /// <summary>
        /// Adds all supplied key/value pairs to a dictionary, with merging
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">A dictionary</param>
        /// <param name="keyValuePairs">The key/value pairs to add</param>
        /// <param name="mergeFunc">The function that merges left/right values when identical keys are encountered</param>
        public static void AddAll<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IDictionary<TKey, TValue> keyValuePairs, Func<TValue, TValue, TValue> mergeFunc = null)
        {
            if (keyValuePairs != null)
            {
                foreach (var kvp in keyValuePairs)
                {
                    if (dictionary.ContainsKey(kvp.Key))
                    {
                        //dictionary[kvp.Key] = kvp.Value;
                        dictionary[kvp.Key] = (mergeFunc ?? ((leftVal, rightVal) => rightVal)).Invoke(dictionary[kvp.Key], kvp.Value);
                    }
                    else
                    {
                        dictionary.Add(kvp.Key, kvp.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Adds a value to a dictionary only if the key does not already exist
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">A dictionary</param>
        /// <param name="key">A key</param>
        /// <param name="value">A value</param>
        public static void AddIfUnique<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, value);
            }
        }

        /// <summary>
        /// Adds a value to a dictionary, if the key already exist the previous value is replaced
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">A dictionary</param>
        /// <param name="key">A key</param>
        /// <param name="value">A value</param>
        public static void AddOrReplace<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
            }
            else
            {
                dictionary.Add(key, value);
            }
        }

        /// <summary>
        /// Creates an immutable version of a dictionary
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">A dictionary</param>
        /// <returns></returns>
        public static ImmutableDictionary<TKey, TValue> AsImmutable<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            return dictionary is ImmutableDictionary<TKey, TValue> immutableDictionary
                ? immutableDictionary
                : new ImmutableDictionary<TKey, TValue>(dictionary);
        }

        /// <summary>
        /// Trims the items in a <c>List&lt;string&gt;</c>
        /// </summary>
        /// <param name="list">A <c>List&lt;string&gt;</c></param>
        /// <returns>The original <c>List&lt;string&gt;</c> with its items trimmed</returns>
        public static List<string> Trim(this List<string> list)
        {
            if (list == null)
                return new List<string>();

            for (int i = 0; i < list.Count; i++)
            {
                list[i] = list[i]?.Trim();
            }
            return list;
        }

        /// <summary>
        /// Zaps the items in a <c>List&lt;string&gt;</c>
        /// </summary>
        /// <param name="list">A <c>List&lt;string&gt;</c></param>
        /// <returns>The original <c>List&lt;string&gt;</c> with its items zapped</returns>
        public static List<string> Zap(this List<string> list)
        {
            if (list == null)
                return new List<string>();

            for (int i = 0; i < list.Count; i++)
            {
                list[i] = Horseshoe.NET.Zap.String(list[i]);
            }
            return list;
        }

        /// <summary>
        /// Zaps the items in a <c>List&lt;string&gt;</c> and removes any <c>null</c>s
        /// </summary>
        /// <param name="list">A <c>List&lt;string&gt;</c></param>
        /// <returns>The original <c>List&lt;string&gt;</c> with trimmed items minus <c>null</c>s</returns>
        public static List<string> ZapAndPrune(this List<string> list)
        {
            list = Zap(list);
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i] == null)
                    list.RemoveAt(i);
            }
            return list;
        }

        /// <summary>
        /// Renders this entire <c>IDictionary</c> to text
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static string Dump<TKey, TValue>(this IDictionary<TKey, TValue> dict)
        {
            return dict == null 
                ? Environment.NewLine
                : TextGrid.FromDictionary(dict).Render();
        }

        #endregion
    }
}
