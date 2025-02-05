using System.Collections.Generic;
using System.Linq;
using System.Text;

using Horseshoe.NET.Text;

namespace Horseshoe.NET.Collections
{
    /// <summary>
    /// A collection of <c>Array</c> extension methods
    /// </summary>
    public static class ArrayExtensions
    {

        /* * * * * * * * * * * * * * 
         *         GROUP 1
         * * * * * * * * * * * * * */

        /// <inheritdoc cref="ArrayUtil.TrimAll(string[])"/>
        public static string[] TrimAll(this string[] array) =>
            ArrayUtil.TrimAll(array);

        /// <inheritdoc cref="ArrayUtil.Zap(string[], PruneOptions)"/>
        public static string[] Zap(this string[] array, PruneOptions pruneOptions = default)
        {
            return ArrayUtil.Zap(array, pruneOptions: pruneOptions).ToArray();
        }

        /// <inheritdoc cref="ArrayUtil.Prune{T}(T[], PruneOptions)"/>
        public static T[] Prune<T>(this T[] array, PruneOptions pruneOptions = default)
        {
            return ArrayUtil.Prune(array, pruneOptions: pruneOptions).ToArray();
        }

        /* * * * * * * * * * * * * * 
         *         GROUP 2
         * * * * * * * * * * * * * */

        /// <inheritdoc cref="ArrayUtil.Pad{T}(T[], int, CollectionBoundary, T, bool)"/>
        public static T[] Pad<T>(this T[] array, int targetSize, CollectionBoundary boundary = default, T padWith = default, bool cannotExceedTargetSize = false) =>
            ArrayUtil.Pad(array, targetSize, boundary: boundary, padWith: padWith, cannotExceedTargetSize: cannotExceedTargetSize);

        /// <inheritdoc cref="ArrayUtil.Crop{T}(T[], int, CollectionBoundary)"/>
        public static T[] Crop<T>(this T[] array, int targetSize, CollectionBoundary boundary = default) =>
            ArrayUtil.Crop(array, targetSize, boundary: boundary);

        /// <inheritdoc cref="ArrayUtil.Fit{T}(T[], int, CollectionBoundary, CollectionBoundary, T)"/>
        public static T[] Fit<T>(this T[] array, int targetSize, CollectionBoundary cropBoundary = default, CollectionBoundary padBoundary = default, T padWith = default) =>
            ArrayUtil.Fit(array, targetSize, cropBoundary: cropBoundary, padBoundary: padBoundary, padWith: padWith);

        /* * * * * * * * * * * * * * 
         *         GROUP 3
         * * * * * * * * * * * * * */

        /// <inheritdoc cref="ArrayUtil.Append{T}(T[], IEnumerable{T})"/>
        public static T[] Append<T>(this T[] array, IEnumerable<T> items) =>
            ArrayUtil.Append(array, items);

        /// <inheritdoc cref="ArrayUtil.Append{T}(T[], T[])"/>
        public static T[] Append<T>(this T[] array, params T[] items) =>
            ArrayUtil.Append(array, items);

        /// <inheritdoc cref="ArrayUtil.AppendIf{T}(T[], bool, IEnumerable{T})"/>
        public static T[] AppendIf<T>(this T[] array, bool condition, IEnumerable<T> items) =>
            ArrayUtil.AppendIf(array, condition, items);

        /// <inheritdoc cref="ArrayUtil.AppendIf{T}(T[], bool, T[])"/>
        public static T[] AppendIf<T>(this T[] array, bool condition, params T[] items) =>
            ArrayUtil.AppendIf(array, condition, items);

        /// <inheritdoc cref="ArrayUtil.Append{T}(T[], IEnumerable{IEnumerable{T}})"/>
        public static T[] Append<T>(this T[] array, IEnumerable<IEnumerable<T>> collections) =>
            ArrayUtil.Append(array, collections);

        /// <inheritdoc cref="ArrayUtil.Append{T}(T[], IEnumerable{T}[])"/>
        public static T[] Append<T>(this T[] array, params IEnumerable<T>[] collections) =>
            ArrayUtil.Append(array, collections);

        /// <inheritdoc cref="ArrayUtil.AppendIf{T}(T[], bool, IEnumerable{IEnumerable{T}})"/>
        public static T[] AppendIf<T>(this T[] array, bool condition, IEnumerable<IEnumerable<T>> collections) =>
            ArrayUtil.AppendIf(array, condition, collections);

        /// <inheritdoc cref="ArrayUtil.AppendIf{T}(T[], bool, IEnumerable{T}[])"/>
        public static T[] AppendIf<T>(this T[] array, bool condition, params IEnumerable<T>[] collections) =>
            ArrayUtil.AppendIf(array, condition, collections);

        /// <inheritdoc cref="ArrayUtil.Insert{T}(T[], int, IEnumerable{T})"/>
        public static T[] Insert<T>(this T[] array, int index, IEnumerable<T> items) =>
            ArrayUtil.Insert(array, index, items);

        /// <inheritdoc cref="ArrayUtil.Insert{T}(T[], int, T[])"/>
        public static T[] Insert<T>(this T[] array, int index, params T[] items) =>
            ArrayUtil.Insert(array, index, items);

        /// <inheritdoc cref="ArrayUtil.InsertIf{T}(T[], bool, int, IEnumerable{T})"/>
        public static T[] InsertIf<T>(this T[] array, bool condition, int index, IEnumerable<T> items) =>
            ArrayUtil.InsertIf(array, condition, index, items);

        /// <inheritdoc cref="ArrayUtil.InsertIf{T}(T[], bool, int, T[])"/>
        public static T[] InsertIf<T>(this T[] array, bool condition, int index, params T[] items) =>
            ArrayUtil.InsertIf(array, condition, index, items);

        /// <inheritdoc cref="ArrayUtil.Insert{T}(T[], int, IEnumerable{IEnumerable{T}})"/>
        public static T[] Insert<T>(this T[] array, int index, IEnumerable<IEnumerable<T>> collections) =>
            ArrayUtil.Insert(array, index, collections);

        /// <inheritdoc cref="ArrayUtil.Insert{T}(T[], int, IEnumerable{T}[])"/>
        public static T[] Insert<T>(this T[] array, int index, params IEnumerable<T>[] collections) =>
            ArrayUtil.Insert(array, index, collections);

        /// <inheritdoc cref="ArrayUtil.InsertIf{T}(T[], bool, int, IEnumerable{IEnumerable{T}})"/>
        public static T[] InsertIf<T>(this T[] array, bool condition, int index, IEnumerable<IEnumerable<T>> collections) =>
            ArrayUtil.InsertIf(array, condition, index, collections);

        /// <inheritdoc cref="ArrayUtil.InsertIf{T}(T[], bool, int, IEnumerable{T}[])"/>
        public static T[] InsertIf<T>(this T[] array, bool condition, int index, params IEnumerable<T>[] collections) =>
            ArrayUtil.InsertIf(array, condition, index, collections);

        /// <inheritdoc cref="ArrayUtil.Prepend{T}(T[], IEnumerable{T})"/>
        public static T[] Prepend<T>(this T[] array, IEnumerable<T> items) =>
            ArrayUtil.Prepend(array, items);

        /// <inheritdoc cref="ArrayUtil.Prepend{T}(T[], T[])"/>
        public static T[] Prepend<T>(this T[] array, params T[] items) =>
            ArrayUtil.Prepend(array, items);

        /// <inheritdoc cref="ArrayUtil.PrependIf{T}(T[], bool, IEnumerable{T})"/>
        public static T[] PrependIf<T>(this T[] array, bool condition, IEnumerable<T> items) =>
            ArrayUtil.PrependIf(array, condition, items);

        /// <inheritdoc cref="ArrayUtil.PrependIf{T}(T[], bool, T[])"/>
        public static T[] PrependIf<T>(this T[] array, bool condition, params T[] items) =>
            ArrayUtil.PrependIf(array, condition, items);

        /// <inheritdoc cref="ArrayUtil.Prepend{T}(T[], IEnumerable{IEnumerable{T}})"/>
        public static T[] Prepend<T>(this T[] array, IEnumerable<IEnumerable<T>> collections) =>
            ArrayUtil.Prepend(array, collections);

        /// <inheritdoc cref="ArrayUtil.Prepend{T}(T[], IEnumerable{T}[])"/>
        public static T[] Prepend<T>(this T[] array, params IEnumerable<T>[] collections) =>
            ArrayUtil.Prepend(array, collections);

        /// <inheritdoc cref="ArrayUtil.PrependIf{T}(T[], bool, IEnumerable{IEnumerable{T}})"/>
        public static T[] PrependIf<T>(this T[] array, bool condition, IEnumerable<IEnumerable<T>> collections) =>
            ArrayUtil.PrependIf(array, condition, collections);

        /// <inheritdoc cref="ArrayUtil.PrependIf{T}(T[], bool, IEnumerable{T}[])"/>
        public static T[] PrependIf<T>(this T[] array, bool condition, params IEnumerable<T>[] collections) =>
            ArrayUtil.PrependIf(array, condition, collections);

        ///// <summary>
        ///// Replace each occurrance of <c>item</c> with <c>replacement</c>
        ///// </summary>
        ///// <typeparam name="T">Type of item</typeparam>
        ///// <param name="array">An array</param>
        ///// <param name="item">An item to replace</param>
        ///// <param name="replacement">The replacement item</param>
        ///// <returns>The modified array</returns>
        //public static T[] ReplaceAll<T>(this T[] array, T item, T replacement) where T : IEquatable<T> =>
        //    ArrayUtil.ReplaceAll(array, item, replacement);

        ///// <summary>
        ///// Replace each occurrance of <c>item</c> with <c>replacement</c>
        ///// </summary>
        ///// <typeparam name="T">Type of item</typeparam>
        ///// <param name="array">An array</param>
        ///// <param name="item">An item to replace</param>
        ///// <param name="replacement">The replacement item</param>
        ///// <param name="comparer">An equality comparer</param>
        ///// <returns>The modified array</returns>
        //public static T[] ReplaceAll<T>(this T[] array, T item, T replacement, IEqualityComparer<T> comparer) where T : IEquatable<T> =>
        //    ArrayUtil.ReplaceAll(array, item, replacement, comparer);

        /// <summary>
        /// Renders a collection of arrays to a multiline string
        /// </summary>
        /// <param name="objectArrays">A collection of <c>object</c> arrays</param>
        /// <param name="separator">A separator</param>
        /// <param name="displayNullAs">Optional, set how <c>null</c> values should be displayed.</param>
        /// <returns></returns>
        public static string Render(this IEnumerable<object[]> objectArrays, string separator = ",", string displayNullAs = TextConstants.Null)
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
    }
}
