using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Horseshoe.NET.Text;

namespace Horseshoe.NET.Collections
{
    /// <summary>
    /// A collection of utility methods for arrays
    /// </summary>
    public static class ArrayUtil
    {
        /// <inheritdoc cref="CollectionUtil.AsArray{T}(IEnumerable{T})"/>
        public static T[] AsArray<T>(IEnumerable<T> collection) =>
            CollectionUtil.AsArray(collection);

        /// <inheritdoc cref="CollectionUtil.ToArray{T}(IEnumerable{T})"/>
        public static T[] ToArray<T>(IEnumerable<T> collection) =>
            CollectionUtil.ToArray(collection);

        /// <summary>
        /// Picks out only the non-<c>null</c> items in an array, which itself may be <c>null</c>.  
        /// Returns a non-<c>null</c> array, possibly with 0 elements.
        /// <example>
        /// <para>Example</para>
        /// <code>
        /// IEnumerable&lt;string&gt; stringCollection = GetArbitraryStringCollection();
        /// string[] prunedStrings = ArrayUtil.Prune(stringCollection);
        /// </code>
        /// </example>
        /// </summary>
        /// <typeparam name="T">Type of element</typeparam>
        /// <param name="collection">A collection, e.g. an array</param>
        /// <returns>A non-<c>null</c> array, possibly with 0 elements</returns>
        public static T[] Prune<T>(IEnumerable<T> collection)
        {
            return CollectionUtil.Prune<T>(collection).ToArray();
        }

        /// <summary>
        /// Picks out only the non-<c>null</c>, non-blank and non-whitespace <c>string</c>s in a collection, which itself may be <c>null</c>.  
        /// Returns a non-<c>null</c> array, possibly with 0 elements.
        /// <example>
        /// <para>Example</para>
        /// <code>
        /// IEnumerable&lt;string&gt; stringCollection = GetArbitraryStringCollection();
        /// string[] zappedStrings = ArrayUtil.Zap(stringCollection);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="collection">A collection</param>
        /// <returns>A non-<c>null</c> array, possibly with 0 elements</returns>
        public static string[] Zap(IEnumerable<string> collection)
        {
            return CollectionUtil.Zap(collection).ToArray();
        }

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
        public static T[] Pad<T>(T[] array, int targetSize, CollectionBoundary boundary = default, T padWith = default, bool cannotExceedTargetSize = false)
        {
            var collection = CollectionUtil.Pad<T>
            (
                array,
                targetSize,
                boundary: boundary,
                padWith: padWith,
                cannotExceedTargetSize: cannotExceedTargetSize
            );
            return collection.ToArray();
        }

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
        public static T[] Crop<T>(T[] array, int targetSize, CollectionBoundary boundary = default)
        {
            var collection = CollectionUtil.Crop<T>
            (
                array,
                targetSize,
                boundary: boundary
            );
            return collection.ToArray();
        }

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
        public static T[] Fit<T>(T[] array, int targetSize, CollectionBoundary cropBoundary = default, CollectionBoundary padBoundary = default, T padWith = default)
        {
            var collection = CollectionUtil.Fit<T>
            (
                array,
                targetSize,
                cropBoundary: cropBoundary,
                padBoundary: padBoundary,
                padWith: padWith
            );
            return collection.ToArray();
        }

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
                .ToArray();
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
                .ToArray();
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
                .ToArray();
        }

        /// <summary>
        /// Appends zero or more items to a collection, result is an array of <c>T</c>
        /// </summary>
        /// <typeparam name="T">Type of collection</typeparam>
        /// <param name="collection">A collection to which <c>items</c> will be appended</param>
        /// <param name="items">Items to append</param>
        /// <returns>An array of <c>T</c></returns>
        public static T[] Append<T>(IEnumerable<T> collection, params T[] items)
        {
            return ListUtil.Append(collection, items).ToArray();
        }

        /// <summary>
        /// Conditionally appends zero or more items to a collection, result is an array of <c>T</c>
        /// </summary>
        /// <typeparam name="T">Type of collection</typeparam>
        /// <param name="condition"><c>true</c> or <c>false</c></param>
        /// <param name="collection">A collection to which <c>items</c> will conditionally be appended</param>
        /// <param name="items">Items to append</param>
        /// <returns>An array of <c>T</c></returns>
        public static T[] AppendIf<T>(bool condition, IEnumerable<T> collection, params T[] items)
        {
            return ListUtil.AppendIf(condition, collection, items).ToArray();
        }

        /// <summary>
        /// Conditionally appends zero or more items to a collection, result is an array of <c>T</c>
        /// </summary>
        /// <typeparam name="T">Type of collection</typeparam>
        /// <param name="condition">A function that dictates whether an item gets appended</param>
        /// <param name="collection">A collection to which <c>items</c> will be conditionally appended</param>
        /// <param name="items">Items to conditionally append</param>
        /// <returns>An array of <c>T</c></returns>
        public static T[] AppendIf<T>(Func<T, bool> condition, IEnumerable<T> collection, params T[] items)
        {
            return ListUtil.AppendIf(condition, collection, items).ToArray();
        }

        /// <summary>
        /// Appends zero or more collections to a collection, result is an array of <c>T</c>
        /// </summary>
        /// <typeparam name="T">Type of collection</typeparam>
        /// <param name="collection">A collection to which <c>items</c> will be appended</param>
        /// <param name="collections">Collections to append</param>
        /// <returns>An array of <c>T</c></returns>
        public static T[] Append<T>(IEnumerable<T> collection, params IEnumerable<T>[] collections)
        {
            return ListUtil.Append(collection, collections).ToArray();
        }

        /// <summary>
        /// Conditionally appends zero or more collections to a collection, result is an array of <c>T</c>
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="condition"><c>true</c> or <c>false</c></param>
        /// <param name="collection">A collection to which <c>collections</c> will conditionally be appended</param>
        /// <param name="collections">Collections to conditionally append</param>
        /// <returns>An array of <c>T</c></returns>
        public static T[] AppendIf<T>(bool condition, IEnumerable<T> collection, params IEnumerable<T>[] collections)
        {
            return ListUtil.AppendIf(condition, collection, collections).ToArray();
        }

        /// <summary>
        /// Conditionally appends zero or more collections to a collection, result is an array of <c>T</c>
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="condition">A function that dictates whether an item gets appended</param>
        /// <param name="collection">A collection to which <c>collections</c> will be conditionally appended</param>
        /// <param name="collections">Collections to conditionaaly append</param>
        /// <returns>An array of <c>T</c></returns>
        public static T[] AppendIf<T>(Func<T, bool> condition, IEnumerable<T> collection, params IEnumerable<T>[] collections)
        {
            return ListUtil.AppendIf(condition, collection, collections).ToArray();
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
        public static T[] Insert<T>(IEnumerable<T> collection, int index, params T[] items)
        {
            return ListUtil.Insert(collection, index, items).ToArray();
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
        public static T[] InsertIf<T>(bool condition, IEnumerable<T> collection, int index, params T[] items)
        {
            return ListUtil.InsertIf(condition, collection, index, items).ToArray();
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
        public static T[] InsertIf<T>(Func<T, bool> condition, IEnumerable<T> collection, int index, params T[] items)
        {
            return ListUtil.InsertIf(condition, collection, index, items).ToArray();
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
        public static T[] Insert<T>(IEnumerable<T> collection, int index, params IEnumerable<T>[] collections)
        {
            return ListUtil.Insert(collection, index, collections).ToArray();
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
        public static T[] InsertIf<T>(bool condition, IEnumerable<T> collection, int index, params IEnumerable<T>[] collections)
        {
            return ListUtil.InsertIf(condition, collection, index, collections).ToArray();
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
        public static T[] InsertIf<T>(Func<T, bool> condition, IEnumerable<T> collection, int index, params IEnumerable<T>[] collections)
        {
            return ListUtil.InsertIf(condition, collection, index, collections).ToArray();
        }

        /// <summary>
        /// Replace each occurrance of <c>item</c> with <c>replacement</c>
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="array">An array</param>
        /// <param name="item">An item to replace</param>
        /// <param name="replacement">The replacement item</param>
        /// <returns>The modified array</returns>
        public static T[] ReplaceAll<T>(T[] array, T item, T replacement) where T : IEquatable<T>
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (Equals(array[i], item))
                {
                    array[i] = replacement;
                }
            }
            return array;
        }

        /// <summary>
        /// Replace each occurrance of <c>item</c> with <c>replacement</c>
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="array">An array</param>
        /// <param name="item">An item to replace</param>
        /// <param name="replacement">The replacement item</param>
        /// <param name="comparer">An equality comparer</param>
        /// <returns>The modified array</returns>
        public static T[] ReplaceAll<T>(T[] array, T item, T replacement, IEqualityComparer<T> comparer) where T : IEquatable<T>
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (comparer != null)
                {
                    if (comparer.Equals(array[i], item))
                    {
                        array[i] = replacement;
                    }
                }
                else
                {
                    if (Equals(array[i], item))
                    {
                        array[i] = replacement;
                    }
                }
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
