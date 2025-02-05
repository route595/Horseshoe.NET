using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Horseshoe.NET.Collections
{
    /// <summary>
    /// A set of utility methods for collections.
    /// </summary>
    public static class CollectionUtil
    {
        /// <summary>
        /// Copies any collection to a new <c>List&lt;T&gt;</c>.  When using optimization, a collection
        /// already of type <c>List&lt;T&gt;</c> is reused instead of being copied to a new list. 
        /// A <c>null</c> source collection results in an empty <c>List&lt;T&gt;</c>.
        /// </summary>
        /// <typeparam name="T">The target collection type.</typeparam>
        /// <param name="collection">A collection to copy to a list, or simply returned (see optimization).</param>
        /// <param name="optimize">If <c>ReuseCollection</c> and <c>collection</c> is alredy of type <c>List&lt;T&gt;</c>, returns the original list. Default is <c>None</c>.</param>
        /// <returns>A <c>List&lt;T&gt;</c></returns>
        /// <exception cref="InvalidCastException"></exception>
        public static List<T> ToList<T>(System.Collections.IEnumerable collection, Optimization optimize = default)
        {
            if (((optimize & Optimization.ReuseCollection) == Optimization.ReuseCollection) && collection is List<T> list)
                return list;
            if (collection is IEnumerable<T> _collection)
                return new List<T>(_collection);
            if (collection == null)
                return new List<T>();
            return new List<T>(collection.Cast<T>());
        }

        /// <summary>
        /// Copies any collection to a new <c>List&lt;T&gt;</c>.  When using optimization, a collection
        /// already of type <c>List&lt;T&gt;</c> is reused instead of being copied to a new list. 
        /// A <c>null</c> source collection results in an empty <c>List&lt;T&gt;</c>.
        /// </summary>
        /// <typeparam name="T">The target collection type.</typeparam>
        /// <param name="collection">A collection to copy to a list, or simply returned (see optimization).</param>
        /// <param name="optimize">If <c>ReuseCollection</c> and <c>collection</c> is alredy of type <c>List&lt;T&gt;</c>, returns the original list. Default is <c>None</c>.</param>
        /// <returns>A <c>List&lt;T&gt;</c></returns>
        public static List<T> ToList<T>(IEnumerable<T> collection, Optimization optimize = default)
        {
            if (((optimize & Optimization.ReuseCollection) == Optimization.ReuseCollection) && collection is List<T> list)
                return list;
            if (collection == null)
                return new List<T>();
            return new List<T>(collection);
        }

        /// <summary>
        /// Copies any collection to a new <c>T[]</c>.  When using optimization, a collection
        /// already of type <c>T[]</c> is reused instead of being copied to a new array. 
        /// Optimizations in <c>ToList()</c> would also apply in cases when called under the hood.  
        /// A <c>null</c> source collection results in an empty <c>T[]</c>.
        /// </summary>
        /// <typeparam name="T">The target collection type.</typeparam>
        /// <param name="collection">A collection to copy to an array, or simply returned (see optimization).</param>
        /// <param name="optimize">If <c>ReuseCollection</c> and <c>collection</c> is alredy of type <c>T[]</c>, returns the original array. Default is <c>None</c>.</param>
        /// <returns>A <c>T[]</c></returns>
        /// <exception cref="InvalidCastException"></exception>
        public static T[] ToArray<T>(System.Collections.IEnumerable collection, Optimization optimize = default)
        {
            if (((optimize & Optimization.ReuseCollection) == Optimization.ReuseCollection) && collection is T[] array)
                return array;
            if (collection is IEnumerable<T> _collection)
                return ToList(_collection, optimize: optimize).ToArray();
            if (collection == null)
                return Array.Empty<T>();
            return ToList(collection.Cast<T>(), optimize: Optimization.None).ToArray();
        }

        /// <summary>
        /// Copies any collection to a new <c>T[]</c>.  When using optimization, a collection
        /// already of type <c>T[]</c> is reused instead of being copied to a new array. 
        /// Optimizations in <c>ToList()</c> would also apply in cases when called under the hood.  
        /// A <c>null</c> source collection results in an empty <c>T[]</c>.
        /// </summary>
        /// <typeparam name="T">The target collection type.</typeparam>
        /// <param name="collection">A collection to copy to an array, or simply returned (see optimization).</param>
        /// <param name="optimize">If <c>ReuseCollection</c> and <c>collection</c> is alredy of type <c>T[]</c>, returns the original array. Default is <c>None</c>.</param>
        /// <returns>A <c>T[]</c></returns>
        public static T[] ToArray<T>(IEnumerable<T> collection, Optimization optimize = default)
        {
            if (((optimize & Optimization.ReuseCollection) == Optimization.ReuseCollection) && collection is T[] array)
                return array;
            if (collection == null)
                return Array.Empty<T>();
            return ToList(collection, optimize: optimize).ToArray();
        }

        /* * * * * * * * * * * * * * 
         *         GROUP 1
         * * * * * * * * * * * * * */

        /// <summary>
        /// Removes all leading and trailing whitespace characters from each non-null <c>string</c> in the collection.
        /// Optimizations are as follows.
        /// <list type="number">
        /// <item>
        /// If the source collection is of type <c>string[]</c> or <c>List&lt;string&gt;</c> that array or list is reused behind the scenes instead of instantiating a new <c>List&lt;string&gt;</c>.
        /// </item>
        /// <item>
        /// An index-based array/list scan is used in lieu of Linq-style collection transforms.
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="collection">A <c>string</c> collection</param>
        /// <returns>The collection</returns>
        public static IEnumerable<string> TrimAll(IEnumerable<string> collection)
        {
            if (collection is string[] stringArray)
            {
                return ArrayUtil.TrimAll(stringArray);
            }
            return ListUtil.TrimAll(ToList(collection, optimize: Optimization.ReuseCollection));
        }

        /// <inheritdoc cref="ListUtil.Zap(IEnumerable{string}, PruneOptions)"/>
        public static IEnumerable<string> Zap(IEnumerable<string> collection, PruneOptions pruneOptions = default)
        {
            return ListUtil.Zap(collection, pruneOptions: pruneOptions);
        }

        /// <inheritdoc cref="ListUtil.Prune{T}(IEnumerable{T}, PruneOptions)"/>
        public static IEnumerable<T> Prune<T>(IEnumerable<T> collection, PruneOptions pruneOptions = default)
        {
            return ListUtil.Prune(collection, pruneOptions: pruneOptions);
        }

        /* * * * * * * * * * * * * * 
         *         GROUP 2
         * * * * * * * * * * * * * */

        /// <inheritdoc cref="ListUtil.Pad{T}(IEnumerable{T}, int, CollectionBoundary, T, bool)"/>
        public static IEnumerable<T> Pad<T>(IEnumerable<T> collection, int targetSize, CollectionBoundary boundary = default, T padWith = default, bool cannotExceedTargetSize = false) =>
            ListUtil.Pad(collection, targetSize, boundary: boundary, padWith: padWith, cannotExceedTargetSize: cannotExceedTargetSize);

        /// <inheritdoc cref="ListUtil.Crop{T}(IEnumerable{T}, int, CollectionBoundary)"/>
        public static IEnumerable<T> Crop<T>(IEnumerable<T> collection, int targetSize, CollectionBoundary boundary = default) =>
            ListUtil.Crop(collection, targetSize, boundary: boundary);

        /// <inheritdoc cref="ListUtil.Fit{T}(IEnumerable{T}, int, CollectionBoundary, CollectionBoundary, T)"/>
        public static IEnumerable<T> Fit<T>(IEnumerable<T> collection, int targetSize, CollectionBoundary cropBoundary = default, CollectionBoundary padBoundary = default, T padWith = default) =>
            ListUtil.Fit(collection, targetSize, cropBoundary: cropBoundary, padBoundary: padBoundary, padWith: padWith);

        /* * * * * * * * * * * * * * 
         *         GROUP 3
         * * * * * * * * * * * * * */

        /// <inheritdoc cref="ListUtil.Append{T}(IEnumerable{T}, IEnumerable{T})"/>
        public static IEnumerable<T> Append<T>(IEnumerable<T> list, IEnumerable<T> items) =>
            ListUtil.Append(list, items);

        /// <inheritdoc cref="ListUtil.Append{T}(IEnumerable{T}, T[])"/>
        public static IEnumerable<T> Append<T>(IEnumerable<T> list, params T[] items) =>
            ListUtil.Append(list, items);

        /// <inheritdoc cref="ListUtil.AppendIf{T}(IEnumerable{T}, bool, IEnumerable{T})"/>
        public static IEnumerable<T> AppendIf<T>(IEnumerable<T> list, bool condition, IEnumerable<T> items) =>
            ListUtil.AppendIf(list, condition, items);

        /// <inheritdoc cref="ListUtil.AppendIf{T}(IEnumerable{T}, bool, T[])"/>
        public static IEnumerable<T> AppendIf<T>(IEnumerable<T> list, bool condition, params T[] items) =>
            ListUtil.AppendIf(list, condition, items);

        /// <inheritdoc cref="ListUtil.Append{T}(IEnumerable{T}, IEnumerable{IEnumerable{T}})"/>
        public static IEnumerable<T> Append<T>(IEnumerable<T> list, IEnumerable<IEnumerable<T>> collections) =>
            ListUtil.Append(list, collections);

        /// <inheritdoc cref="ListUtil.Append{T}(IEnumerable{T}, IEnumerable{T}[])"/>
        public static IEnumerable<T> Append<T>(IEnumerable<T> list, params IEnumerable<T>[] collections) =>
            ListUtil.Append(list, collections);

        /// <inheritdoc cref="ListUtil.AppendIf{T}(IEnumerable{T}, bool, IEnumerable{IEnumerable{T}})"/>
        public static IEnumerable<T> AppendIf<T>(IEnumerable<T> list, bool condition, IEnumerable<IEnumerable<T>> collections) =>
            ListUtil.AppendIf(list, condition, collections);

        /// <inheritdoc cref="ListUtil.AppendIf{T}(IEnumerable{T}, bool, IEnumerable{T}[])"/>
        public static IEnumerable<T> AppendIf<T>(IEnumerable<T> list, bool condition, params IEnumerable<T>[] collections) =>
            ListUtil.AppendIf(list, condition, collections);

        /// <inheritdoc cref="ListUtil.Insert{T}(IEnumerable{T}, int, IEnumerable{T})"/>
        public static IEnumerable<T> Insert<T>(IEnumerable<T> list, int index, IEnumerable<T> items) =>
            ListUtil.Insert(list, index, items);

        /// <inheritdoc cref="ListUtil.Insert{T}(IEnumerable{T}, int, T[])"/>
        public static IEnumerable<T> Insert<T>(IEnumerable<T> list, int index, params T[] items) =>
            ListUtil.Insert(list, index, items);

        /// <inheritdoc cref="ListUtil.InsertIf{T}(IEnumerable{T}, bool, int, IEnumerable{T})"/>
        public static IEnumerable<T> InsertIf<T>(IEnumerable<T> list, bool condition, int index, IEnumerable<T> items) =>
            ListUtil.InsertIf(list, condition, index, items);

        /// <inheritdoc cref="ListUtil.InsertIf{T}(IEnumerable{T}, bool, int, T[])"/>
        public static IEnumerable<T> InsertIf<T>(IEnumerable<T> list, bool condition, int index, params T[] items) =>
            ListUtil.InsertIf(list, condition, index, items);

        /// <inheritdoc cref="ListUtil.Insert{T}(IEnumerable{T}, int, IEnumerable{IEnumerable{T}})"/>
        public static IEnumerable<T> Insert<T>(IEnumerable<T> list, int index, IEnumerable<IEnumerable<T>> collections) =>
            ListUtil.Insert(list, index, collections);

        /// <inheritdoc cref="ListUtil.Insert{T}(IEnumerable{T}, int, IEnumerable{T}[])"/>
        public static IEnumerable<T> Insert<T>(IEnumerable<T> list, int index, params IEnumerable<T>[] collections) =>
            ListUtil.Insert(list, index, collections);

        /// <inheritdoc cref="ListUtil.InsertIf{T}(IEnumerable{T}, bool, int, IEnumerable{IEnumerable{T}})"/>
        public static IEnumerable<T> InsertIf<T>(IEnumerable<T> list, bool condition, int index, IEnumerable<IEnumerable<T>> collections) =>
            ListUtil.InsertIf(list, condition, index, collections);

        /// <inheritdoc cref="ListUtil.InsertIf{T}(IEnumerable{T}, bool, int, IEnumerable{T}[])"/>
        public static IEnumerable<T> InsertIf<T>(IEnumerable<T> list, bool condition, int index, params IEnumerable<T>[] collections) =>
            ListUtil.InsertIf(list, condition, index, collections);

        /// <inheritdoc cref="ListUtil.Prepend{T}(IEnumerable{T}, IEnumerable{T})"/>
        public static IEnumerable<T> Prepend<T>(IEnumerable<T> list, IEnumerable<T> items) =>
            ListUtil.Prepend(list, items);

        /// <inheritdoc cref="ListUtil.Prepend{T}(IEnumerable{T}, T[])"/>
        public static IEnumerable<T> Prepend<T>(IEnumerable<T> list, params T[] items) =>
            ListUtil.Prepend(list, items);

        /// <inheritdoc cref="ListUtil.PrependIf{T}(IEnumerable{T}, bool, IEnumerable{T})"/>
        public static IEnumerable<T> PrependIf<T>(IEnumerable<T> list, bool condition, IEnumerable<T> items) =>
            ListUtil.PrependIf(list, condition, items);

        /// <inheritdoc cref="ListUtil.PrependIf{T}(IEnumerable{T}, bool, T[])"/>
        public static IEnumerable<T> PrependIf<T>(IEnumerable<T> list, bool condition, params T[] items) =>
            ListUtil.PrependIf(list, condition, items);

        /// <inheritdoc cref="ListUtil.Prepend{T}(IEnumerable{T}, IEnumerable{IEnumerable{T}})"/>
        public static IEnumerable<T> Prepend<T>(IEnumerable<T> list, IEnumerable<IEnumerable<T>> collections) =>
            ListUtil.Prepend(list, collections);

        /// <inheritdoc cref="ListUtil.Prepend{T}(IEnumerable{T}, IEnumerable{T}[])"/>
        public static IEnumerable<T> Prepend<T>(IEnumerable<T> list, params IEnumerable<T>[] collections) =>
            ListUtil.Prepend(list, collections);

        /// <inheritdoc cref="ListUtil.PrependIf{T}(IEnumerable{T}, bool, IEnumerable{IEnumerable{T}})"/>
        public static IEnumerable<T> PrependIf<T>(IEnumerable<T> list, bool condition, IEnumerable<IEnumerable<T>> collections) =>
            ListUtil.PrependIf(list, condition, collections);

        /// <inheritdoc cref="ListUtil.PrependIf{T}(IEnumerable{T}, bool, IEnumerable{T}[])"/>
        public static IEnumerable<T> PrependIf<T>(IEnumerable<T> list, bool condition, params IEnumerable<T>[] collections) =>
            ListUtil.PrependIf(list, condition, collections);

        /* * * * * * * * * * * * * * 
         *         GROUP 4
         * * * * * * * * * * * * * */

        /// <summary>
        /// Combines zero or more collections into a single list
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="collections">Collections to combine</param>
        /// <returns>The combined <c>List&lt;T&gt;</c></returns>
        public static IEnumerable<T> Combine<T>(IEnumerable<IEnumerable<T>> collections)
        {
            return new CollectionBuilder<T>()
                .Append(collections)
                .RenderToList();
        }

        /// <summary>
        /// Combines zero or more collections into a single list
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="collections">Collections to combine</param>
        /// <returns>The combined <c>List&lt;T&gt;</c></returns>
        public static IEnumerable<T> Combine<T>(params IEnumerable<T>[] collections)
        {
            return new CollectionBuilder<T>()
                .Append(collections)
                .RenderToList();
        }

        /// <summary>
        /// Combines zero or more collections into a single list of distinct values
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="collections">Collections to combine</param>
        /// <returns>The combined <c>List&lt;T&gt;</c></returns>
        public static IEnumerable<T> CombineDistinct<T>(IEnumerable<IEnumerable<T>> collections)
        {
            return new CollectionBuilder<T>(distinctValues: true)
                .Append(collections)
                .RenderToList();
        }

        /// <summary>
        /// Combines zero or more collections into a single list of distinct values
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="collections">Collections to combine</param>
        /// <returns>The combined <c>List&lt;T&gt;</c></returns>
        public static IEnumerable<T> CombineDistinct<T>(params IEnumerable<T>[] collections)
        {
            return new CollectionBuilder<T>(distinctValues: true)
                .Append(collections)
                .RenderToList();
        }

        /// <inheritdoc cref="ListUtil.ReplaceAll{T}(List{T}, T, T)"/>
        public static IEnumerable<T> ReplaceAll<T>(IEnumerable<T> collection, T item, T replacement)
        {
            if (collection is T[] tArray)
                return ArrayUtil.ReplaceAll(tArray, item, replacement);
            return ListUtil.ReplaceAll(ToList(collection, optimize: Optimization.ReuseCollection), item, replacement);
        }

        /// <inheritdoc cref="ListUtil.ReplaceAll{T}(List{T}, T, T, IEqualityComparer{T})"/>
        public static IEnumerable<T> ReplaceAll<T>(IEnumerable<T> collection, T item, T replacement, IEqualityComparer<T> comparer)
        {
            if (collection is T[] tArray)
                return ArrayUtil.ReplaceAll(tArray, item, replacement, comparer);
            return ListUtil.ReplaceAll(ToList(collection, optimize: Optimization.ReuseCollection), item, replacement, comparer);
        }

        /// <inheritdoc cref="ListUtil.ReplaceAllIgnoreCase(List{string}, string, string)"/>
        public static IEnumerable<string> ReplaceAllIgnoreCase(IEnumerable<string> collection, string item, string replacement)
        {
            if (collection is string[] stringArray)
                return ArrayUtil.ReplaceAllIgnoreCase(stringArray, item, replacement);
            return ListUtil.ReplaceAllIgnoreCase(ToList(collection, optimize: Optimization.ReuseCollection), item, replacement);
        }

        /// <summary>
        /// Determines whether a sequence contains a specified element by using the default equality comparer.
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="collection">A collection to search</param>
        /// <param name="item">The item to find</param>
        /// <returns>If item found then <c>true</c>, <c>false</c> otherwise.</returns>
        public static bool Contains<T>(IEnumerable<T> collection, T item)
        {
            if (collection == null)
                return false;
            return collection.Contains(item);
        }

        /// <summary>
        /// Tests if a collection contains all of the supplied items.  
        /// Returns <c>false</c> if either <c>collection</c> or <c>items</c> is omitted.
        /// Optimizations are as follows.
        /// <list type="number">
        /// <item>
        /// The source collection is reduced to its distinct values before performing the item search.
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="collection">The collection in which to search for <c>items</c>.</param>
        /// <param name="items">Distinct items to search for in <c>collection</c></param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAll<T>(IEnumerable<T> collection, IEnumerable<T> items)
        {
            return new CollectionBuilder<T>(distinctValues: true)
                .Append(collection)
                .ContainsAll(items);
        }

        /// <summary>
        /// Tests if a collection contains all of the supplied items.  
        /// Returns <c>false</c> if either <c>collection</c> or <c>items</c> is omitted.
        /// Optimizations are as follows.
        /// <list type="number">
        /// <item>
        /// The source collection is reduced to its distinct values before performing the item search.
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="collection">The collection in which to search for <c>items</c>.</param>
        /// <param name="items">Distinct items to search for in <c>collection</c></param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAll<T>(IEnumerable<T> collection, params T[] items)
        {
            return ContainsAll(collection, items as IEnumerable<T>);
        }

        /// <summary>
        /// Tests if a collection contains all of the supplied <c>string</c>s.  
        /// Returns <c>false</c> if either <c>collection</c> or <c>items</c> is omitted.
        /// Optimizations are as follows.
        /// <list type="number">
        /// <item>
        /// The source <c>string</c> collection is reduced to its distinct values before performing the item search.
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="collection">The <c>string</c> collection in which to search for <c>items</c>.</param>
        /// <param name="items">Distinct <c>string</c>s to search for in <c>collection</c></param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAllIgnoreCase(IEnumerable<string> collection, IEnumerable<string> items)
        {
            if (collection == null || !collection.Any() || items == null || !items.Any())
                return false;
            bool itemFound;
            foreach (string item in items.Distinct())
            {
                itemFound = false;
                foreach (string value in collection.Distinct())
                {
                    if (string.Equals(item, value, StringComparison.CurrentCultureIgnoreCase))
                    {
                        itemFound = true;
                        break;
                    }
                }
                if (!itemFound)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Tests if a collection contains all of the supplied <c>string</c>s.  
        /// Returns <c>false</c> if either <c>collection</c> or <c>items</c> is omitted.
        /// Optimizations are as follows.
        /// <list type="number">
        /// <item>
        /// The source <c>string</c> collection is reduced to its distinct values before performing the item search.
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="collection">The <c>string</c> collection in which to search for <c>items</c>.</param>
        /// <param name="items">Distinct <c>string</c>s to search for in <c>collection</c></param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAllIgnoreCase(IEnumerable<string> collection, params string[] items)
        {
            return ContainsAllIgnoreCase(collection, items as IEnumerable<string>);
        }

        /// <summary>
        /// Tests if a collection contains at least one of the supplied items.  
        /// Returns <c>false</c> if either <c>collection</c> or <c>items</c> is omitted.
        /// Optimizations are as follows.
        /// <list type="number">
        /// <item>
        /// The source collection is reduced to its distinct values before performing the item search.
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="collection">The collection in which to search for <c>items</c>.</param>
        /// <param name="items">Distinct items to search for in <c>collection</c></param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAny<T>(IEnumerable<T> collection, IEnumerable<T> items)
        {
            return new CollectionBuilder<T>(distinctValues: true)
                .Append(collection)
                .ContainsAny(items);
        }

        /// <summary>
        /// Tests if a collection contains at least one of the supplied items.  
        /// Returns <c>false</c> if either <c>collection</c> or <c>items</c> is omitted.
        /// Optimizations are as follows.
        /// <list type="number">
        /// <item>
        /// The source collection is reduced to its distinct values before performing the item search.
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="collection">The collection in which to search for <c>items</c>.</param>
        /// <param name="items">Distinct items to search for in <c>collection</c></param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAny<T>(IEnumerable<T> collection, params T[] items)
        {
            return ContainsAny(collection, items as IEnumerable<T>);
        }

        /// <summary>
        /// Tests if a collection contains at least one of the supplied <c>string</c>s.  
        /// Returns <c>false</c> if either <c>collection</c> or <c>items</c> is omitted.
        /// Optimizations are as follows.
        /// <list type="number">
        /// <item>
        /// The source <c>string</c> collection is reduced to its distinct values before performing the item search.
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="collection">The <c>string</c> collection in which to search for <c>items</c>.</param>
        /// <param name="items">Distinct <c>string</c>s to search for in <c>collection</c></param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAnyIgnoreCase(IEnumerable<string> collection, IEnumerable<string> items)
        {
            if (collection == null || !collection.Any() || items == null || !items.Any())
                return false;
            foreach (string item in items.Distinct())
            {
                foreach (string value in collection.Distinct())
                {
                    if (string.Equals(item, value, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Tests if a collection contains at least one of the supplied <c>string</c>s.  
        /// Returns <c>false</c> if either <c>collection</c> or <c>items</c> is omitted.
        /// Optimizations are as follows.
        /// <list type="number">
        /// <item>
        /// The source <c>string</c> collection is reduced to its distinct values before performing the item search.
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="collection">The <c>string</c> collection in which to search for <c>items</c>.</param>
        /// <param name="items">Distinct <c>string</c>s to search for in <c>collection</c></param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAnyIgnoreCase(IEnumerable<string> collection, params string[] items)
        {
            return ContainsAnyIgnoreCase(collection, items as IEnumerable<string>);
        }

        /// <summary>
        /// Compares two collections for equality. Empty and <c>null</c> <c>collection</c>s are considered identical.
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="controlCollection">A collection to search</param>
        /// <param name="compareCollection">A collection to compare</param>
        /// <param name="optimize">
        /// Optimization options:
        /// <list type="bullet">
        /// <item>
        /// ReuseCollection - use this option if source collections are of type <c>List&lt;T&gt;</c> to reuse them under the hood rather than new lists.
        /// </item>
        /// <item>
        /// Distinct - use this option to shrink the source collections down to distinct values prior to searching.
        /// </item>
        /// </list>
        /// </param>
        /// <param name="ignoreOrder"><c>true</c> if not an order dependent comparison</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool IsIdentical<T>(IEnumerable<T> controlCollection, IEnumerable<T> compareCollection, Optimization optimize = default, bool ignoreOrder = false)
        {
            if (controlCollection is T[] controlArray && compareCollection is T[] compareArray)
                return ArrayUtil.IsIdentical(controlArray, compareArray, optimize: optimize, ignoreOrder: ignoreOrder);
            return ListUtil.IsIdentical(ToList(controlCollection, optimize: optimize), ToList(compareCollection, optimize: optimize), optimize: optimize, ignoreOrder: ignoreOrder);
        }

        /// <summary>
        /// Compares two collections for equality. Empty and <c>null</c> <c>collection</c>s are considered identical.
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="controlCollection">A collection to search</param>
        /// <param name="compareCollection">A collection to compare</param>
        /// <param name="comparer">An optional equality comparer</param>
        /// <param name="optimize">
        /// Optimization options:
        /// <list type="bullet">
        /// <item>
        /// ReuseCollection - use this option if source collections are of type <c>List&lt;T&gt;</c> to reuse them under the hood rather than new lists.
        /// </item>
        /// <item>
        /// Distinct - use this option to shrink the source collections down to distinct values prior to searching.
        /// </item>
        /// </list>
        /// </param>
        /// <param name="ignoreOrder"><c>true</c> if not an order dependent comparison</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool IsIdentical<T>(IEnumerable<T> controlCollection, IEnumerable<T> compareCollection, IEqualityComparer<T> comparer, Optimization optimize = default, bool ignoreOrder = false)
        {
            if (controlCollection is T[] controlArray && compareCollection is T[] compareArray)
                return ArrayUtil.IsIdentical(controlArray, compareArray, comparer, optimize: optimize, ignoreOrder: ignoreOrder);
            return ListUtil.IsIdentical(ToList(controlCollection, optimize: optimize), ToList(compareCollection, optimize: optimize), comparer, optimize: optimize, ignoreOrder: ignoreOrder);
        }

        /// <summary>
        /// Compares two <c>string</c> collections for equality. Empty and <c>null</c> <c>collection</c>s are considered identical.
        /// </summary>
        /// <param name="controlCollection">A <c>string</c> collection to search</param>
        /// <param name="compareCollection">A <c>string</c> collection to compare</param>
        /// <param name="optimize">
        /// Optimization options:
        /// <list type="bullet">
        /// <item>
        /// ReuseCollection - use this option if source collections are of type <c>List&lt;T&gt;</c> to reuse them under the hood rather than new lists.
        /// </item>
        /// <item>
        /// Distinct - use this option to shrink the source collections down to distinct values prior to searching.
        /// </item>
        /// </list>
        /// </param>
        /// <param name="ignoreOrder"><c>true</c> if not an order dependent comparison</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool IsIdenticalIgnoreCase(IEnumerable<string> controlCollection, IEnumerable<string> compareCollection, Optimization optimize = default, bool ignoreOrder = false)
        {
            if (controlCollection is string[] controlArray && compareCollection is string[] compareArray)
                return ArrayUtil.IsIdenticalIgnoreCase(controlArray, compareArray, optimize: optimize, ignoreOrder: ignoreOrder);
            return ListUtil.IsIdenticalIgnoreCase(ToList(controlCollection, optimize: optimize), ToList(compareCollection, optimize: optimize), optimize: optimize, ignoreOrder: ignoreOrder);
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
