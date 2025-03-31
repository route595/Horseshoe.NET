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

        /// <summary>
        /// Returns a new dictionary containing the provided mappings.  If optimizing and argument is 
        /// already a dictionary then that is what is returned. RTL merging by default, i.e. the last 
        /// duplicate value overwites any previous ones.  To change this behavior
        /// see <c>options</c> (<see cref="MergeOptions"/>).
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="mappings">A key/value pair collection</param>
        /// <param name="options">Dictionary merge options</param>
        /// <param name="optimize">If <c>ReuseCollection</c> and <c>mappings</c> is alredy of type <c>ImmutableDictionary&lt;TKey,TValue&gt;</c>, returns the original dictionary. Default is <c>None</c>.</param>
        /// <returns>A new dictionary</returns>
        public static Dictionary<TKey, TValue> From<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> mappings, MergeOptions options = null, Optimization optimize = default)
        {
            if (mappings is Dictionary<TKey, TValue> _dict && (optimize & Optimization.ReuseCollection) == Optimization.ReuseCollection)
                return _dict;
            var dict = new Dictionary<TKey, TValue>();
            TValue value;
            foreach (var kvp in mappings)
            {
                // use new value includes lef
                value = kvp.Value;
                if (dict.ContainsKey(kvp.Key))
                {
                    // choose (or manipulate) value based on client logic
                    if (options is MergeOptions<TKey, TValue> genericOptions && genericOptions.CustomMerge != null)
                        value = genericOptions.CustomMerge(kvp.Key, dict[kvp.Key], kvp.Value, dict, mappings);
                    // keep old value (a.k.a. left-to-right merging)
                    else if (options is MergeOptions mergeOptions && mergeOptions.Mode == DictionaryMergeMode.LTR)
                        continue;
                }
                dict[kvp.Key] = value;
            }
            return dict;
        }

        /// <summary>
        /// Builds a new dictionary from a single mapping.
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="mapping">A key/value pair</param>
        /// <returns>A new dictionary</returns>
        public static Dictionary<TKey, TValue> From<TKey, TValue>(KeyValuePair<TKey, TValue> mapping)
        {
            return From(mapping.Key, mapping.Value);
        }

        /// <summary>
        /// Builds a new dictionary from a single mapping.
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="key">A key</param>
        /// <param name="value">A value</param>
        /// <returns>A new dictionary</returns>
        public static Dictionary<TKey, TValue> From<TKey, TValue>(TKey key, TValue value)
        {
            return new Dictionary<TKey, TValue> { { key, value } };
        }

        /// <summary>
        /// Returns a new immutable dictionary containing the provided mappings.  If optimizing and argument is 
        /// already an immutable dictionary then that is what is returned. RTL merging by default, i.e. the last 
        /// duplicate value overwites any previous ones.  To change this behavior
        /// see <c>options</c> (<see cref="MergeOptions"/>).
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="mappings">A key/value pair collection</param>
        /// <param name="options">Dictionary merge options</param>
        /// <param name="optimize">If <c>ReuseCollection</c> and <c>mappings</c> is alredy of type <c>ImmutableDictionary&lt;TKey,TValue&gt;</c>, returns the original dictionary. Default is <c>None</c>.</param>
        /// <returns>A new dictionary</returns>
        public static ImmutableDictionary<TKey, TValue> ImmutableFrom<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> mappings, MergeOptions options = null, Optimization optimize = default)
        {
            if (mappings is ImmutableDictionary<TKey, TValue> iDict && (optimize & Optimization.ReuseCollection) == Optimization.ReuseCollection)
                return iDict;
            IDictionary<TKey, TValue> dict;
            if (mappings is Dictionary<TKey, TValue> _dict && (optimize & Optimization.ReuseCollection) == Optimization.ReuseCollection)
            {
                dict = _dict;
            }
            else
            {
                dict = From(mappings, options: options);
            }
            return new ImmutableDictionary<TKey, TValue>(dict);
        }

        /// <summary>
        /// Gets the value associated with the specified key
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dict">A dictionary</param>
        /// <param name="key">A key </param>
        /// <param name="value">A value (out)</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool TryGetValue<TKey, TValue>(IDictionary<TKey, TValue> dict, TKey key, out TValue value)
        {
            if (dict == null)
            {
                value = default;
                return false;
            }

            return dict.TryGetValue(key, out value);
        }

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
        /// Appends additional mappings to a dictionary.  By default, a duplicate key causes an exception. 
        /// To change this behavior see <see cref="MergeOptions"/>.
        /// Returns original dictionary as long as it wasn't <c>null</c>.
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">A dictionary</param>
        /// <param name="mappingCollectionsToAppend">Dictionaries and/or other mapping collections to append</param>
        /// <param name="options">Dictionary merge options</param>
        /// <returns>The original dictionary as long as it wasn't <c>null</c>.</returns>
        public static IDictionary<TKey, TValue> Append<TKey, TValue>(IDictionary<TKey, TValue> dictionary, IEnumerable<IEnumerable<KeyValuePair<TKey, TValue>>> mappingCollectionsToAppend, MergeOptions options = null)
        {
            if (dictionary == null)
                dictionary = new Dictionary<TKey, TValue>();

            var _mappingCollectionsToAppend = ListUtil.Prune(mappingCollectionsToAppend); // eliminate null collections

            options = options ?? new MergeOptions();

            foreach (var mappingsToAppend in _mappingCollectionsToAppend)
            {
                foreach (var kvp in mappingsToAppend)
                {
                    AppendMapping(dictionary, kvp, /*mappingsToAppend,*/ options);
                }
            }
            return dictionary;
        }

        private static void AppendMapping<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key, TValue value, /*IEnumerable<KeyValuePair<TKey, TValue>> mappingsToAppend,*/ MergeOptions options)
        {
            if (dictionary.ContainsKey(key))
            {
                options = options ?? new MergeOptions();
                if (options is MergeOptions<TKey, TValue> genericOptions && genericOptions.CustomMerge != null)
                {
                    dictionary[key] = genericOptions.CustomMerge.Invoke(key, dictionary[key], value, dictionary, null);
                    return;
                }
                switch (options.Mode)
                {
                    case DictionaryMergeMode.RTL:
                        dictionary[key] = value;
                        return;
                    case DictionaryMergeMode.LTR:
                        return;
                    case DictionaryMergeMode.Error:
                        break;
                }
            }
            dictionary.Add(key, value);
        }

        private static void AppendMapping<TKey, TValue>(IDictionary<TKey, TValue> dictionary, KeyValuePair<TKey, TValue> mapping, /*IEnumerable<KeyValuePair<TKey, TValue>> mappingsToAppend,*/ MergeOptions options)
        {
            AppendMapping(dictionary, mapping.Key, mapping.Value, /*mappingsToAppend,*/ options);
        }

        /// <summary>
        /// Appends additional mappings to a dictionary.  Duplicate keys will be merged left-to-right i.e. ignored. 
        /// To change this behavior use <see cref="Append{TKey, TValue}(IDictionary{TKey, TValue}, IEnumerable{IEnumerable{KeyValuePair{TKey, TValue}}}, MergeOptions)"/>.
        /// Returns original dictionary as long as it wasn't <c>null</c>.
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">A dictionary</param>
        /// <param name="mappingCollectionsToAppend">Dictionaries and/or other mapping collections to append</param>
        /// <returns>The original dictionary as long as it wasn't <c>null</c>.</returns>
        public static IDictionary<TKey, TValue> AppendLTR<TKey, TValue>(IDictionary<TKey, TValue> dictionary, params IEnumerable<KeyValuePair<TKey, TValue>>[] mappingCollectionsToAppend) =>
            Append(dictionary, mappingCollectionsToAppend, options: new MergeOptions { Mode = DictionaryMergeMode.LTR });

        /// <summary>
        /// Appends additional mappings to a dictionary.  Duplicate keys will be merged right-to-keft i.e. overwritten. 
        /// To change this behavior use <see cref="Append{TKey, TValue}(IDictionary{TKey, TValue}, IEnumerable{IEnumerable{KeyValuePair{TKey, TValue}}}, MergeOptions)"/>.
        /// Returns original dictionary as long as it wasn't <c>null</c>.
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">A dictionary</param>
        /// <param name="mappingCollectionsToAppend">Dictionaries and/or other mapping collections to append</param>
        /// <returns>The original dictionary as long as it wasn't <c>null</c>.</returns>
        public static IDictionary<TKey, TValue> AppendRTL<TKey, TValue>(IDictionary<TKey, TValue> dictionary, params IEnumerable<KeyValuePair<TKey, TValue>>[] mappingCollectionsToAppend) =>
            Append(dictionary, mappingCollectionsToAppend, options: new MergeOptions { Mode = DictionaryMergeMode.RTL });

        /// <summary>
        /// Appends additional mappings to a dictionary.  By default, a duplicate key causes an exception. 
        /// To change this behavior see <see cref="MergeOptions"/>.
        /// Returns original dictionary as long as it wasn't <c>null</c>.
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">A dictionary</param>
        /// <param name="mappingsToAppend">Dictionary or other mapping collection to append</param>
        /// <param name="options">Dictionary merge options</param>
        /// <returns>The original dictionary as long as it wasn't <c>null</c>.</returns>
        public static IDictionary<TKey, TValue> Append<TKey, TValue>(IDictionary<TKey, TValue> dictionary, IEnumerable<KeyValuePair<TKey, TValue>> mappingsToAppend, MergeOptions options = null) =>
            Append(dictionary, new[] { mappingsToAppend }, options: options);

        /// <summary>
        /// Appends additional mappings to a dictionary.  Duplicate keys will be merged left-to-right i.e. ignored. 
        /// To change this behavior use <see cref="Append{TKey, TValue}(IDictionary{TKey, TValue}, IEnumerable{KeyValuePair{TKey, TValue}}, MergeOptions)"/>.
        /// Returns original dictionary as long as it wasn't <c>null</c>.
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">A dictionary</param>
        /// <param name="mappingsToAppend">Dictionary or other mapping collection to append</param>
        /// <returns>The original dictionary as long as it wasn't <c>null</c>.</returns>
        public static IDictionary<TKey, TValue> AppendLTR<TKey, TValue>(IDictionary<TKey, TValue> dictionary, IEnumerable<KeyValuePair<TKey, TValue>> mappingsToAppend) =>
            Append(dictionary, mappingsToAppend, options: new MergeOptions { Mode = DictionaryMergeMode.LTR });

        /// <summary>
        /// Appends additional mappings to a dictionary.  Duplicate keys will be merged right-to-keft i.e. overwritten. 
        /// To change this behavior use <see cref="Append{TKey, TValue}(IDictionary{TKey, TValue}, IEnumerable{KeyValuePair{TKey, TValue}}, MergeOptions)"/>.
        /// Returns original dictionary as long as it wasn't <c>null</c>.
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">A dictionary</param>
        /// <param name="mappingsToAppend">Dictionary or other mapping collection to append</param>
        /// <returns>The original dictionary as long as it wasn't <c>null</c>.</returns>
        public static IDictionary<TKey, TValue> AppendRTL<TKey, TValue>(IDictionary<TKey, TValue> dictionary, IEnumerable<KeyValuePair<TKey, TValue>> mappingsToAppend) =>
            Append(dictionary, mappingsToAppend, options: new MergeOptions { Mode = DictionaryMergeMode.RTL });

        /// <summary>
        /// Appends an additional mapping to a dictionary.  By default, a duplicate key causes an exception. 
        /// To change this behavior see <see cref="MergeOptions"/>.
        /// Returns original dictionary as long as it wasn't <c>null</c>.
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">A dictionary</param>
        /// <param name="mappingToAppend">A key and value to append</param>
        /// <param name="options">Dictionary merge options</param>
        /// <returns>The original dictionary as long as it wasn't <c>null</c>.</returns>
        public static IDictionary<TKey, TValue> Append<TKey, TValue>(IDictionary<TKey, TValue> dictionary, KeyValuePair<TKey, TValue> mappingToAppend, MergeOptions options = null)
        {
            if (dictionary == null)
                dictionary = new Dictionary<TKey, TValue>();

            AppendMapping(dictionary, mappingToAppend, /*null,*/ options);

            return dictionary;
        }

        /// <summary>
        /// Appends an additional mapping to a dictionary.  Duplicate keys will be merged left-to-right i.e. ignored. 
        /// To change this behavior use <see cref="Append{TKey, TValue}(IDictionary{TKey, TValue}, KeyValuePair{TKey, TValue}, MergeOptions)"/>.
        /// Returns original dictionary as long as it wasn't <c>null</c>.
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">A dictionary</param>
        /// <param name="mappingToAppend">A key/value pair to append</param>
        /// <returns>The original dictionary as long as it wasn't <c>null</c>.</returns>
        public static IDictionary<TKey, TValue> AppendLTR<TKey, TValue>(IDictionary<TKey, TValue> dictionary, KeyValuePair<TKey, TValue> mappingToAppend)
        {
            return Append(dictionary, mappingToAppend, options: new MergeOptions { Mode = DictionaryMergeMode.LTR });
        }

        /// <summary>
        /// Appends an additional mapping to a dictionary.  Duplicate keys will be merged right-to-keft i.e. overwritten. 
        /// To change this behavior use <see cref="Append{TKey, TValue}(IDictionary{TKey, TValue}, KeyValuePair{TKey, TValue}, MergeOptions)"/>.
        /// Returns original dictionary as long as it wasn't <c>null</c>.
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">A dictionary</param>
        /// <param name="mappingToAppend">A key/value pair to append</param>
        /// <returns>The original dictionary as long as it wasn't <c>null</c>.</returns>
        public static IDictionary<TKey, TValue> AppendRTL<TKey, TValue>(IDictionary<TKey, TValue> dictionary, KeyValuePair<TKey, TValue> mappingToAppend)
        {
            return Append(dictionary, mappingToAppend, options: new MergeOptions { Mode = DictionaryMergeMode.RTL });
        }

        /// <summary>
        /// Appends an additional mapping to a dictionary.  By default, a duplicate key causes an exception. 
        /// To change this behavior see <see cref="MergeOptions"/>.
        /// Returns original dictionary as long as it wasn't <c>null</c>.
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">A dictionary</param>
        /// <param name="key">A key to append</param>
        /// <param name="value">A value to append</param>
        /// <param name="options">Dictionary merge options</param>
        /// <returns>The original dictionary as long as it wasn't <c>null</c>.</returns>
        public static IDictionary<TKey, TValue> Append<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key, TValue value, MergeOptions options = null)
        {
            if (dictionary == null)
                dictionary = new Dictionary<TKey, TValue>();

            AppendMapping(dictionary, key, value, /*null,*/ options);

            return dictionary;
        }

        /// <summary>
        /// Appends an additional mapping to a dictionary.  Duplicate keys will be merged left-to-right i.e. ignored. 
        /// To change this behavior use <see cref="Append{TKey, TValue}(IDictionary{TKey, TValue}, TKey, TValue, MergeOptions)"/>.
        /// Returns original dictionary as long as it wasn't <c>null</c>.
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">A dictionary</param>
        /// <param name="key">A key to append</param>
        /// <param name="value">A value to append</param>
        /// <returns>The original dictionary as long as it wasn't <c>null</c>.</returns>
        public static IDictionary<TKey, TValue> AppendLTR<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            return Append(dictionary, key, value, options: new MergeOptions { Mode = DictionaryMergeMode.LTR });
        }

        /// <summary>
        /// Appends an additional mapping to a dictionary.  Duplicate keys will be merged right-to-keft i.e. overwritten. 
        /// To change this behavior use <see cref="Append{TKey, TValue}(IDictionary{TKey, TValue}, TKey, TValue, MergeOptions)"/>.
        /// Returns original dictionary as long as it wasn't <c>null</c>.
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">A dictionary</param>
        /// <param name="key">A key to append</param>
        /// <param name="value">A value to append</param>
        /// <returns>The original dictionary as long as it wasn't <c>null</c>.</returns>
        public static IDictionary<TKey, TValue> AppendRTL<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            return Append(dictionary, key, value, options: new MergeOptions { Mode = DictionaryMergeMode.RTL });
        }

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
        public static IDictionary<TKey, TValue> Combine<TKey, TValue>(IEnumerable<IEnumerable<KeyValuePair<TKey, TValue>>> dictionaries, MergeOptions options = null)
        {
            var _dictionaries = ListUtil.Prune(dictionaries); // eliminate null collections

            if (!_dictionaries.Any())
                return new Dictionary<TKey, TValue>();

            options = options ?? new MergeOptions();

            var dictionary = options.Optimize && _dictionaries.First() is IDictionary<TKey, TValue> idict
                ? idict
                : From(_dictionaries.First(), options: options);

            for (int i = 1, count = _dictionaries.Count(); i < count; i++)
            {
                foreach (var kvp in _dictionaries[i])
                {
                    AppendMapping(dictionary, kvp, /*_dictionaries[i],*/ options);
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
        public static IDictionary<TKey, TValue> CombineLTR<TKey, TValue>(params IEnumerable<KeyValuePair<TKey, TValue>>[] dictionaries) =>
            Combine(dictionaries, options: new MergeOptions { Mode = DictionaryMergeMode.LTR });

        /// <summary>
        /// Combines multiple dictionaries into one. Identical keys are merged left-to-right, i.e. table being appended to is not overwritten.  
        /// Returns a non-<c>null</c> dictionary, possibly with 0 elements.
        /// Optimized for improved memory managment, however, source dictionary could be altered before being returned by the method.
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionaries">Dictionaries to combine</param>
        /// <returns>A non-<c>null</c> dictionary, possibly with 0 elements</returns>
        public static IDictionary<TKey, TValue> CombineLTR_Optimized<TKey, TValue>(params IEnumerable<KeyValuePair<TKey, TValue>>[] dictionaries) =>
            Combine(dictionaries, options: new MergeOptions { Mode = DictionaryMergeMode.LTR, Optimize = true });

        /// <summary>
        /// Combines multiple dictionaries into one. Identical keys are merged right-to-left, i.e. table(s) being appended overwrite table being appended to.  
        /// Returns a non-<c>null</c> dictionary, possibly with 0 elements.
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionaries">Dictionaries to combine</param>
        /// <returns>A non-<c>null</c> dictionary, possibly with 0 elements</returns>
        public static IDictionary<TKey, TValue> CombineRTL<TKey, TValue>(params IEnumerable<KeyValuePair<TKey, TValue>>[] dictionaries) =>
            Combine(dictionaries, options: new MergeOptions { Mode = DictionaryMergeMode.RTL });

        /// <summary>
        /// Combines multiple dictionaries into one. Identical keys are merged right-to-left, i.e. table(s) being appended overwrite table being appended to.  
        /// Returns a non-<c>null</c> dictionary, possibly with 0 elements.
        /// Optimized for improved memory managment, however, source dictionary could be altered before being returned by the method.
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionaries">Dictionaries to combine</param>
        /// <returns>A non-<c>null</c> dictionary, possibly with 0 elements</returns>
        public static IDictionary<TKey, TValue> CombineRTL_Optimized<TKey, TValue>(params IEnumerable<KeyValuePair<TKey, TValue>>[] dictionaries) =>
            Combine(dictionaries, options: new MergeOptions { Mode = DictionaryMergeMode.RTL });
    }
}
