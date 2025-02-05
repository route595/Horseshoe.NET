using System.Collections.Generic;

namespace Horseshoe.NET.Collections
{
    /// <summary>
    /// A collection of <c>List</c> extension methods
    /// </summary>
    public static class ListExtensions
    {

        /* * * * * * * * * * * * * * 
         *         GROUP 1
         * * * * * * * * * * * * * */

        /// <inheritdoc cref="ListUtil.TrimAll(IEnumerable{string})"/>
        public static List<string> TrimAll(this List<string> collection) =>
            ListUtil.TrimAll(collection);

        /// <inheritdoc cref="ListUtil.Zap(IEnumerable{string}, PruneOptions)"/>
        public static List<string> Zap(this List<string> collection, PruneOptions pruneOptions = default) =>
             ListUtil.Zap(collection, pruneOptions: pruneOptions);

        /// <inheritdoc cref="ListUtil.Prune{T}(IEnumerable{T}, PruneOptions)"/>
        public static List<T> Prune<T>(this List<T> collection, PruneOptions pruneOptions = PruneOptions.All) =>
             ListUtil.Prune(collection, pruneOptions: pruneOptions);

        /* * * * * * * * * * * * * * 
         *         GROUP 2
         * * * * * * * * * * * * * */

        /// <inheritdoc cref="ListUtil.Pad{T}(IEnumerable{T}, int, CollectionBoundary, T, bool)"/>
        public static List<T> Pad<T>(this List<T> list, int targetSize, CollectionBoundary boundary = default, T padWith = default, bool cannotExceedTargetSize = false) =>
            ListUtil.Pad(list, targetSize, boundary: boundary, padWith: padWith, cannotExceedTargetSize: cannotExceedTargetSize);

        /// <inheritdoc cref="ListUtil.Crop{T}(IEnumerable{T}, int, CollectionBoundary)"/>
        public static List<T> Crop<T>(this List<T> list, int targetSize, CollectionBoundary boundary = default) =>
            ListUtil.Crop(list, targetSize, boundary: boundary);

        /// <inheritdoc cref="ListUtil.Fit{T}(IEnumerable{T}, int, CollectionBoundary, CollectionBoundary, T)"/>
        public static List<T> Fit<T>(this List<T> list, int targetSize, CollectionBoundary cropBoundary = default, CollectionBoundary padBoundary = default, T padWith = default) =>
            ListUtil.Fit(list, targetSize, cropBoundary: cropBoundary, padBoundary: padBoundary, padWith: padWith);

        /* * * * * * * * * * * * * * 
         *         GROUP 3
         * * * * * * * * * * * * * */

        /// <inheritdoc cref="ListUtil.Append{T}(IEnumerable{T}, IEnumerable{T})"/>
        public static List<T> Append<T>(this List<T> list, IEnumerable<T> items) =>
            ListUtil.Append(list, items);

        /// <inheritdoc cref="ListUtil.Append{T}(IEnumerable{T}, T[])"/>
        public static List<T> Append<T>(this List<T> list, params T[] items) =>
            ListUtil.Append(list, items);

        /// <inheritdoc cref="ListUtil.AppendIf{T}(IEnumerable{T}, bool, IEnumerable{T})"/>
        public static List<T> AppendIf<T>(this List<T> list, bool condition, IEnumerable<T> items) =>
            ListUtil.AppendIf(list, condition, items);

        /// <inheritdoc cref="ListUtil.AppendIf{T}(IEnumerable{T}, bool, T[])"/>
        public static List<T> AppendIf<T>(this List<T> list, bool condition, params T[] items) =>
            ListUtil.AppendIf(list, condition, items);

        /// <inheritdoc cref="ListUtil.Append{T}(IEnumerable{T}, IEnumerable{IEnumerable{T}})"/>
        public static List<T> Append<T>(this List<T> list, IEnumerable<IEnumerable<T>> collections) =>
            ListUtil.Append(list, collections);

        /// <inheritdoc cref="ListUtil.Append{T}(IEnumerable{T}, IEnumerable{T}[])"/>
        public static List<T> Append<T>(this List<T> list, params IEnumerable<T>[] collections) =>
            ListUtil.Append(list, collections);

        /// <inheritdoc cref="ListUtil.AppendIf{T}(IEnumerable{T}, bool, IEnumerable{IEnumerable{T}})"/>
        public static List<T> AppendIf<T>(this List<T> list, bool condition, IEnumerable<IEnumerable<T>> collections) =>
            ListUtil.AppendIf(list, condition, collections);

        /// <inheritdoc cref="ListUtil.AppendIf{T}(IEnumerable{T}, bool, IEnumerable{T}[])"/>
        public static List<T> AppendIf<T>(this List<T> list, bool condition, params IEnumerable<T>[] collections) =>
            ListUtil.AppendIf(list, condition, collections);

        /// <inheritdoc cref="ListUtil.Insert{T}(IEnumerable{T}, int, IEnumerable{T})"/>
        public static List<T> Insert<T>(this List<T> list, int index, IEnumerable<T> items) =>
            ListUtil.Insert(list, index, items);

        /// <inheritdoc cref="ListUtil.Insert{T}(IEnumerable{T}, int, T[])"/>
        public static List<T> Insert<T>(this List<T> list, int index, params T[] items) =>
            ListUtil.Insert(list, index, items);

        /// <inheritdoc cref="ListUtil.InsertIf{T}(IEnumerable{T}, bool, int, IEnumerable{T})"/>
        public static List<T> InsertIf<T>(this List<T> list, bool condition, int index, IEnumerable<T> items) =>
            ListUtil.InsertIf(list, condition, index, items);

        /// <inheritdoc cref="ListUtil.InsertIf{T}(IEnumerable{T}, bool, int, T[])"/>
        public static List<T> InsertIf<T>(this List<T> list, bool condition, int index, params T[] items) =>
            ListUtil.InsertIf(list, condition, index, items);

        /// <inheritdoc cref="ListUtil.Insert{T}(IEnumerable{T}, int, IEnumerable{IEnumerable{T}})"/>
        public static List<T> Insert<T>(this List<T> list, int index, IEnumerable<IEnumerable<T>> collections) =>
            ListUtil.Insert(list, index, collections);

        /// <inheritdoc cref="ListUtil.Insert{T}(IEnumerable{T}, int, IEnumerable{T}[])"/>
        public static List<T> Insert<T>(this List<T> list, int index, params IEnumerable<T>[] collections) =>
            ListUtil.Insert(list, index, collections);

        /// <inheritdoc cref="ListUtil.InsertIf{T}(IEnumerable{T}, bool, int, IEnumerable{IEnumerable{T}})"/>
        public static List<T> InsertIf<T>(this List<T> list, bool condition, int index, IEnumerable<IEnumerable<T>> collections) =>
            ListUtil.InsertIf(list, condition, index, collections);

        /// <inheritdoc cref="ListUtil.InsertIf{T}(IEnumerable{T}, bool, int, IEnumerable{T}[])"/>
        public static List<T> InsertIf<T>(this List<T> list, bool condition, int index, params IEnumerable<T>[] collections) =>
            ListUtil.InsertIf(list, condition, index, collections);

        /// <inheritdoc cref="ListUtil.Prepend{T}(IEnumerable{T}, IEnumerable{T})"/>
        public static List<T> Prepend<T>(this List<T> list, IEnumerable<T> items) =>
            ListUtil.Prepend(list, items);

        /// <inheritdoc cref="ListUtil.Prepend{T}(IEnumerable{T}, T[])"/>
        public static List<T> Prepend<T>(this List<T> list, params T[] items) =>
            ListUtil.Prepend(list, items);

        /// <inheritdoc cref="ListUtil.PrependIf{T}(IEnumerable{T}, bool, IEnumerable{T})"/>
        public static List<T> PrependIf<T>(this List<T> list, bool condition, IEnumerable<T> items) =>
            ListUtil.PrependIf(list, condition, items);

        /// <inheritdoc cref="ListUtil.PrependIf{T}(IEnumerable{T}, bool, T[])"/>
        public static List<T> PrependIf<T>(this List<T> list, bool condition, params T[] items) =>
            ListUtil.PrependIf(list, condition, items);

        /// <inheritdoc cref="ListUtil.Prepend{T}(IEnumerable{T}, IEnumerable{IEnumerable{T}})"/>
        public static List<T> Prepend<T>(this List<T> list, IEnumerable<IEnumerable<T>> collections) =>
            ListUtil.Prepend(list, collections);

        /// <inheritdoc cref="ListUtil.Prepend{T}(IEnumerable{T}, IEnumerable{T}[])"/>
        public static List<T> Prepend<T>(this List<T> list, params IEnumerable<T>[] collections) =>
            ListUtil.Prepend(list, collections);

        /// <inheritdoc cref="ListUtil.PrependIf{T}(IEnumerable{T}, bool, IEnumerable{IEnumerable{T}})"/>
        public static List<T> PrependIf<T>(this List<T> list, bool condition, IEnumerable<IEnumerable<T>> collections) =>
            ListUtil.PrependIf(list, condition, collections);

        /// <inheritdoc cref="ListUtil.PrependIf{T}(IEnumerable{T}, bool, IEnumerable{T}[])"/>
        public static List<T> PrependIf<T>(this List<T> list, bool condition, params IEnumerable<T>[] collections) =>
            ListUtil.PrependIf(list, condition, collections);

        /* * * * * * * * * * * * * * 
         *         GROUP 4
         * * * * * * * * * * * * * */

        /// <inheritdoc cref="ListUtil.Pop{T}(List{T})"/>
        public static T Pop<T>(this List<T> list) =>
            ListUtil.Pop(list);

        /// <inheritdoc cref="ListUtil.PopAt{T}(List{T}, int)"/>
        public static T PopAt<T>(this List<T> list, int index) =>
            ListUtil.PopAt(list, index);

        ///// <inheritdoc cref="ListUtil.ReplaceAll{T}(List{T}, T, T)"/>
        //public static List<T> ReplaceAll<T>(this List<T> list, T item, T replacement) =>
        //    ListUtil.ReplaceAll(list, item, replacement);

        ///// <inheritdoc cref="ListUtil.ReplaceAll{T}(List{T}, T, T, IEqualityComparer{T})"/>
        //public static List<T> ReplaceAll<T>(this List<T> list, T item, T replacement, IEqualityComparer<T> comparer) =>
        //    ListUtil.ReplaceAll(list, item, replacement, comparer);

        ///// <inheritdoc cref="ListUtil.ReplaceAllIgnoreCase(List{string}, string, string)"/>
        //public static List<string> ReplaceAllIgnoreCase(this List<string> list, string item, string replacement) =>
        //    ListUtil.ReplaceAllIgnoreCase(list, item, replacement);

        ///// <inheritdoc cref="ListUtil.IsIdentical{T}(List{T}, List{T}, Optimization, bool)"/>
        //public static bool IsIdentical<T>(this List<T> controlList, List<T> compareList, Optimization optimize = default, bool ignoreOrder = false) =>
        //    ListUtil.IsIdentical(controlList, compareList, optimize: optimize, ignoreOrder: ignoreOrder);

        ///// <inheritdoc cref="ListUtil.IsIdentical{T}(List{T}, List{T}, IEqualityComparer{T}, Optimization, bool)"/>
        //public static bool IsIdentical<T>(this List<T> controlList, List<T> compareList, IEqualityComparer<T> comparer, Optimization optimize = default, bool ignoreOrder = false) =>
        //    ListUtil.IsIdentical(controlList, compareList, comparer, optimize: optimize, ignoreOrder: ignoreOrder);

        ///// <inheritdoc cref="ListUtil.IsIdenticalIgnoreCase(List{string}, List{string}, Optimization, bool)"/>
        //public static bool IsIdenticalIgnoreCase(this List<string> controlList, List<string> compareList, Optimization optimize = default, bool ignoreOrder = false) =>
        //    ListUtil.IsIdenticalIgnoreCase(controlList, compareList, optimize: optimize, ignoreOrder: ignoreOrder);
    }
}
