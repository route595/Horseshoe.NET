using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Horseshoe.NET.ObjectsTypesAndValues;

namespace Horseshoe.NET.Collections
{
    /// <summary>
    /// A collection of utility methods for arrays
    /// </summary>
    public static class ArrayUtil
    {

        /* * * * * * * * * * * * * * 
         *         GROUP 1
         * * * * * * * * * * * * * */

        /// <summary>
        /// Removes leading and trailing whitespace characters from each <c>string</c> in the array.
        /// Optimizations are as follows.
        /// <list type="number">
        /// <item>
        /// An index-based array scan is used in lieu of Linq-style collection transforms.
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="array">A <c>string</c> array</param>
        /// <returns>The array</returns>
        public static string[] TrimAll(string[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = array[i]?.Trim();
            }
            return array;
        }

        /// <summary>
        /// Removes leading and trailing whitespace characters from each <c>string</c> in <c>array</c> 
        /// converting blank values to <c>null</c>. 
        /// Additionally, depending on <c>pruneOptions</c>, some or all <c>null</c> values may be removed.
        /// Optimizations are as follows.
        /// <list type="number">
        /// <item>
        /// Index-based list scans (2 or 4 depending on <c>pruneOptions</c>) are used in lieu of Linq-style collection filtering.
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="array">A <c>string</c> array</param>
        /// <param name="pruneOptions">Pruning options for <c>null</c> values (e.g. <c>Leading</c>, <c>Trailing</c>, <c>All</c>..., default is <c>None</c>)</param>
        /// <returns>A copy of the array, with <c>null</c> values possibly pruned</returns>
        public static string[] Zap(string[] array, PruneOptions pruneOptions = default) =>
            ListUtil.Zap(array, pruneOptions: pruneOptions).ToArray();

        /// <summary>
        /// Removes some or all <c>null</c> values from <c>array</c>.
        /// Optimizations are as follows.
        /// <list type="number">
        /// <item>
        /// Index-based list scans (0 or 2 depending on <c>pruneOptions</c>) are used in lieu of Linq-style collection filtering.
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">Type of array</typeparam>
        /// <param name="array">An array</param>
        /// <param name="pruneOptions">Pruning options for <c>null</c> values (e.g. <c>Leading</c>, <c>Trailing</c>..., default is <c>All</c>)</param>
        /// <returns>The modified array</returns>
        public static T[] Prune<T>(T[] array, PruneOptions pruneOptions = PruneOptions.All) =>
            ListUtil.Prune(array, pruneOptions: pruneOptions).ToArray();

        /* * * * * * * * * * * * * * 
         *         GROUP 2
         * * * * * * * * * * * * * */

        /// <inheritdoc cref="ListUtil.Pad{T}(IEnumerable{T}, int, CollectionBoundary, T, bool)"/>
        public static T[] Pad<T>(T[] array, int targetSize, CollectionBoundary boundary = default, T padWith = default, bool cannotExceedTargetSize = false) =>
            ListUtil.Pad(array, targetSize, boundary: boundary, padWith: padWith, cannotExceedTargetSize: cannotExceedTargetSize).ToArray();

        /// <inheritdoc cref="ListUtil.Crop{T}(IEnumerable{T}, int, CollectionBoundary)"/>
        public static T[] Crop<T>(T[] array, int targetSize, CollectionBoundary boundary = default) =>
            ListUtil.Crop(array, targetSize, boundary: boundary).ToArray();

        /// <inheritdoc cref="ListUtil.Fit{T}(IEnumerable{T}, int, CollectionBoundary, CollectionBoundary, T)"/>
        public static T[] Fit<T>(T[] array, int targetSize, CollectionBoundary cropBoundary = default, CollectionBoundary padBoundary = default, T padWith = default) =>
            ListUtil.Fit(array, targetSize, cropBoundary: cropBoundary, padBoundary: padBoundary, padWith: padWith).ToArray();

        /* * * * * * * * * * * * * * 
         *         GROUP 3
         * * * * * * * * * * * * * */

        /// <summary>
        /// Appends zero or more items to an array. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If there are no items to append returns the original <c>T[]</c>.
        /// </item>
        /// <item>
        /// When making multiple modifications consider using <c>List&lt;T&gt;</c> and daisychaining for the best optimized experience, see example below. 
        /// </item>
        /// </list>
        /// <code>
        /// ListUtil.PrependIf(myArray, x)
        ///     .InsertIf(y > x, y)
        ///     .AppendIf(z > y, z)
        ///     .ToArray();
        /// </code>
        /// </summary>
        /// <typeparam name="T">List type</typeparam>
        /// <param name="array">The array to which <c>items</c> will be appended</param>
        /// <param name="items">Items to append</param>
        /// <returns>The modified <c>T[]</c></returns>
        public static T[] Append<T>(T[] array, IEnumerable<T> items)
        {
            if (items == null || !items.Any())
                return array ?? Array.Empty<T>();         // optimization 1
            return ListUtil.Append(array, items).ToArray();
        }

        /// <summary>
        /// Appends zero or more items to an array. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If there are no items to append returns the original <c>T[]</c>.
        /// </item>
        /// <item>
        /// When making multiple modifications consider using <c>List&lt;T&gt;</c> and daisychaining for the best optimized experience, see example below. 
        /// </item>
        /// </list>
        /// <code>
        /// ListUtil.PrependIf(myArray, x)
        ///     .InsertIf(y > x, y)
        ///     .AppendIf(z > y, z)
        ///     .ToArray();
        /// </code>
        /// </summary>
        /// <typeparam name="T">Array type</typeparam>
        /// <param name="array">The array to which <c>items</c> will be appended</param>
        /// <param name="items">Items to append</param>
        /// <returns>The modified <c>T[]</c></returns>
        public static T[] Append<T>(T[] array, params T[] items)
        {
            return Append(array, items as IEnumerable<T>);
        }

        /// <summary>
        /// Conditionally appends zero or more items to an array. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If <c>condition == false</c> or there are no items to append returns the original <c>T[]</c>.
        /// </item>
        /// <item>
        /// When making multiple modifications consider using <c>List&lt;T&gt;</c> and daisychaining for the best optimized experience, see example below. 
        /// </item>
        /// </list>
        /// <code>
        /// ListUtil.PrependIf(myArray, x)
        ///     .InsertIf(y > x, y)
        ///     .AppendIf(z > y, z)
        ///     .ToArray();
        /// </code>
        /// </summary>
        /// <typeparam name="T">Array type</typeparam>
        /// <param name="array">The array to which <c>items</c> will be appended</param>
        /// <param name="condition">Whether or not to append the item(s)</param>
        /// <param name="items">Items to append</param>
        /// <returns>The modified <c>T[]</c></returns>
        public static T[] AppendIf<T>(T[] array, bool condition, IEnumerable<T> items)
        {
            if (!condition)
                return array ?? Array.Empty<T>();  // optimization 1b
            return Append(array, items);
        }

        /// <summary>
        /// Conditionally appends zero or more items to an array. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If <c>condition == false</c> or there are no items to append returns the original <c>T[]</c>.
        /// </item>
        /// <item>
        /// When making multiple modifications consider using <c>List&lt;T&gt;</c> and daisychaining for the best optimized experience, see example below. 
        /// </item>
        /// </list>
        /// <code>
        /// ListUtil.PrependIf(myArray, x)
        ///     .InsertIf(y > x, y)
        ///     .AppendIf(z > y, z)
        ///     .ToArray();
        /// </code>
        /// </summary>
        /// <typeparam name="T">Array type</typeparam>
        /// <param name="array">The array to which <c>items</c> will be appended</param>
        /// <param name="condition">Whether or not to append the item(s)</param>
        /// <param name="items">Items to append</param>
        /// <returns>The modified <c>T[]</c></returns>
        public static T[] AppendIf<T>(T[] array, bool condition, params T[] items)
        {
            return AppendIf(array, condition, items as IEnumerable<T>);
        }

        /// <summary>
        /// Appends zero or more collections to an array. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If there are no items to append returns the original <c>T[]</c>.
        /// </item>
        /// <item>
        /// When making multiple modifications consider using <c>List&lt;T&gt;</c> and daisychaining for the best optimized experience, see example below. 
        /// </item>
        /// </list>
        /// <code>
        /// ListUtil.PrependIf(myArray, x)
        ///     .InsertIf(y > x, y)
        ///     .AppendIf(z > y, z)
        ///     .ToArray();
        /// </code>
        /// </summary>
        /// <typeparam name="T">Array type</typeparam>
        /// <param name="array">The array to which <c>collections</c> will be appended</param>
        /// <param name="collections">Collections to append</param>
        /// <returns>The modified <c>T[]</c></returns>
        public static T[] Append<T>(T[] array, IEnumerable<IEnumerable<T>> collections)
        {
            if (collections == null || !collections.Any())
                return array ?? Array.Empty<T>();            // optimization 1
            return ListUtil.Append(array, collections).ToArray();
        }

        /// <summary>
        /// Appends zero or more collections to an array. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If there are no items to append returns the original <c>T[]</c>.
        /// </item>
        /// <item>
        /// When making multiple modifications consider using <c>List&lt;T&gt;</c> and daisychaining for the best optimized experience, see example below. 
        /// </item>
        /// </list>
        /// <code>
        /// ListUtil.PrependIf(myArray, x)
        ///     .InsertIf(y > x, y)
        ///     .AppendIf(z > y, z)
        ///     .ToArray();
        /// </code>
        /// </summary>
        /// <typeparam name="T">Array type</typeparam>
        /// <param name="array">The array to which <c>collections</c> will be appended</param>
        /// <param name="collections">Collections to append</param>
        /// <returns>The modified <c>T[]</c></returns>
        public static T[] Append<T>(T[] array, params IEnumerable<T>[] collections)
        {
            return Append(array, collections as IEnumerable<IEnumerable<T>>);
        }

        /// <summary>
        /// Conditionally appends zero or more collections to an array. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If <c>condition == false</c> or there are no items to append returns the original <c>T[]</c>.
        /// </item>
        /// <item>
        /// When making multiple modifications consider using <c>List&lt;T&gt;</c> and daisychaining for the best optimized experience, see example below. 
        /// </item>
        /// </list>
        /// <code>
        /// ListUtil.PrependIf(myArray, x)
        ///     .InsertIf(y > x, y)
        ///     .AppendIf(z > y, z)
        ///     .ToArray();
        /// </code>
        /// </summary>
        /// <typeparam name="T">Array type</typeparam>
        /// <param name="array">The array to which <c>collections</c> will be appended</param>
        /// <param name="condition">Whether or not to append the collection(s)</param>
        /// <param name="collections">Collections to append</param>
        /// <returns>The modified <c>T[]</c></returns>
        public static T[] AppendIf<T>(T[] array, bool condition, IEnumerable<IEnumerable<T>> collections)
        {
            if (!condition)
                return array ?? Array.Empty<T>();      // optimization 1b
            return Append(array, collections);
        }

        /// <summary>
        /// Conditionally appends zero or more collections to an array. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If <c>condition == false</c> or there are no items to append returns the original <c>T[]</c>.
        /// </item>
        /// <item>
        /// When making multiple modifications consider using <c>List&lt;T&gt;</c> and daisychaining for the best optimized experience, see example below. 
        /// </item>
        /// </list>
        /// <code>
        /// ListUtil.PrependIf(myArray, x)
        ///     .InsertIf(y > x, y)
        ///     .AppendIf(z > y, z)
        ///     .ToArray();
        /// </code>
        /// </summary>
        /// <typeparam name="T">Array type</typeparam>
        /// <param name="array">The array to which <c>collections</c> will be appended</param>
        /// <param name="condition">Whether or not to append the collection(s)</param>
        /// <param name="collections">Collections to append</param>
        /// <returns>The modified <c>T[]</c></returns>
        public static T[] AppendIf<T>(T[] array, bool condition, params IEnumerable<T>[] collections)
        {
            return AppendIf(array, condition, collections as IEnumerable<IEnumerable<T>>);
        }

        /// <summary>
        /// Inserts zero or more items into an array. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If there are no items to insert returns the original <c>T[]</c>.
        /// </item>
        /// <item>
        /// When making multiple modifications consider using <c>List&lt;T&gt;</c> and daisychaining for the best optimized experience, see example below. 
        /// </item>
        /// </list>
        /// <code>
        /// ListUtil.PrependIf(myArray, x)
        ///     .InsertIf(y > x, y)
        ///     .AppendIf(z > y, z)
        ///     .ToArray();
        /// </code>
        /// </summary>
        /// <typeparam name="T">Array type</typeparam>
        /// <param name="array">The array into which <c>items</c> will be inserted</param>
        /// <param name="index">The position in the collection to insert the item(s)</param>
        /// <param name="items">Items to insert</param>
        /// <returns>The modified <c>T[]</c></returns>
        public static T[] Insert<T>(T[] array, int index, IEnumerable<T> items)
        {
            if (items == null || !items.Any())
                return array ?? Array.Empty<T>();               // optimization 1
            return ListUtil.Insert(array, index, items).ToArray();
        }

        /// <summary>
        /// Inserts zero or more items into an array. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If there are no items to insert returns the original <c>T[]</c>.
        /// </item>
        /// <item>
        /// When making multiple modifications consider using <c>List&lt;T&gt;</c> and daisychaining for the best optimized experience, see example below. 
        /// </item>
        /// </list>
        /// <code>
        /// ListUtil.PrependIf(myArray, x)
        ///     .InsertIf(y > x, y)
        ///     .AppendIf(z > y, z)
        ///     .ToArray();
        /// </code>
        /// </summary>
        /// <typeparam name="T">Array type</typeparam>
        /// <param name="array">The array into which <c>items</c> will be inserted</param>
        /// <param name="index">The position in the collection to insert the item(s)</param>
        /// <param name="items">Items to insert</param>
        /// <returns>The modified <c>T[]</c></returns>
        public static T[] Insert<T>(T[] array, int index, params T[] items)
        {
            return Insert(array, index, items as IEnumerable<T>);
        }

        /// <summary>
        /// Conditionally inserts zero or more items into an array. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If <c>condition == false</c> or there are no items to insert returns the original <c>T[]</c>.
        /// </item>
        /// <item>
        /// When making multiple modifications consider using <c>List&lt;T&gt;</c> and daisychaining for the best optimized experience, see example below. 
        /// </item>
        /// </list>
        /// <code>
        /// ListUtil.PrependIf(myArray, x)
        ///     .InsertIf(y > x, y)
        ///     .AppendIf(z > y, z)
        ///     .ToArray();
        /// </code>
        /// </summary>
        /// <typeparam name="T">Array type</typeparam>
        /// <param name="array">The array into which <c>items</c> will be inserted</param>
        /// <param name="condition">Whether or not to insert the item(s)</param>
        /// <param name="index">The position in the collection to insert the item(s)</param>
        /// <param name="items">Items to insert</param>
        /// <returns>The modified <c>T[]</c></returns>
        public static T[] InsertIf<T>(T[] array, bool condition, int index, IEnumerable<T> items)
        {
            if (!condition)
                return array ?? Array.Empty<T>();         // optimization 1b
            return Insert(array, index, items);
        }

        /// <summary>
        /// Conditionally inserts zero or more items into an array. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If <c>condition == false</c> or there are no items to insert returns the original <c>T[]</c>.
        /// </item>
        /// <item>
        /// When making multiple modifications consider using <c>List&lt;T&gt;</c> and daisychaining for the best optimized experience, see example below. 
        /// </item>
        /// </list>
        /// <code>
        /// ListUtil.PrependIf(myArray, x)
        ///     .InsertIf(y > x, y)
        ///     .AppendIf(z > y, z)
        ///     .ToArray();
        /// </code>
        /// </summary>
        /// <typeparam name="T">Array type</typeparam>
        /// <param name="array">The array into which <c>items</c> will be inserted</param>
        /// <param name="condition">Whether or not to insert the item(s)</param>
        /// <param name="index">The position in the collection to insert the item(s)</param>
        /// <param name="items">Items to insert</param>
        /// <returns>The modified <c>T[]</c></returns>
        public static T[] InsertIf<T>(T[] array, bool condition, int index, params T[] items)
        {
            return InsertIf(array, condition, index, items as IEnumerable<T>);
        }

        /// <summary>
        /// Inserts zero or more collections into an array. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If there are no items to insert returns the original <c>T[]</c>.
        /// </item>
        /// <item>
        /// When making multiple modifications consider using <c>List&lt;T&gt;</c> and daisychaining for the best optimized experience, see example below. 
        /// </item>
        /// </list>
        /// <code>
        /// ListUtil.PrependIf(myArray, x)
        ///     .InsertIf(y > x, y)
        ///     .AppendIf(z > y, z)
        ///     .ToArray();
        /// </code>
        /// </summary>
        /// <typeparam name="T">Array type</typeparam>
        /// <param name="array">The array into which <c>collections</c> will be inserted</param>
        /// <param name="index">The position in the collection to insert the collections(s)</param>
        /// <param name="collections">Collections to insert</param>
        /// <returns>The modified <c>T[]</c></returns>
        public static T[] Insert<T>(T[] array, int index, IEnumerable<IEnumerable<T>> collections)
        {
            if (collections == null || !collections.Any())
                return array ?? Array.Empty<T>();               // optimization 1
            return ListUtil.Insert(array, index, collections).ToArray();
        }

        /// <summary>
        /// Inserts zero or more collections into an array. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If there are no items to insert returns the original <c>T[]</c>.
        /// </item>
        /// <item>
        /// When making multiple modifications consider using <c>List&lt;T&gt;</c> and daisychaining for the best optimized experience, see example below. 
        /// </item>
        /// </list>
        /// <code>
        /// ListUtil.PrependIf(myArray, x)
        ///     .InsertIf(y > x, y)
        ///     .AppendIf(z > y, z)
        ///     .ToArray();
        /// </code>
        /// </summary>
        /// <typeparam name="T">Array type</typeparam>
        /// <param name="array">The array into which <c>collections</c> will be inserted</param>
        /// <param name="index">The position in the collection to insert the collections(s)</param>
        /// <param name="collections">Collections to insert</param>
        /// <returns>The modified <c>T[]</c></returns>
        public static T[] Insert<T>(T[] array, int index, params IEnumerable<T>[] collections)
        {
            return Insert(array, index, collections as IEnumerable<IEnumerable<T>>);
        }

        /// <summary>
        /// Conditionally inserts zero or more collections into an array. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If <c>condition == false</c> or there are no items to insert returns the original <c>T[]</c>.
        /// </item>
        /// <item>
        /// When making multiple modifications consider using <c>List&lt;T&gt;</c> and daisychaining for the best optimized experience, see example below. 
        /// </item>
        /// </list>
        /// <code>
        /// ListUtil.PrependIf(myArray, x)
        ///     .InsertIf(y > x, y)
        ///     .AppendIf(z > y, z)
        ///     .ToArray();
        /// </code>
        /// </summary>
        /// <typeparam name="T">Array type</typeparam>
        /// <param name="array">The array into which <c>collections</c> will be inserted</param>
        /// <param name="condition">Whether or not to insert the collection(s)</param>
        /// <param name="index">The position in the collection to insert the collections(s)</param>
        /// <param name="collections">Collections to insert</param>
        /// <returns>The modified <c>T[]</c></returns>
        public static T[] InsertIf<T>(T[] array, bool condition, int index, IEnumerable<IEnumerable<T>> collections)
        {
            if (!condition)
                return array ?? Array.Empty<T>();         // optimization 1b
            return Insert(array, index, collections);
        }

        /// <summary>
        /// Conditionally inserts zero or more collections into an array. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If <c>condition == false</c> or there are no items to insert returns the original <c>T[]</c>.
        /// </item>
        /// <item>
        /// When making multiple modifications consider using <c>List&lt;T&gt;</c> and daisychaining for the best optimized experience, see example below. 
        /// </item>
        /// </list>
        /// <code>
        /// ListUtil.PrependIf(myArray, x)
        ///     .InsertIf(y > x, y)
        ///     .AppendIf(z > y, z)
        ///     .ToArray();
        /// </code>
        /// </summary>
        /// <typeparam name="T">Array type</typeparam>
        /// <param name="array">The array into which <c>collections</c> will be inserted</param>
        /// <param name="condition">Whether or not to insert the collection(s)</param>
        /// <param name="index">The position in the collection to insert the collections(s)</param>
        /// <param name="collections">Collections to insert</param>
        /// <returns>The modified <c>T[]</c></returns>
        public static T[] InsertIf<T>(T[] array, bool condition, int index, params IEnumerable<T>[] collections)
        {
            return InsertIf(array, condition, index, collections as IEnumerable<IEnumerable<T>>);
        }

        /// <summary>
        /// Prepends zero or more items to an array. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If there are no items to prepend returns the original <c>T[]</c>.
        /// </item>
        /// <item>
        /// When making multiple modifications consider using <c>List&lt;T&gt;</c> and daisychaining for the best optimized experience, see example below. 
        /// </item>
        /// </list>
        /// <code>
        /// ListUtil.PrependIf(myArray, x)
        ///     .InsertIf(y > x, y)
        ///     .AppendIf(z > y, z)
        ///     .ToArray();
        /// </code>
        /// </summary>
        /// <typeparam name="T">Array type</typeparam>
        /// <param name="array">The array to which <c>items</c> will be prepended</param>
        /// <param name="items">Items to prepend</param>
        /// <returns>The modified <c>T[]</c></returns>
        public static T[] Prepend<T>(T[] array, IEnumerable<T> items)
        {
            return Insert(array, 0, items);
        }

        /// <summary>
        /// Prepends zero or more items to an array. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If there are no items to prepend returns the original <c>T[]</c>.
        /// </item>
        /// <item>
        /// When making multiple modifications consider using <c>List&lt;T&gt;</c> and daisychaining for the best optimized experience, see example below. 
        /// </item>
        /// </list>
        /// <code>
        /// ListUtil.PrependIf(myArray, x)
        ///     .InsertIf(y > x, y)
        ///     .AppendIf(z > y, z)
        ///     .ToArray();
        /// </code>
        /// </summary>
        /// <typeparam name="T">Array type</typeparam>
        /// <param name="array">The array to which <c>items</c> will be prepended</param>
        /// <param name="items">Items to prepend</param>
        /// <returns>The modified <c>T[]</c></returns>
        public static T[] Prepend<T>(T[] array, params T[] items)
        {
            return Insert(array, 0, items);
        }

        /// <summary>
        /// Conditionally prepends zero or more items to an array. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If <c>condition == false</c> or there are no items to prepend returns the original <c>T[]</c>.
        /// </item>
        /// <item>
        /// </item>
        /// <item>
        /// When making multiple modifications consider using <c>List&lt;T&gt;</c> and daisychaining for the best optimized experience, see example below. 
        /// </item>
        /// </list>
        /// <code>
        /// ListUtil.PrependIf(myArray, x)
        ///     .InsertIf(y > x, y)
        ///     .AppendIf(z > y, z)
        ///     .ToArray();
        /// </code>
        /// </summary>
        /// <typeparam name="T">Array type</typeparam>
        /// <param name="array">The array to which <c>items</c> will be prepended</param>
        /// <param name="condition">Whether or not to prepend the item(s)</param>
        /// <param name="items">Items to prepend</param>
        /// <returns>The modified <c>T[]</c></returns>
        public static T[] PrependIf<T>(T[] array, bool condition, IEnumerable<T> items)
        {
            return InsertIf(array, condition, 0, items);
        }

        /// <summary>
        /// Conditionally prepends zero or more items to an array. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If <c>condition == false</c> or there are no items to prepend returns the original <c>T[]</c>.
        /// </item>
        /// <item>
        /// </item>
        /// <item>
        /// When making multiple modifications consider using <c>List&lt;T&gt;</c> and daisychaining for the best optimized experience, see example below. 
        /// </item>
        /// </list>
        /// <code>
        /// ListUtil.PrependIf(myArray, x)
        ///     .InsertIf(y > x, y)
        ///     .AppendIf(z > y, z)
        ///     .ToArray();
        /// </code>
        /// </summary>
        /// <typeparam name="T">Array type</typeparam>
        /// <param name="array">The array to which <c>items</c> will be prepended</param>
        /// <param name="condition">Whether or not to prepend the item(s)</param>
        /// <param name="items">Items to prepend</param>
        /// <returns>The modified <c>T[]</c></returns>
        public static T[] PrependIf<T>(T[] array, bool condition, params T[] items)
        {
            return InsertIf(array, condition, 0, items);
        }

        /// <summary>
        /// Prepends zero or more collections to an array. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If there are no items to prepend returns the original <c>T[]</c>.
        /// </item>
        /// <item>
        /// When making multiple modifications consider using <c>List&lt;T&gt;</c> and daisychaining for the best optimized experience, see example below. 
        /// </item>
        /// </list>
        /// <code>
        /// ListUtil.PrependIf(myArray, x)
        ///     .InsertIf(y > x, y)
        ///     .AppendIf(z > y, z)
        ///     .ToArray();
        /// </code>
        /// </summary>
        /// <typeparam name="T">Array type</typeparam>
        /// <param name="array">The array to which <c>collections</c> will be prepend</param>
        /// <param name="collections">Collections to prepend</param>
        /// <returns>The modified <c>T[]</c></returns>
        public static T[] Prepend<T>(T[] array, IEnumerable<IEnumerable<T>> collections)
        {
            return Insert(array, 0, collections);
        }

        /// <summary>
        /// Prepends zero or more collections to an array. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If there are no items to prepend returns the original <c>T[]</c>.
        /// </item>
        /// <item>
        /// When making multiple modifications consider using <c>List&lt;T&gt;</c> and daisychaining for the best optimized experience, see example below. 
        /// </item>
        /// </list>
        /// <code>
        /// ListUtil.PrependIf(myArray, x)
        ///     .InsertIf(y > x, y)
        ///     .AppendIf(z > y, z)
        ///     .ToArray();
        /// </code>
        /// </summary>
        /// <typeparam name="T">Array type</typeparam>
        /// <param name="array">The array to which <c>collections</c> will be prepended</param>
        /// <param name="collections">Collections to prepend</param>
        /// <returns>The modified <c>T[]</c></returns>
        public static T[] Prepend<T>(T[] array, params IEnumerable<T>[] collections)
        {
            return Insert(array, 0, collections);
        }

        /// <summary>
        /// Conditionally prepends zero or more collections to an array. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If <c>condition == false</c> or there are no items to prepend returns the original <c>T[]</c>.
        /// </item>
        /// <item>
        /// When making multiple modifications consider using <c>List&lt;T&gt;</c> and daisychaining for the best optimized experience, see example below. 
        /// </item>
        /// </list>
        /// <code>
        /// ListUtil.PrependIf(myArray, x)
        ///     .InsertIf(y > x, y)
        ///     .AppendIf(z > y, z)
        ///     .ToArray();
        /// </code>
        /// </summary>
        /// <typeparam name="T">Array type</typeparam>
        /// <param name="array">The array to which <c>collections</c> will be prepended</param>
        /// <param name="condition">Whether or not to prepend the item(s)</param>
        /// <param name="collections">Collections to prepend</param>
        /// <returns>The modified <c>T[]</c></returns>
        public static T[] PrependIf<T>(T[] array, bool condition, IEnumerable<IEnumerable<T>> collections)
        {
            return InsertIf(array, condition, 0, collections);
        }

        /// <summary>
        /// Conditionally prepends zero or more collections to an array. 
        /// Please note the following optimizations.
        /// <list type="number">
        /// <item>
        /// If <c>condition == false</c> or there are no items to prepend returns the original <c>T[]</c>.
        /// </item>
        /// <item>
        /// When making multiple modifications consider using <c>List&lt;T&gt;</c> and daisychaining for the best optimized experience, see example below. 
        /// </item>
        /// </list>
        /// <code>
        /// ListUtil.PrependIf(myArray, x)
        ///     .InsertIf(y > x, y)
        ///     .AppendIf(z > y, z)
        ///     .ToArray();
        /// </code>
        /// </summary>
        /// <typeparam name="T">Array type</typeparam>
        /// <param name="array">The array to which <c>collections</c> will be prepended</param>
        /// <param name="condition">Whether or not to prepend the item(s)</param>
        /// <param name="collections">Collections to prepend</param>
        /// <returns>The modified <c>T[]</c></returns>
        public static T[] PrependIf<T>(T[] array, bool condition, params IEnumerable<T>[] collections)
        {
            return InsertIf(array, condition, 0, collections);
        }

        /* * * * * * * * * * * * * * 
         *         GROUP 4
         * * * * * * * * * * * * * */

        /// <summary>
        /// Combines zero or more collections into a single array
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="collections">Collections to combine</param>
        /// <returns>The combined array</returns>
        public static T[] Combine<T>(params IEnumerable<T>[] collections)
        {
            return new CollectionBuilder<T>()
                .Append(collections)
                .RenderToArray();
        }

        /// <summary>
        /// Combines zero or more collections into a single array of distinct values
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="collections">Collections to combine</param>
        /// <returns>The combined array</returns>
        public static T[] CombineDistinct<T>(params IEnumerable<T>[] collections)
        {
            return new CollectionBuilder<T>(distinctValues: true)
                .Append(collections)
                .RenderToArray();
        }

        /// <summary>
        /// Combines zero or more collections into a single array of distinct values
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="comparer">The compararer used to determine distinctness</param>
        /// <param name="collections">Collections to combine</param>
        /// <returns>The combined array</returns>
        public static T[] CombineDistinct<T>(IEqualityComparer<T> comparer, params IEnumerable<T>[] collections)
        {
            return new CollectionBuilder<T>(distinctValues: true, comparer: comparer)
                .Append(collections)
                .RenderToArray();
        }

        /// <summary>
        /// Replace each occurrance of <c>item</c> with <c>replacement</c>.
        /// A <c>null</c> source array results in an empty <c>T[]</c>.
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="array">An array</param>
        /// <param name="item">An item to replace</param>
        /// <param name="replacement">The replacement item</param>
        /// <returns>The modified array</returns>
        public static T[] ReplaceAll<T>(T[] array, T item, T replacement)
        {
            if (array == null)
                return Array.Empty<T>();
            for (int i = 0; i < array.Length; i++)
            {
                if (Equals(array[i], item))
                    array[i] = replacement;
            }
            return array;
        }

        /// <summary>
        /// Replace each occurrance of <c>item</c> with <c>replacement</c>
        /// A <c>null</c> source array results in an empty <c>T[]</c>.
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="array">An array</param>
        /// <param name="item">An item to replace</param>
        /// <param name="replacement">The replacement item</param>
        /// <param name="comparer">An equality comparer</param>
        /// <returns>The modified array</returns>
        public static T[] ReplaceAll<T>(T[] array, T item, T replacement, IEqualityComparer<T> comparer)
        {
            if (array == null)
                return Array.Empty<T>();
            for (int i = 0; i < array.Length; i++)
            {
                if (comparer.Equals(array[i], item))
                    array[i] = replacement;
            }
            return array;
        }

        /// <summary>
        /// Replace each occurrance of <c>item</c> with <c>replacement</c> using case-insensitive comparison.
        /// A <c>null</c> source array results in an empty <c>string[]</c>.
        /// </summary>
        /// <param name="array">A <c>string</c> array</param>
        /// <param name="item">A <c>string</c> to replace</param>
        /// <param name="replacement">The replacement <c>string</c></param>
        /// <returns>The modified array</returns>
        public static string[] ReplaceAllIgnoreCase(string[] array, string item, string replacement)
        {
            if (array == null)
                return Array.Empty<string>();
            for (int i = 0; i < array.Length; i++)
            {
                if (string.Equals(array[i], item, StringComparison.OrdinalIgnoreCase))
                    array[i] = replacement;
            }
            return array;
        }

        /// <summary>
        /// Removes elements from an array returning a second array with the removed elements
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="array">A collection</param>
        /// <param name="startIndex">Start index</param>
        /// <param name="length">Length</param>
        /// <returns></returns>
        public static T[] Scoop<T>(ref T[] array, int startIndex, int length = -1)
        {
            if (length == -1)
            {
                length = array.Length - startIndex;
            }
            var newArray = array
                .Skip(startIndex)
                .Take(length)
                .ToArray();
            array = array
                .Take(startIndex)
                .Concat(
                    array
                        .Skip(startIndex + length)
                        .Take(array.Length)
                )
                .ToArray();
            return newArray;
        }

        /// <summary>
        /// Removes elements from the end of an array returning a second array with the removed elements
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="array">A collection</param>
        /// <param name="length">Length</param>
        /// <returns></returns>
        public static T[] ScoopOffTheEnd<T>(ref T[] array, int length)
        {
            return Scoop(ref array, array.Length - length);
        }

        /// <summary>
        /// Compares two arrays for equality. Empty and <c>null</c> arrays are considered identical.
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="controlArray">An array to search</param>
        /// <param name="compareArray">An array to compare</param>
        /// <param name="optimize">
        /// Optimization options:
        /// <list type="bullet">
        /// <item>
        /// Distinct - use this option to shrink the source arrays down to distinct values prior to searching.
        /// </item>
        /// </list>
        /// </param>
        /// <param name="ignoreOrder"><c>true</c> if not an order dependent comparison</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool IsIdentical<T>(T[] controlArray, T[] compareArray, Optimization optimize = default, bool ignoreOrder = false)
        {
            if (controlArray == null || !controlArray.Any())
            {
                if (compareArray == null || !compareArray.Any())
                    return true;
                return false;
            }
            else if (compareArray == null || !compareArray.Any())
                return false;

            if ((optimize & Optimization.Distinct) == Optimization.Distinct)
            {
                controlArray = controlArray
                    .Distinct()
                    .ToArray();
                compareArray = compareArray
                    .Distinct()
                    .ToArray();

                if (controlArray.Length != compareArray.Length)
                    throw new CollectionException("Distinct lists are not the same length");
            }
            if (controlArray.Length != compareArray.Length)
                throw new CollectionException("Lists are not the same length");

            if (ignoreOrder)
            {
                var cache = CollectionUtil.ToList(compareArray);
                bool itemRemoved;
                foreach (var item in controlArray)
                {
                    itemRemoved = false;
                    for (var i = 0; i < cache.Count; i++)
                    {
                        if (Equals(cache[i], item))
                        {
                            cache.RemoveAt(i);
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
                for (int i = 0; i < controlArray.Length; i++)
                {
                    if (!Equals(controlArray[i], compareArray[i]))
                        return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Compares two arrays for equality. Empty and <c>null</c> arrays are considered identical.
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="controlArray">An array to search</param>
        /// <param name="compareArray">An array to compare</param>
        /// <param name="comparer">An optional equality comparer</param>
        /// <param name="optimize">
        /// Optimization options:
        /// <list type="bullet">
        /// <item>
        /// Distinct - use this option to shrink the source arrays down to distinct values prior to searching.
        /// </item>
        /// </list>
        /// </param>
        /// <param name="ignoreOrder"><c>true</c> if not an order dependent comparison</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool IsIdentical<T>(T[] controlArray, T[] compareArray, IEqualityComparer<T> comparer, Optimization optimize = default, bool ignoreOrder = false)
        {
            if (controlArray == null || !controlArray.Any())
            {
                if (compareArray == null || !compareArray.Any())
                    return true;
                return false;
            }
            else if (compareArray == null || !compareArray.Any())
                return false;

            if ((optimize & Optimization.Distinct) == Optimization.Distinct)
            {
                controlArray = controlArray
                    .Distinct(comparer)
                    .ToArray();
                compareArray = compareArray
                    .Distinct(comparer)
                    .ToArray();

                if (controlArray.Length != compareArray.Length)
                    throw new CollectionException("Distinct lists are not the same length");
            }
            if (controlArray.Length != compareArray.Length)
                throw new CollectionException("Lists are not the same length");

            if (ignoreOrder)
            {
                var cache = CollectionUtil.ToList(compareArray);
                bool itemRemoved;
                foreach (var item in controlArray)
                {
                    itemRemoved = false;
                    for (var i = 0; i < cache.Count; i++)
                    {
                        if (comparer.Equals(cache[i], item))
                        {
                            cache.RemoveAt(i);
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
                for (int i = 0; i < controlArray.Length; i++)
                {
                    if (!comparer.Equals(controlArray[i], compareArray[i]))
                        return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Compares two <c>string</c> arrays for equality. Empty and <c>null</c> arrays are considered identical.
        /// </summary>
        /// <param name="controlArray">A <c>string</c> array to search</param>
        /// <param name="compareArray">A <c>string</c> array to compare</param>
        /// <param name="optimize">
        /// Optimization options:
        /// <list type="bullet">
        /// <item>
        /// Distinct - use this option to shrink the source arrays down to distinct values prior to searching.
        /// </item>
        /// </list>
        /// </param>
        /// <param name="ignoreOrder"><c>true</c> if not an order dependent comparison</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool IsIdenticalIgnoreCase(string[] controlArray, string[] compareArray, Optimization optimize = default, bool ignoreOrder = false)
        {
            if (controlArray == null || !controlArray.Any())
            {
                if (compareArray == null || !compareArray.Any())
                    return true;
                return false;
            }
            else if (compareArray == null || !compareArray.Any())
                return false;

            if ((optimize & Optimization.Distinct) == Optimization.Distinct)
            {
                controlArray = controlArray
                    .Distinct()
                    .ToArray();
                compareArray = compareArray
                    .Distinct()
                    .ToArray();

                if (controlArray.Length != compareArray.Length)
                    throw new CollectionException("Distinct lists are not the same length");
            }
            if (controlArray.Length != compareArray.Length)
                throw new CollectionException("Lists are not the same length");

            if (ignoreOrder)
            {
                var cache = CollectionUtil.ToList(compareArray);
                bool itemRemoved;
                foreach (var item in controlArray)
                {
                    itemRemoved = false;
                    for (var i = 0; i < cache.Count; i++)
                    {
                        if (string.Equals(cache[i], item, StringComparison.OrdinalIgnoreCase))
                        {
                            cache.RemoveAt(i);
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
                for (int i = 0; i < controlArray.Length; i++)
                {
                    if (!string.Equals(controlArray[i], compareArray[i], StringComparison.OrdinalIgnoreCase))
                        return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Displays the object arrays in <c>string</c> format.
        /// </summary>
        /// <param name="objectArrays">A collection of <c>object[]</c>.</param>
        /// <param name="columnNames">Optional. The names of the corresponding columns.</param>
        /// <returns>A <c>string</c> representation of the collection.</returns>
        /// <exception cref="UtilityException"></exception>
        public static string StringDump(IEnumerable<object[]> objectArrays, string[] columnNames = null)
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
                    _width = ValueUtil.Display(array[i]).Length;
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
                    sb.Append(ValueUtil.Display(array[i]).PadRight(colWidths[i]));
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
