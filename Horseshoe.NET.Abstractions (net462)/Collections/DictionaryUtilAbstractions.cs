using System.Collections.Generic;

namespace Horseshoe.NET.Collections
{
    /// <summary>
    /// A collection of utility methods for <c>IDictionary</c>.
    /// </summary>
    public static class DictionaryUtilAbstractions
    {
        /// <summary>
        /// Gets the value of a dictionary entry if it exists, otherwise <c>default</c> (e.g. <c>null</c>).
        /// </summary>
        /// <typeparam name="TKey">Type of key.</typeparam>
        /// <typeparam name="TValue">Type of value.</typeparam>
        /// <param name="dictionary">A dictionary.</param>
        /// <param name="key">A key.</param>
        public static TValue ValueOrDefault<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary.TryGetValue(key, out TValue value))
            {
                return value;
            }
            return default;
        }

        /// <summary>
        /// Adds or updates a value in a dictionary.
        /// </summary>
        /// <typeparam name="TKey">Type of key.</typeparam>
        /// <typeparam name="TValue">Type of value.</typeparam>
        /// <param name="dictionary">A dictionary.</param>
        /// <param name="key">A key.</param>
        /// <param name="value">A value.</param>
        public static void AddOrReplace<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            // replace
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
            }
            // add
            else
            {
                dictionary.Add(key, value);
            }
        }

        /// <summary>
        /// Adds or updates a value in a dictionary or removes the entry if it exists and the new value is <c>null</c>.
        /// </summary>
        /// <typeparam name="TKey">Type of key.</typeparam>
        /// <typeparam name="TValue">Type of value.</typeparam>
        /// <param name="dictionary">A dictionary.</param>
        /// <param name="key">A key.</param>
        /// <param name="value">A value.</param>
        public static void AddRemoveOrReplace<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            // remove
            if (typeof(TValue).IsClass && value == null)
            {
                if (dictionary.ContainsKey(key))
                {
                    dictionary.Remove(key);
                }
            }
            // replace
            else if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
            }
            // add
            else
            {
                dictionary.Add(key, value);
            }
        }
    }
}
