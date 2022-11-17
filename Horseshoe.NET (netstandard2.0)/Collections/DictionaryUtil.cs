using System;
using System.Collections.Generic;
using System.Linq;

namespace Horseshoe.NET.Collections
{
    /// <summary>
    /// A collection of utility methods for <c>IDictionary</c>
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
        /// Combines multiple dictionaries into one, merges identical keys right-to-left (right-most replaces left-most)
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">A dictionary</param>
        /// <param name="dictionariesToAppend">Dictionaries to append</param>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> Append<TKey, TValue>(IDictionary<TKey, TValue> dictionary, params IDictionary<TKey, TValue>[] dictionariesToAppend)
        {
            return Append(null, dictionary, dictionariesToAppend);
        }

        /// <summary>
        /// Combines multiple dictionaries into one, merges identical keys right-to-left (right-most replaces left-most)
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="mergeFunc">The function that merges left/right values when identical keys are encountered</param>
        /// <param name="dictionary">A dictionary</param>
        /// <param name="dictionariesToAppend">Dictionaries to append</param>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> Append<TKey, TValue>(Func<TValue, TValue, TValue> mergeFunc, IDictionary <TKey, TValue> dictionary, params IDictionary<TKey, TValue>[] dictionariesToAppend)
        {
            dictionary = dictionary == null
                ? new Dictionary<TKey, TValue>()
                : new Dictionary<TKey, TValue>(dictionary);
            foreach (var dictToAppend in dictionariesToAppend)
            {
                dictionary.AddAll(dictToAppend, mergeFunc: mergeFunc);
            }
            return dictionary;
        }

        /// <summary>
        /// Appends zero or more dictionaries to a bas, merges identical keys left-to-right (left-most replaces right-most)
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">A dictionary</param>
        /// <param name="dictionariesToAppend">Dictionaries to append</param>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> AppendMergeLeftToRight<TKey, TValue>(IDictionary<TKey, TValue> dictionary, params IDictionary<TKey, TValue>[] dictionariesToAppend)
        {
            return Append((leftVal, rightVal) => leftVal, dictionary, dictionariesToAppend);
        }

        /// <summary>
        /// Combines multiple dictionaries into one, merges identical keys right-to-left (right-most replaces left-most)
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionaries">Dictionaries to combine</param>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> Combine<TKey, TValue>(params IDictionary<TKey, TValue>[] dictionaries)
        {
            return Combine(null, dictionaries);
        }

        /// <summary>
        /// Combines multiple dictionaries into one, merges identical keys left-to-right (left-most replaces right-most)
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="mergeFunc">The function that merges left/right values when identical keys are encountered</param>
        /// <param name="dictionaries">Dictionaries to combine</param>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> Combine<TKey, TValue>(Func<TValue, TValue, TValue> mergeFunc, params IDictionary<TKey, TValue>[] dictionaries)
        {
            if (dictionaries.Any())
            {
                var dict = new Dictionary<TKey, TValue>(dictionaries[0]);
                for (int i = 1; i < dictionaries.Length; i++)
                {
                    dict.AddAll(dictionaries[i], mergeFunc: mergeFunc);
                }
                return dict;
            }
            return new Dictionary<TKey, TValue>();
        }

        /// <summary>
        /// Combines multiple dictionaries into one, merges identical keys left-to-right (left-most replaces right-most)
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionaries">Dictionaries to combine</param>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> CombineMergeLeftToRight<TKey, TValue>(params IDictionary<TKey, TValue>[] dictionaries)
        {
            return Combine((leftVal, rightVal) => leftVal, dictionaries);
        }
    }
}
