using System.Collections.Generic;
using System.Linq;

using Horseshoe.NET.Text.TextGrid;

namespace Horseshoe.NET.Collections
{
    /// <summary>
    /// A collection of <c>IDictionary</c> extension methods for <c>Collection</c>
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <inheritdoc cref="DictionaryUtil.Extract{TKey, TValue}(IDictionary{TKey, TValue}, TKey, out TValue)"/>
        public static bool Extract<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, out TValue item) =>
            DictionaryUtil.Extract(dictionary, key, out item);

        /// <inheritdoc cref="DictionaryUtil.Append{TKey, TValue}(IDictionary{TKey, TValue}, IEnumerable{IEnumerable{KeyValuePair{TKey, TValue}}}, MergeOptions)"/>
        public static IDictionary<TKey, TValue> Append<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<KeyValuePair<TKey, TValue>>[] mappingCollectionsToAppend, MergeOptions options = null) =>
            DictionaryUtil.Append(dictionary, mappingCollectionsToAppend, options: options);

        /// <inheritdoc cref="DictionaryUtil.AppendLTR{TKey, TValue}(IDictionary{TKey, TValue}, IEnumerable{KeyValuePair{TKey, TValue}}[])"/>
        public static IDictionary<TKey, TValue> AppendLTR<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, params IEnumerable<KeyValuePair<TKey, TValue>>[] mappingCollectionsToAppend) =>
            DictionaryUtil.AppendLTR(dictionary, mappingCollectionsToAppend);

        /// <inheritdoc cref="DictionaryUtil.AppendRTL{TKey, TValue}(IDictionary{TKey, TValue}, IEnumerable{KeyValuePair{TKey, TValue}}[])"/>
        public static IDictionary<TKey, TValue> AppendRTL<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, params IEnumerable<KeyValuePair<TKey, TValue>>[] mappingCollectionsToAppend) =>
            DictionaryUtil.AppendRTL(dictionary, mappingCollectionsToAppend);

        /// <inheritdoc cref="DictionaryUtil.Append{TKey, TValue}(IDictionary{TKey, TValue}, IEnumerable{KeyValuePair{TKey, TValue}}, MergeOptions)"/>
        public static IDictionary<TKey, TValue> Append<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<KeyValuePair<TKey, TValue>> mappingsToAppend, MergeOptions options = null) =>
            DictionaryUtil.Append(dictionary, mappingsToAppend, options: options);

        /// <inheritdoc cref="DictionaryUtil.AppendLTR{TKey, TValue}(IDictionary{TKey, TValue}, IEnumerable{KeyValuePair{TKey, TValue}})"/>
        public static IDictionary<TKey, TValue> AppendLTR<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<KeyValuePair<TKey, TValue>> mappingsToAppend) =>
            DictionaryUtil.AppendLTR(dictionary, mappingsToAppend);

        /// <inheritdoc cref="DictionaryUtil.AppendRTL{TKey, TValue}(IDictionary{TKey, TValue}, IEnumerable{KeyValuePair{TKey, TValue}})"/>
        public static IDictionary<TKey, TValue> AppendRTL<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<KeyValuePair<TKey, TValue>> mappingsToAppend) =>
            DictionaryUtil.AppendRTL(dictionary, mappingsToAppend);

        /// <inheritdoc cref="DictionaryUtil.Append{TKey, TValue}(IDictionary{TKey, TValue}, KeyValuePair{TKey, TValue}, MergeOptions)"/>
        public static IDictionary<TKey, TValue> Append<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, KeyValuePair<TKey, TValue> mappingToAppend, MergeOptions options = null) =>
            DictionaryUtil.Append(dictionary, mappingToAppend, options: options);

        /// <inheritdoc cref="DictionaryUtil.AppendLTR{TKey, TValue}(IDictionary{TKey, TValue}, KeyValuePair{TKey, TValue})"/>
        public static IDictionary<TKey, TValue> AppendLTR<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, KeyValuePair<TKey, TValue> mappingToAppend) =>
            DictionaryUtil.AppendLTR(dictionary, mappingToAppend);

        /// <inheritdoc cref="DictionaryUtil.AppendRTL{TKey, TValue}(IDictionary{TKey, TValue}, KeyValuePair{TKey, TValue})"/>
        public static IDictionary<TKey, TValue> AppendRTL<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, KeyValuePair<TKey, TValue> mappingToAppend) =>
            DictionaryUtil.AppendRTL(dictionary, mappingToAppend);

        /// <inheritdoc cref="DictionaryUtil.Append{TKey, TValue}(IDictionary{TKey, TValue}, TKey, TValue, MergeOptions)"/>
        public static IDictionary<TKey, TValue> Append<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value, MergeOptions options = null) =>
            DictionaryUtil.Append(dictionary, key, value, options: options);

        /// <inheritdoc cref="DictionaryUtil.AppendLTR{TKey, TValue}(IDictionary{TKey, TValue}, TKey, TValue)"/>
        public static IDictionary<TKey, TValue> AppendLTR<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value) =>
            DictionaryUtil.AppendLTR(dictionary, key, value);

        /// <inheritdoc cref="DictionaryUtil.AppendRTL{TKey, TValue}(IDictionary{TKey, TValue}, TKey, TValue)"/>
        public static IDictionary<TKey, TValue> AppendRTL<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value) =>
            DictionaryUtil.AppendRTL(dictionary, key, value);

        /// <summary>
        /// Conditionally appends additional mappings to a dictionary.  Duplicate keys will be merged left-to-right i.e. ignored. 
        /// To change this behavior use <see cref="Append{TKey, TValue}(IDictionary{TKey, TValue}, IEnumerable{KeyValuePair{TKey, TValue}}[], MergeOptions)"/>.
        /// Returns original dictionary as long as it wasn't <c>null</c>.
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">A dictionary</param>
        /// <param name="condition">A condition</param>
        /// <param name="mappingCollectionsToAppend">Dictionaries and/or other mapping collections to append</param>
        /// <returns>The original dictionary as long as it wasn't <c>null</c>.</returns>
        public static IDictionary<TKey, TValue> AppendIfLTR<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, bool condition, params IEnumerable<KeyValuePair<TKey, TValue>>[] mappingCollectionsToAppend)
        {
            if (condition) 
                return DictionaryUtil.AppendLTR(dictionary, mappingCollectionsToAppend);
            return dictionary;
        }

        /// <summary>
        /// Conditionally appends additional mappings to a dictionary.  Duplicate keys will be merged right-to-keft i.e. overwritten. 
        /// To change this behavior use <see cref="Append{TKey, TValue}(IDictionary{TKey, TValue}, IEnumerable{KeyValuePair{TKey, TValue}}[], MergeOptions)"/>.
        /// Returns original dictionary as long as it wasn't <c>null</c>.
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">A dictionary</param>
        /// <param name="condition">A condition</param>
        /// <param name="mappingCollectionsToAppend">Dictionaries and/or other mapping collections to append</param>
        /// <returns>The original dictionary as long as it wasn't <c>null</c>.</returns>
        public static IDictionary<TKey, TValue> AppendIfRTL<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, bool condition, params IEnumerable<KeyValuePair<TKey, TValue>>[] mappingCollectionsToAppend)
        {
            if (condition)
                DictionaryUtil.AppendRTL(dictionary, mappingCollectionsToAppend);
            return dictionary;
        }

        /// <summary>
        /// Conditionally appends additional mappings to a dictionary.  Duplicate keys will be merged left-to-right i.e. ignored. 
        /// To change this behavior use <see cref="Append{TKey, TValue}(IDictionary{TKey, TValue}, IEnumerable{KeyValuePair{TKey, TValue}}, MergeOptions)"/>.
        /// Returns original dictionary as long as it wasn't <c>null</c>.
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">A dictionary</param>
        /// <param name="condition">A condition</param>
        /// <param name="mappingsToAppend">Dictionary or other mapping collection to append</param>
        /// <returns>The original dictionary as long as it wasn't <c>null</c>.</returns>
        public static IDictionary<TKey, TValue> AppendIfLTR<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, bool condition, IEnumerable<KeyValuePair<TKey, TValue>> mappingsToAppend)
        {
            if (condition)
                return DictionaryUtil.AppendLTR(dictionary, mappingsToAppend);
            return dictionary;
        }

        /// <summary>
        /// Conditionally appends additional mappings to a dictionary.  Duplicate keys will be merged right-to-keft i.e. overwritten. 
        /// To change this behavior use <see cref="Append{TKey, TValue}(IDictionary{TKey, TValue}, IEnumerable{KeyValuePair{TKey, TValue}}, MergeOptions)"/>.
        /// Returns original dictionary as long as it wasn't <c>null</c>.
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">A dictionary</param>
        /// <param name="condition">A condition</param>
        /// <param name="mappingsToAppend">Dictionary or other mapping collection to append</param>
        /// <returns>The original dictionary as long as it wasn't <c>null</c>.</returns>
        public static IDictionary<TKey, TValue> AppendIfRTL<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, bool condition, IEnumerable<KeyValuePair<TKey, TValue>> mappingsToAppend)
        {
            if (condition)
                return DictionaryUtil.AppendRTL(dictionary, mappingsToAppend);
            return dictionary;
        }

        /// <summary>
        /// Conditionally appends an additional mapping to a dictionary.  Duplicate keys will be merged left-to-right i.e. ignored. 
        /// To change this behavior use <see cref="Append{TKey, TValue}(IDictionary{TKey, TValue}, KeyValuePair{TKey, TValue}, MergeOptions)"/>.
        /// Returns original dictionary as long as it wasn't <c>null</c>.
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">A dictionary</param>
        /// <param name="condition">A condition</param>
        /// <param name="mappingToAppend">A key/value pair to append</param>
        /// <returns>The original dictionary as long as it wasn't <c>null</c>.</returns>
        public static IDictionary<TKey, TValue> AppendIfLTR<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, bool condition, KeyValuePair<TKey, TValue> mappingToAppend)
        {
            if (condition)
                return DictionaryUtil.AppendLTR(dictionary, mappingToAppend);
            return dictionary;
        }

        /// <summary>
        /// Conditionally appends an additional mapping to a dictionary.  Duplicate keys will be merged right-to-keft i.e. overwritten. 
        /// To change this behavior use <see cref="Append{TKey, TValue}(IDictionary{TKey, TValue}, KeyValuePair{TKey, TValue}, MergeOptions)"/>.
        /// Returns original dictionary as long as it wasn't <c>null</c>.
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">A dictionary</param>
        /// <param name="condition">A condition</param>
        /// <param name="mappingToAppend">A key/value pair to append</param>
        /// <returns>The original dictionary as long as it wasn't <c>null</c>.</returns>
        public static IDictionary<TKey, TValue> AppendIfRTL<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, bool condition, KeyValuePair<TKey, TValue> mappingToAppend)
        {
            if (condition)
                return DictionaryUtil.AppendRTL(dictionary, mappingToAppend);
            return dictionary;
        }

        /// <summary>
        /// Conditionally appends an additional mapping to a dictionary.  Duplicate keys will be merged left-to-right i.e. ignored. 
        /// To change this behavior use <see cref="Append{TKey, TValue}(IDictionary{TKey, TValue}, TKey, TValue, MergeOptions)"/>.
        /// Returns original dictionary as long as it wasn't <c>null</c>.
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">A dictionary</param>
        /// <param name="condition">A condition</param>
        /// <param name="key">A key to append</param>
        /// <param name="value">A value to append</param>
        /// <returns>The original dictionary as long as it wasn't <c>null</c>.</returns>
        public static IDictionary<TKey, TValue> AppendIfLTR<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, bool condition, TKey key, TValue value)
        {
            if (condition)
                return DictionaryUtil.AppendLTR(dictionary, key, value);
            return dictionary;
        }

        /// <summary>
        /// Conditionally appends an additional mapping to a dictionary.  Duplicate keys will be merged right-to-keft i.e. overwritten. 
        /// To change this behavior use <see cref="Append{TKey, TValue}(IDictionary{TKey, TValue}, TKey, TValue, MergeOptions)"/>.
        /// Returns original dictionary as long as it wasn't <c>null</c>.
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">A dictionary</param>
        /// <param name="condition">A condition</param>
        /// <param name="key">A key to append</param>
        /// <param name="value">A value to append</param>
        /// <returns>The original dictionary as long as it wasn't <c>null</c>.</returns>
        public static IDictionary<TKey, TValue> AppendIfRTL<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, bool condition, TKey key, TValue value)
        {
            if (condition)
                return DictionaryUtil.AppendRTL(dictionary, key, value);
            return dictionary;
        }

        /// <summary>
        /// Conditionally appends an additional mapping to a dictionary.  Duplicate keys will be merged left-to-right i.e. ignored. 
        /// To change this behavior use <see cref="Append{TKey, TValue}(IDictionary{TKey, TValue}, TKey, TValue, MergeOptions)"/>.
        /// Returns original dictionary as long as it wasn't <c>null</c>.
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">A dictionary</param>
        /// <param name="key">A key to append</param>
        /// <param name="value">A value to append</param>
        /// <returns>The original dictionary as long as it wasn't <c>null</c>.</returns>
        public static IDictionary<TKey, TValue> AppendIfNotNullLTR<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (value != null)
                return DictionaryUtil.AppendLTR(dictionary, key, value);
            return dictionary;
        }

        /// <summary>
        /// Conditionally appends an additional mapping to a dictionary.  Duplicate keys will be merged right-to-keft i.e. overwritten. 
        /// To change this behavior use <see cref="Append{TKey, TValue}(IDictionary{TKey, TValue}, TKey, TValue, MergeOptions)"/>.
        /// Returns original dictionary as long as it wasn't <c>null</c>.
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">A dictionary</param>
        /// <param name="key">A key to append</param>
        /// <param name="value">A value to append</param>
        /// <returns>The original dictionary as long as it wasn't <c>null</c>.</returns>
        public static IDictionary<TKey, TValue> AppendIfNotNullRTL<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (value != null)
                return DictionaryUtil.AppendRTL(dictionary, key, value);
            return dictionary;
        }

        /// <summary>
        /// Adds a value to a dictionary only if the key does not already exist
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">A dictionary</param>
        /// <param name="key">A key</param>
        /// <param name="value">A value</param>
        public static void AddIfUnique<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, value);
            }
        }

        /// <inheritdoc cref="DictionaryUtil.ValueOrDefault{TKey, TValue}(IDictionary{TKey, TValue}, TKey)"/>
        public static TValue ValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            return DictionaryUtil.ValueOrDefault(dictionary, key);
        }

        /// <inheritdoc cref="DictionaryUtil.AddOrReplace{TKey, TValue}(IDictionary{TKey, TValue}, TKey, TValue)"/>
        public static void AddOrReplace<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            DictionaryUtil.AddOrReplace(dictionary, key, value);
        }

        /// <inheritdoc cref="DictionaryUtil.AddRemoveOrReplace{TKey, TValue}(IDictionary{TKey, TValue}, TKey, TValue)"/>
        public static void AddRemoveOrReplace<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            DictionaryUtil.AddRemoveOrReplace(dictionary, key, value);
        }

        /// <summary>
        /// Creates an immutable version of a dictionary if not already immutable
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="mappings">A dictionary</param>
        /// <returns>An immutable dictionary with the provided mappings</returns>
        public static ImmutableDictionary<TKey, TValue> AsImmutable<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> mappings)
        {
            return mappings is ImmutableDictionary<TKey, TValue> immutableDictionary
                ? immutableDictionary
                : DictionaryUtil.ImmutableFrom<TKey, TValue>(mappings);
        }

        /// <summary>
        /// Renders this dictionary to text
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="dict">A dictionary.</param>
        /// <param name="itemSeparator">The item separator.</param>
        /// <param name="keyValueSeparator">The key/value separator.</param>
        /// <returns>A string representation of dictionary data.</returns>
        public static string Dump<TKey, TValue>(this IDictionary<TKey, TValue> dict, string itemSeparator = "; ", string keyValueSeparator = ": ")
        {
            return dict == null 
                ? string.Empty
                : string.Join(itemSeparator, dict.Select(kvp => kvp.Key + keyValueSeparator + kvp.Value));
        }

        /// <summary>
        /// Displays a dictionary's contents as a string
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">A dictionary</param>
        /// <param name="equals">Equality operator</param>
        /// <param name="separator">Separator</param>
        /// <returns></returns>
        public static string StringDump<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, string equals = " = ", string separator = ", ")
        {
            return string.Join(separator, dictionary.Select(kvp => kvp.Key + equals + kvp.Value));
        }

        /// <summary>
        /// Displays a dictionary's contents as a grid
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">A dictionary</param>
        /// <param name="borderPolicy">The border policy</param>
        /// <returns></returns>
        public static string StringDumpToGrid<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, BorderPolicy borderPolicy = default)
        {
            var textGrid = new TextGrid { BorderPolicy = borderPolicy };
            var col1 = new Column { Title = typeof(TKey).Name };
            var col2 = new Column { Title = typeof(TValue).Name };
            textGrid.AddColumns(col1, col2);
            foreach (var kvp in dictionary)
            {
                col1.Add(kvp.Key);
                col2.Add(kvp.Value);
            }
            return textGrid.Render();
        }

        /// <summary>
        /// Copies this dictionary to a <c>TextGrid</c>
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="dict">A dictionary.</param>
        /// <returns>A text grid for displaying dictionary data.</returns>
        public static TextGrid DumpToTextGrid<TKey, TValue>(this IDictionary<TKey, TValue> dict)
        {
            return dict == null
                ? new TextGrid()
                : TextGrid.FromDictionary(dict);
        }
    }
}
