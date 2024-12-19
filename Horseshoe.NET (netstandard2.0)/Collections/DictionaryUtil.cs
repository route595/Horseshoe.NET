using System.Collections.Generic;
using System.Linq;

namespace Horseshoe.NET.Collections
{
    /// <summary>
    /// A collection of utility methods for <c>IDictionary</c>.
    /// </summary>
    public static class DictionaryUtil
    {
        /// <summary>
        /// Removes and returns a value from a dictionary, like <c>Array.pop()</c> in JavaScript
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">A non-null dictionary</param>
        /// <param name="key">The key to search</param>
        /// <param name="item">The extracted item</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool Extract<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key, out TValue item)
        {
            if (dictionary.ContainsKey(key))
            {
                var value = dictionary[key];
                dictionary.Remove(key);
                item = value;
                return true;
            }
            item = default;
            return false;
        }

        /// <summary>
        /// Appends multiple dictionaries into another.  By default, identical keys are merged right-to-left, i.e. table(s) being appended overwrite 
        /// table being appended to.  To change this behavior see <see cref="MergeOptions"/>.
        /// Returns a non-<c>null</c> dictionary, possibly with 0 elements.
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">A dictionary</param>
        /// <param name="dictionariesToAppend">Dictionaries to append</param>
        /// <param name="options">Dictionary merge options</param>
        /// <returns>A non-<c>null</c> dictionary, possibly with 0 elements</returns>
        public static IDictionary<TKey, TValue> Append<TKey, TValue>(IDictionary<TKey, TValue> dictionary, IEnumerable<IDictionary<TKey, TValue>> dictionariesToAppend, MergeOptions<TKey, TValue> options = null)
        {
            List<IDictionary<TKey, TValue>> list = dictionariesToAppend != null
                ? new List<IDictionary<TKey, TValue>>(dictionariesToAppend)
                : new List<IDictionary<TKey, TValue>>();
            list.Insert(0, dictionary);

            return Combine(list, options: options);
        }

        /// <summary>
        /// Combines multiple dictionaries into one.  Identical keys are merged left-to-right i.e. table being appended to is not overwritten.
        /// Returns a non-<c>null</c> dictionary, possibly with 0 elements.
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">A dictionary</param>
        /// <param name="dictionariesToAppend">Dictionaries to append</param>
        /// <returns>A non-<c>null</c> dictionary, possibly with 0 elements</returns>
        public static IDictionary<TKey, TValue> AppendLTR<TKey, TValue>(IDictionary<TKey, TValue> dictionary, params IDictionary<TKey, TValue>[] dictionariesToAppend) =>
            Append(dictionary, dictionariesToAppend, options: new MergeOptions<TKey, TValue> { Mode = DictionaryMergeMode.LTR });

        /// <summary>
        /// Combines multiple dictionaries into one.  Identical keys are merged right-to-left i.e. table(s) being appended overwrite table being appended to.
        /// Returns a non-<c>null</c> dictionary, possibly with 0 elements.
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">A dictionary</param>
        /// <param name="dictionariesToAppend">Dictionaries to append</param>
        /// <returns>A non-<c>null</c> dictionary, possibly with 0 elements</returns>
        public static IDictionary<TKey, TValue> AppendRTL<TKey, TValue>(IDictionary<TKey, TValue> dictionary, params IDictionary<TKey, TValue>[] dictionariesToAppend) =>
            Append(dictionary, dictionariesToAppend, options: new MergeOptions<TKey, TValue> { Mode = DictionaryMergeMode.RTL });

        /// <summary>
        /// Combines multiple dictionaries into one.  By default, identical keys are merged right-to-left i.e. table(s) being appended overwrite 
        /// table being appended to.  To change this behavior see <see cref="MergeOptions"/>.
        /// Returns a non-<c>null</c> dictionary, possibly with 0 elements.
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">A dictionary</param>
        /// <param name="dictionaryToAppend">Dictionary to append</param>
        /// <param name="options">Dictionary merge options</param>
        /// <returns>A non-<c>null</c> dictionary, possibly with 0 elements</returns>
        public static IDictionary<TKey, TValue> Append<TKey, TValue>(IDictionary<TKey, TValue> dictionary, IDictionary<TKey, TValue> dictionaryToAppend, MergeOptions<TKey, TValue> options = null) =>
            Append(dictionary, new[] { dictionaryToAppend }, options: options);

        /// <summary>
        /// Combines multiple dictionaries into one.  Identical keys are merged left-to-right i.e. table being appended to is not overwritten.
        /// Returns a non-<c>null</c> dictionary, possibly with 0 elements.
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">A dictionary</param>
        /// <param name="dictionaryToAppend">Dictionaries to append</param>
        /// <returns>A non-<c>null</c> dictionary, possibly with 0 elements</returns>
        public static IDictionary<TKey, TValue> AppendLTR<TKey, TValue>(IDictionary<TKey, TValue> dictionary, IDictionary<TKey, TValue> dictionaryToAppend) =>
            Append(dictionary, dictionaryToAppend, options: new MergeOptions<TKey, TValue> { Mode = DictionaryMergeMode.LTR });

        /// <summary>
        /// Combines multiple dictionaries into one.  Identical keys are merged left-to-right i.e. table being appended to is not overwritten.
        /// Returns a non-<c>null</c> dictionary, possibly with 0 elements. 
        /// Optimized for improved memory managment, however, source dictionary could be altered before being returned by the method.
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">A dictionary</param>
        /// <param name="dictionaryToAppend">Dictionaries to append</param>
        /// <returns>A non-<c>null</c> dictionary, possibly with 0 elements</returns>
        public static IDictionary<TKey, TValue> AppendLTR_Optimized<TKey, TValue>(IDictionary<TKey, TValue> dictionary, IDictionary<TKey, TValue> dictionaryToAppend) =>
            Append(dictionary, dictionaryToAppend, options: new MergeOptions<TKey, TValue> { Mode = DictionaryMergeMode.LTR, Optimize = true });

        /// <summary>
        /// Combines multiple dictionaries into one.  Identical keys are merged right-to-left i.e. table(s) being appended overwrite table being appended to.
        /// Returns a non-<c>null</c> dictionary, possibly with 0 elements.
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">A dictionary</param>
        /// <param name="dictionaryToAppend">Dictionaries to append</param>
        /// <returns>A non-<c>null</c> dictionary, possibly with 0 elements</returns>
        public static IDictionary<TKey, TValue> AppendRTL<TKey, TValue>(IDictionary<TKey, TValue> dictionary, IDictionary<TKey, TValue> dictionaryToAppend) =>
            Append(dictionary, dictionaryToAppend, options: new MergeOptions<TKey, TValue> { Mode = DictionaryMergeMode.RTL });

        /// <summary>
        /// Combines multiple dictionaries into one.  Identical keys are merged right-to-left i.e. table(s) being appended overwrite table being appended to.
        /// Returns a non-<c>null</c> dictionary, possibly with 0 elements.
        /// Optimized for improved memory managment, however, source dictionary could be altered before being returned by the method.
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">A dictionary</param>
        /// <param name="dictionaryToAppend">Dictionaries to append</param>
        /// <returns>A non-<c>null</c> dictionary, possibly with 0 elements</returns>
        public static IDictionary<TKey, TValue> AppendRTL_Optimized<TKey, TValue>(IDictionary<TKey, TValue> dictionary, IDictionary<TKey, TValue> dictionaryToAppend) =>
            Append(dictionary, dictionaryToAppend, options: new MergeOptions<TKey, TValue> { Mode = DictionaryMergeMode.RTL, Optimize = true });

        /// <summary>
        /// Combines multiple dictionaries into one. By default, identical keys are merged right-to-left, i.e. table(s) being appended overwrite 
        /// table being appended to.  To change this behavior see <see cref="MergeOptions"/>.
        /// Returns a non-<c>null</c> dictionary, possibly with 0 elements.
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionaries">Dictionaries to combine</param>
        /// <param name="options">Dictionary merge options</param>
        /// <returns>A non-<c>null</c> dictionary, possibly with 0 elements</returns>
        public static IDictionary<TKey, TValue> Combine<TKey, TValue>(IEnumerable<IDictionary<TKey, TValue>> dictionaries, MergeOptions<TKey, TValue> options = null)
        {
            dictionaries = ArrayUtil.Prune(dictionaries);

            if (!dictionaries.Any())
                return new Dictionary<TKey, TValue>();

            options = options ?? new MergeOptions<TKey, TValue>();

            var dictionary = options.Optimize
                ? dictionaries.First()
                : new Dictionary<TKey, TValue>(dictionaries.First());

            var temp = new List<IDictionary<TKey, TValue>>(dictionaries);
            temp.RemoveAt(0);
            dictionaries = temp.ToArray();

            foreach (var dictionaryToAppend in dictionaries)
            {
                foreach (var kvp in dictionaryToAppend)
                {
                    if (dictionary.ContainsKey(kvp.Key))
                    {
                        TValue selectedValue;
                        if (options.Merge != null)
                        {
                            selectedValue = options.Merge.Invoke(dictionary, dictionaryToAppend, kvp.Key);
                        }
                        else
                        {
                            switch (options.Mode)
                            {
                                case DictionaryMergeMode.RTL:
                                default:
                                    selectedValue = kvp.Value;
                                    break;
                                case DictionaryMergeMode.LTR:
                                    continue;
                            }
                        }
                        dictionary[kvp.Key] = selectedValue;
                    }
                    else
                    {
                        dictionary.Add(kvp.Key, kvp.Value);
                    }
                }
            }
            return dictionary;
        }

        /// <summary>
        /// Combines multiple dictionaries into one. Identical keys are merged left-to-right, i.e. table being appended to is not overwritten.  
        /// Returns a non-<c>null</c> dictionary, possibly with 0 elements.
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionaries">Dictionaries to combine</param>
        /// <returns>A non-<c>null</c> dictionary, possibly with 0 elements</returns>
        public static IDictionary<TKey, TValue> CombineLTR<TKey, TValue>(params IDictionary<TKey, TValue>[] dictionaries) =>
            Combine(dictionaries, options: new MergeOptions<TKey, TValue> { Mode = DictionaryMergeMode.LTR });

        /// <summary>
        /// Combines multiple dictionaries into one. Identical keys are merged left-to-right, i.e. table being appended to is not overwritten.  
        /// Returns a non-<c>null</c> dictionary, possibly with 0 elements.
        /// Optimized for improved memory managment, however, source dictionary could be altered before being returned by the method.
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionaries">Dictionaries to combine</param>
        /// <returns>A non-<c>null</c> dictionary, possibly with 0 elements</returns>
        public static IDictionary<TKey, TValue> CombineLTR_Optimized<TKey, TValue>(params IDictionary<TKey, TValue>[] dictionaries) =>
            Combine(dictionaries, options: new MergeOptions<TKey, TValue> { Mode = DictionaryMergeMode.LTR, Optimize = true });

        /// <summary>
        /// Combines multiple dictionaries into one. Identical keys are merged right-to-left, i.e. table(s) being appended overwrite table being appended to.  
        /// Returns a non-<c>null</c> dictionary, possibly with 0 elements.
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionaries">Dictionaries to combine</param>
        /// <returns>A non-<c>null</c> dictionary, possibly with 0 elements</returns>
        public static IDictionary<TKey, TValue> CombineRTL<TKey, TValue>(params IDictionary<TKey, TValue>[] dictionaries) =>
            Combine(dictionaries, options: new MergeOptions<TKey, TValue> { Mode = DictionaryMergeMode.RTL });

        /// <summary>
        /// Combines multiple dictionaries into one. Identical keys are merged right-to-left, i.e. table(s) being appended overwrite table being appended to.  
        /// Returns a non-<c>null</c> dictionary, possibly with 0 elements.
        /// Optimized for improved memory managment, however, source dictionary could be altered before being returned by the method.
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionaries">Dictionaries to combine</param>
        /// <returns>A non-<c>null</c> dictionary, possibly with 0 elements</returns>
        public static IDictionary<TKey, TValue> CombineRTL_Optimized<TKey, TValue>(params IDictionary<TKey, TValue>[] dictionaries) =>
            Combine(dictionaries, options: new MergeOptions<TKey, TValue> { Mode = DictionaryMergeMode.RTL });
    }
}
