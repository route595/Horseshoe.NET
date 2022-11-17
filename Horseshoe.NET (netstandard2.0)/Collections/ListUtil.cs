using System;
using System.Collections.Generic;

namespace Horseshoe.NET.Collections
{
    /// <summary>
    /// A collection of <c>List</c> utility methods
    /// </summary>
    public static class ListUtil
    {
        private static List<T> AsList<T>(IEnumerable<T> array) =>
            CollectionUtil.AsList(array);

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
            var collection = CollectionUtil.Combine(collections);
            return AsList(collection);
        }

        /// <summary>
        /// Combines zero or more lists into a single list of distinct values
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="collections">Collections to combine</param>
        /// <returns>The combined list</returns>
        public static List<T> CombineDistinct<T>(params IEnumerable<T>[] collections)
        {
            var collection = CollectionUtil.CombineDistinct(collections);
            return AsList(collection);
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
            var collection = CollectionUtil.CombineDistinct(comparer, collections);
            return AsList(collection);
        }

        /// <summary>
        /// Appends zero or more items to a list
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="list">A list</param>
        /// <param name="items">Items to append</param>
        /// <returns>The appended list</returns>
        public static List<T> Append<T>(List<T> list, params T[] items)
        {
            var collection = CollectionUtil.Append(list, items);
            return AsList(collection);
        }

        /// <summary>
        /// Conditionally appends zero or more items to a list
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="condition"><c>true</c> or <c>false</c></param>
        /// <param name="list">A list</param>
        /// <param name="items">Items to append</param>
        /// <returns>The appended list</returns>
        public static List<T> AppendIf<T>(bool condition, List<T> list, params T[] items)
        {
            var collection = CollectionUtil.AppendIf(condition, list, items);
            return AsList(collection);
        }

        /// <summary>
        /// Conditionally appends zero or more items to a list
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="condition">A required function that returns <c>true</c> or <c>false</c></param>
        /// <param name="list">A list</param>
        /// <param name="items">Items to append</param>
        /// <returns>The appended list</returns>
        public static List<T> AppendIf<T>(Func<T, bool> condition, List<T> list, params T[] items)
        {
            var collection = CollectionUtil.AppendIf(condition, list, items);
            return AsList(collection);
        }

        /// <summary>
        /// Appends zero or more collections to a list
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="list">A list</param>
        /// <param name="collections">Collections to append</param>
        /// <returns>The appended list</returns>
        public static List<T> Append<T>(List<T> list, params IEnumerable<T>[] collections)
        {
            var collection = CollectionUtil.Append(list, collections);
            return AsList(collection);
        }

        /// <summary>
        /// Conditionally appends zero or more collections to a list
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="condition"><c>true</c> or <c>false</c></param>
        /// <param name="list">A list</param>
        /// <param name="collections">Collections to append</param>
        /// <returns>The appended list</returns>
        public static List<T> AppendIf<T>(bool condition, List<T> list, params IEnumerable<T>[] collections)
        {
            var collection = CollectionUtil.AppendIf(condition, list, collections);
            return AsList(collection);
        }

        /// <summary>
        /// Conditionally appends zero or more collections to a list
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="condition">A required function that returns <c>true</c> or <c>false</c></param>
        /// <param name="list">A list</param>
        /// <param name="collections">Collections to append</param>
        /// <returns>The appended list</returns>
        public static List<T> AppendIf<T>(Func<T, bool> condition, List<T> list, params IEnumerable<T>[] collections)
        {
            var collection = CollectionUtil.AppendIf(condition, list, collections);
            return AsList(collection);
        }

        /// <summary>
        /// Appends zero or more items to a list
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="list">A list</param>
        /// <param name="items">Items to append</param>
        /// <returns>The appended list</returns>
        public static List<T> Append_KeepOrig<T>(List<T> list, params T[] items)
        {
            var collection = CollectionUtil.Append_KeepOrig(list, items);
            return AsList(collection);
        }

        /// <summary>
        /// Conditionally appends zero or more items to a list
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="condition"><c>true</c> or <c>false</c></param>
        /// <param name="list">A list</param>
        /// <param name="items">Items to append</param>
        /// <returns>The appended list</returns>
        public static List<T> AppendIf_KeepOrig<T>(bool condition, List<T> list, params T[] items)
        {
            var collection = CollectionUtil.AppendIf_KeepOrig(condition, list, items);
            return AsList(collection);
        }

        /// <summary>
        /// Conditionally appends zero or more items to a list
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="condition">A required function that returns <c>true</c> or <c>false</c></param>
        /// <param name="list">A list</param>
        /// <param name="items">Items to append</param>
        /// <returns>The appended list</returns>
        public static List<T> AppendIf_KeepOrig<T>(Func<T, bool> condition, List<T> list, params T[] items)
        {
            var collection = CollectionUtil.AppendIf_KeepOrig(condition, list, items);
            return AsList(collection);
        }

        /// <summary>
        /// Appends zero or more collections to a list
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="list">A list</param>
        /// <param name="collections">Collections to append</param>
        /// <returns>The appended list</returns>
        public static List<T> Append_KeepOrig<T>(List<T> list, params IEnumerable<T>[] collections)
        {
            var collection = CollectionUtil.Append_KeepOrig(list, collections);
            return AsList(collection);
        }

        /// <summary>
        /// Conditionally appends zero or more collections to a list
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="condition"><c>true</c> or <c>false</c></param>
        /// <param name="list">A list</param>
        /// <param name="collections">Collections to append</param>
        /// <returns>The appended list</returns>
        public static List<T> AppendIf_KeepOrig<T>(bool condition, List<T> list, params IEnumerable<T>[] collections)
        {
            var collection = CollectionUtil.AppendIf_KeepOrig(condition, list, collections);
            return AsList(collection);
        }

        /// <summary>
        /// Conditionally appends zero or more collections to a list
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="condition">A required function that returns <c>true</c> or <c>false</c></param>
        /// <param name="list">A list</param>
        /// <param name="collections">Collections to append</param>
        /// <returns>The appended list</returns>
        public static List<T> AppendIf_KeepOrig<T>(Func<T, bool> condition, List<T> list, params IEnumerable<T>[] collections)
        {
            var collection = CollectionUtil.AppendIf_KeepOrig(condition, list, collections);
            return AsList(collection);
        }

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
    }
}
