using System.Collections.Generic;
using System.Linq;

namespace Horseshoe.NET.Collections
{
    /// <summary>
    /// A collection of <c>Collection</c> extension methods
    /// </summary>
    public static class Extensions
    {

        /* * * * * * * * * * * * * * 
         *         GROUP 1
         * * * * * * * * * * * * * */

        /// <inheritdoc cref="CollectionUtil.TrimAll(IEnumerable{string})"/>
        public static IEnumerable<string> TrimAll(this IEnumerable<string> collection) =>
            CollectionUtil.TrimAll(collection);

        /// <inheritdoc cref="CollectionUtil.Zap(IEnumerable{string}, PruneOptions)"/>
        public static IEnumerable<string> Zap(this IEnumerable<string> collection, PruneOptions pruneOptions = default) =>
            CollectionUtil.Zap(collection, pruneOptions: pruneOptions);

        /// <inheritdoc cref="CollectionUtil.Prune{T}(IEnumerable{T}, PruneOptions)"/>
        public static IEnumerable<T> Prune<T>(this IEnumerable<T> collection, PruneOptions pruneOptions = default) =>
            CollectionUtil.Prune(collection, pruneOptions: pruneOptions);

        /* * * * * * * * * * * * * * 
         *         GROUP 2
         * * * * * * * * * * * * * */

        /// <inheritdoc cref="CollectionUtil.Pad{T}(IEnumerable{T}, int, CollectionBoundary, T, bool)"/>
        public static IEnumerable<T> Pad<T>(this IEnumerable<T> collection, int targetSize, CollectionBoundary boundary = default, T padWith = default, bool cannotExceedTargetSize = false) =>
            CollectionUtil.Pad(collection, targetSize, boundary: boundary, padWith: padWith, cannotExceedTargetSize: cannotExceedTargetSize);

        /// <inheritdoc cref="CollectionUtil.Crop{T}(IEnumerable{T}, int, CollectionBoundary)"/>
        public static IEnumerable<T> Crop<T>(this IEnumerable<T> collection, int targetSize, CollectionBoundary boundary = default) =>
            CollectionUtil.Crop(collection, targetSize, boundary: boundary);

        /// <inheritdoc cref="CollectionUtil.Fit{T}(IEnumerable{T}, int, CollectionBoundary, CollectionBoundary, T)"/>
        public static IEnumerable<T> Fit<T>(this IEnumerable<T> collection, int targetSize, CollectionBoundary cropBoundary = default, CollectionBoundary padBoundary = default, T padWith = default) =>
            CollectionUtil.Fit(collection, targetSize, cropBoundary: cropBoundary, padBoundary: padBoundary, padWith: padWith);

        /* * * * * * * * * * * * * * 
         *         GROUP 3
         * * * * * * * * * * * * * */

        // Append(), Insert(), Prepend() limited to list and array

        /* * * * * * * * * * * * * * 
         *         GROUP 4
         * * * * * * * * * * * * * */

        /// <inheritdoc cref="CollectionUtil.ReplaceAll{T}(IEnumerable{T}, T, T)"/>
        public static IEnumerable<T> ReplaceAll<T>(this IEnumerable<T> collection, T item, T replacement) =>
            CollectionUtil.ReplaceAll(collection, item, replacement);

        /// <inheritdoc cref="CollectionUtil.ReplaceAll{T}(IEnumerable{T}, T, T, IEqualityComparer{T})"/>
        public static IEnumerable<T> ReplaceAll<T>(this IEnumerable<T> collection, T item, T replacement, IEqualityComparer<T> comparer) =>
            CollectionUtil.ReplaceAll(collection, item, replacement, comparer);

        /// <inheritdoc cref="CollectionUtil.IsIdentical{T}(IEnumerable{T}, IEnumerable{T}, Optimization, bool)"/>
        public static bool IsIdentical<T>(this IEnumerable<T> controlCollection, IEnumerable<T> compareCollection, Optimization optimize = default, bool ignoreOrder = false) =>
            CollectionUtil.IsIdentical(controlCollection, compareCollection, optimize: optimize, ignoreOrder: ignoreOrder);

        /// <inheritdoc cref="CollectionUtil.IsIdentical{T}(IEnumerable{T}, IEnumerable{T}, IEqualityComparer{T}, Optimization, bool)"/>
        public static bool IsIdentical<T>(this IEnumerable<T> controlCollection, IEnumerable<T> compareCollection, IEqualityComparer<T> comparer, Optimization optimize = default, bool ignoreOrder = false) =>
            CollectionUtil.IsIdentical(controlCollection, compareCollection, comparer, optimize: optimize, ignoreOrder: ignoreOrder);

        /// <inheritdoc cref="CollectionUtil.IsIdenticalIgnoreCase(IEnumerable{string}, IEnumerable{string}, Optimization, bool)"/>
        public static bool IsIdenticalIgnoreCase(this IEnumerable<string> controlCollection, IEnumerable<string> compareCollection, Optimization optimize = default, bool ignoreOrder = false) =>
            CollectionUtil.IsIdenticalIgnoreCase(controlCollection, compareCollection, optimize: optimize, ignoreOrder: ignoreOrder);

        /// <summary>
        /// Checks if the supplied item is the first in the collection
        /// </summary>
        /// <typeparam name="T">Collection type</typeparam>
        /// <param name="collection">A collection</param>
        /// <param name="item">An item</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool StartsWith<T>(this IEnumerable<T> collection, T item)
        {
            if (collection == null || !collection.Any())
                return false;
            var list = CollectionUtil.ToList(collection, optimize: Optimization.ReuseCollection);
            return list.IndexOf(item) == 0;
        }

        /// <summary>
        /// Checks if the supplied item is the last in the collection
        /// </summary>
        /// <typeparam name="T">Collection type</typeparam>
        /// <param name="collection">A collection</param>
        /// <param name="item">An item</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool EndsWith<T>(this IEnumerable<T> collection, T item)
        {
            if (collection == null || !collection.Any())
                return false;
            var list = CollectionUtil.ToList(collection, optimize: Optimization.ReuseCollection);
            return list.LastIndexOf(item) == list.Count - 1;
        }
    }
}
