using System.Collections.Generic;

namespace Horseshoe.NET.Collections
{
    /// <summary>
    /// A collection of utility methods for <c>IDictionary</c>.
    /// </summary>
    public static class DictionaryUtilAbstractions
    {
        /// <summary>
        /// Adds a value to a dictionary, if the key already exist the previous value is replaced.
        /// </summary>
        /// <typeparam name="TKey">Type of key.</typeparam>
        /// <typeparam name="TValue">Type of value.</typeparam>
        /// <param name="dictionary">A dictionary.</param>
        /// <param name="key">A key.</param>
        /// <param name="value">A value.</param>
        public static void AddOrReplace<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
            }
            else
            {
                dictionary.Add(key, value);
            }
        }

    }
}
