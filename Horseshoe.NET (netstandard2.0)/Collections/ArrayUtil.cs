using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

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
            var collection = CollectionUtil.Combine(collections);
            return collection.ToArray();
        }

        /// <summary>
        /// Combines zero or more collections into a single array of distinct values
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="collections">Collections to combine</param>
        /// <returns>The combined array</returns>
        public static T[] CombineDistinct<T>(params IEnumerable<T>[] collections)
        {
            var collection = CollectionUtil.CombineDistinct(collections);
            return collection.ToArray();
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
            var collection = CollectionUtil.CombineDistinct(comparer, collections);
            return collection.ToArray();
        }

        /// <summary>
        /// Appends zero or more items to an array
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="array">An array</param>
        /// <param name="items">Items to append</param>
        /// <returns>The appended array</returns>
        public static T[] Append<T>(T[] array, params T[] items)
        {
            return Append<T>(array, items as IEnumerable<T>);
        }

        /// <summary>
        /// Appends zero or more items to an array
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="array">An array</param>
        /// <param name="items">Items to append</param>
        /// <returns>The appended array</returns>
        public static T[] Append<T>(T[] array, IEnumerable<T> items)  // array = ['apple', 'orange', 'banana']   items = { 'pear', 'grape' }
        {                                                             //            [0]      [1]       [2]                  [0]      [1]
            if (array == null)
            {
                if (items == null || !items.Any())
                    return new T[0];
                if (items is T[] _array)
                    return _array;
                return items.ToArray();
            }
            if (items == null || !items.Any())
                return array;
            var newArray = new T[array.Length + items.Count()];       // newArray = [ null,     null,     null,       null,     null]
            Array.Copy(array, newArray, array.Length);                // newArray = ['apple', 'orange', 'banana',     null,     null]
            if (items is T[] itemArray)                               //               [0]      [1]       [2]          [3]       [4]
            {
                for (int i = 0; i < itemArray.Length; i++) 
                {                                                     //                                              'pear'   'grape'
                    newArray[array.Length + i] = itemArray[i];        //                                             [3 + 0]   [3 + 1]     (array len = 3)
                }
            }
            else
            {
                var itemIndex = 0;
                foreach (var item in items)
                {                                                     //                                              'pear'   'grape'
                    newArray[array.Length + itemIndex++] = item;      //                                             [3 + 0]   [3 + 1]     (array len = 3)
                }
            }
            return newArray;
        }

        /// <summary>
        /// Conditionally appends zero or more items to an array
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="condition"><c>true</c> or <c>false</c></param>
        /// <param name="array">An array</param>
        /// <param name="items">Items to append</param>
        /// <returns>The appended array</returns>
        public static T[] AppendIf<T>(bool condition, T[] array, params T[] items)
        {
            if (!condition)
                return array;
            return Append(array, items);
        }

        /// <summary>
        /// Adds zero or more items to the beginning of an array
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="array">An array</param>
        /// <param name="items">Items to prepend</param>
        /// <returns>The appended array</returns>
        public static T[] Prepend<T>(T[] array, params T[] items)
        {
            return Prepend<T>(array, items as IEnumerable<T>);
        }

        /// <summary>
        /// Adds zero or more items to the beginning of an array
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="array">An array</param>
        /// <param name="items">Items to prepend</param>
        /// <returns>The appended array</returns>
        public static T[] Prepend<T>(T[] array, IEnumerable<T> items) // array = ['apple', 'orange', 'banana']   items = { 'pear', 'grape' }
        {                                                             //            [0]      [1]       [2]                  [0]      [1]
            if (array == null)
            {
                if (items == null || !items.Any())
                    return new T[0];
                if (items is T[] _array)
                    return _array;
                return items.ToArray();
            }
            if (items == null || !items.Any())
                return array;
            var itemCount = items.Count();
            var newArray = new T[array.Length + itemCount];           // newArray = [null,     null,    null,     null,     null  ]
            Array.Copy(array, 0, newArray, itemCount, array.Length);  // newArray = [null,     null,   'apple', 'orange', 'banana']
            if (items is T[] itemArray)                               //              [0]       [1]      [2]      [3]       [4]
            {
                for (int i = 0; i < itemArray.Length; i++)
                {                                                     //             'pear'   'grape'
                    newArray[i] = itemArray[i];                       //              [0]       [1]
                }
            }
            else
            {
                var itemIndex = 0;
                foreach (var item in items)
                {                                                     //             'pear'   'grape'
                    newArray[itemIndex++] = item;                     //              [0]       [1]
                }
            }
            return newArray;
        }

        /// <summary>
        /// Conditionally adds zero or more items to the beginning an array
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="condition"><c>true</c> or <c>false</c></param>
        /// <param name="array">An array</param>
        /// <param name="items">Items to prepend</param>
        /// <returns>The appended array</returns>
        public static T[] PrependIf<T>(bool condition, T[] array, params T[] items)
        {
            if (!condition)
                return array;
            return Prepend(array, items);
        }

        /// <summary>
        /// Appends zero or more collections to an array
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="array">An array</param>
        /// <param name="collections">Collections to append</param>
        /// <returns>The appended array</returns>
        public static T[] Append<T>(T[] array, params IEnumerable<T>[] collections)
        {
            var collection = CollectionUtil.Append(array, collections);
            return collection.ToArray();
        }

        /// <summary>
        /// Conditionally appends zero or more collections to an array
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="condition"><c>true</c> or <c>false</c></param>
        /// <param name="array">An array</param>
        /// <param name="collections">Collections to append</param>
        /// <returns>The appended array</returns>
        public static T[] AppendIf<T>(bool condition, T[] array, params IEnumerable<T>[] collections)
        {
            var collection = CollectionUtil.AppendIf(condition, array, collections);
            return collection.ToArray();
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
    }
}
