using System;
using System.Collections;
using System.Collections.Generic;

namespace Horseshoe.NET.Collections
{
    /// <summary>
    /// An <c>IDictionary</c> whose alter methods have been disabled with <see cref="NotImplementedException"/>
    /// </summary>
    /// <typeparam name="TKey">key type</typeparam>
    /// <typeparam name="TValue">value type</typeparam>
    public class ImmutableDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        readonly IDictionary<TKey, TValue> data;

        /// <summary>
        /// Create new <c>ImmutableDictionary</c>
        /// </summary>
        public ImmutableDictionary()
        {
            data = new Dictionary<TKey, TValue>();
        }

        /// <summary>
        /// Create new <c>ImmutableDictionary</c>
        /// </summary>
        /// <param name="capacity">initial capacity</param>
        public ImmutableDictionary(int capacity)
        {
            data = new Dictionary<TKey, TValue>(capacity);
        }

        /// <summary>
        /// Create new <c>ImmutableDictionary</c>
        /// </summary>
        /// <param name="comparer">An equality comparer</param>
        public ImmutableDictionary(IEqualityComparer<TKey> comparer)
        {
            data = new Dictionary<TKey, TValue>(comparer);
        }

        /// <summary>
        /// Create new <c>ImmutableDictionary</c> from an existing dictionary
        /// </summary>
        /// <param name="dictionary">a dictionary</param>
        public ImmutableDictionary(IDictionary<TKey, TValue> dictionary)
        {
            data = new Dictionary<TKey, TValue>(dictionary);
        }

        /// <summary>
        /// Create new <c>ImmutableDictionary</c>
        /// </summary>
        /// <param name="capacity">initial capacity</param>
        /// <param name="comparer">An equality comparer</param>
        public ImmutableDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            data = new Dictionary<TKey, TValue>(capacity, comparer);
        }

        /// <summary>
        /// Create new <c>ImmutableDictionary</c> from an existing dictionary
        /// </summary>
        /// <param name="dictionary">a dictionary</param>
        /// <param name="comparer">An equality comparer</param>
        public ImmutableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
        {
            data = new Dictionary<TKey, TValue>(dictionary, comparer);
        }

        /// <summary>
        /// Indexer for this <c>ImmutableDictionary</c>
        /// </summary>
        /// <param name="key"></param>
        /// <returns>The value corresponding to the supplied key</returns>
        /// <exception cref="NotImplementedException"></exception>
        public TValue this[TKey key] { get => data[key]; set => throw new NotImplementedException("This dictionary is immutable"); }

        /// <summary>
        /// The collection of keys for this <c>ImmutableDictionary</c>
        /// </summary>
        public ICollection<TKey> Keys => data.Keys;

        /// <summary>
        /// The collection of values for this <c>ImmutableDictionary</c>
        /// </summary>
        public ICollection<TValue> Values => data.Values;

        /// <summary>
        /// The number of elements
        /// </summary>
        public int Count => data.Count;

        /// <summary>
        /// Whether this <c>ImmutableDictionary</c> is read-only (<c>true</c>)
        /// </summary>
        public bool IsReadOnly => true;

        /// <summary>
        /// Disabled.  Adds an item to the <c>IDictionary</c>.
        /// </summary>
        /// <param name="key">a key</param>
        /// <param name="value">a value</param>
        /// <exception cref="NotImplementedException"></exception>
        public void Add(TKey key, TValue value) => throw new NotImplementedException("This dictionary is immutable");

        /// <summary>
        /// Disabled.  Adds an item to the <c>IDictionary</c>.
        /// </summary>
        /// <param name="item">a <c>KeyValuePair</c></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Add(KeyValuePair<TKey, TValue> item) => throw new NotImplementedException("This dictionary is immutable");

        /// <summary>
        /// Disabled.  Removes all key/vlaue mappings from this <c>IDictionary</c>.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Clear() => throw new NotImplementedException("This dictionary is immutable");

        /// <summary>
        /// Returns whether this <c>IDictionary</c> contains a specific <c>KeyValuePair</c>
        /// </summary>
        /// <param name="item">a <c>KeyValuePair</c></param>
        /// <returns></returns>
        public bool Contains(KeyValuePair<TKey, TValue> item) => data.Contains(item);

        /// <summary>
        /// Returns whether this <c>IDictionary</c> contains a specific key
        /// </summary>
        /// <param name="key">a key</param>
        /// <returns></returns>
        public bool ContainsKey(TKey key) => data.ContainsKey(key);

        /// <summary>
        /// Copies the elements of this <c>IDictionary</c> to a <c>KeyValuePair[]</c>
        /// </summary>
        /// <param name="array">an array</param>
        /// <param name="arrayIndex">the 0-based index in <c>array</c> where copying begins</param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => data.CopyTo(array, arrayIndex);

        /// <summary>
        /// Retuns and enumerator that iterates through this <c>ImmutableDictionary</c>
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => new Dictionary<TKey, TValue>(data).GetEnumerator();

        /// <summary>
        /// Disabled.  Removes the element with the specified key from this <c>IDictionary</c>.
        /// </summary>
        /// <param name="key">a key</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool Remove(TKey key) => throw new NotImplementedException("This dictionary is immutable");

        /// <summary>
        /// Disabled.  Removed the supplied <c>KeyValuePair</c> from this <c>IDictionary</c>.
        /// </summary>
        /// <param name="item">a <c>KeyValuePair</c></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool Remove(KeyValuePair<TKey, TValue> item) => throw new NotImplementedException("This dictionary is immutable");

        /// <summary>
        /// Gets the value associated with the specified key
        /// </summary>
        /// <param name="key">a key</param>
        /// <param name="value">a velue</param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, out TValue value) => data.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
