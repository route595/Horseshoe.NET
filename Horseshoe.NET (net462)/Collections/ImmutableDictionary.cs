using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.Collections
{
    public class ImmutableDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        IDictionary<TKey, TValue> data;

        public ImmutableDictionary()
        {
            data = new Dictionary<TKey, TValue>();
        }

        public ImmutableDictionary(int capacity)
        {
            data = new Dictionary<TKey, TValue>(capacity);
        }

        public ImmutableDictionary(IEqualityComparer<TKey> comparer)
        {
            data = new Dictionary<TKey, TValue>(comparer);
        }

        public ImmutableDictionary(IDictionary<TKey, TValue> dictionary)
        {
            data = new Dictionary<TKey, TValue>(dictionary);
        }

        public ImmutableDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            data = new Dictionary<TKey, TValue>(capacity, comparer);
        }

        public ImmutableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
        {
            data = new Dictionary<TKey, TValue>(dictionary, comparer);
        }

        public TValue this[TKey key] { get => data[key]; set => throw new NotImplementedException("This dictionary is immutable"); }

        public ICollection<TKey> Keys => data.Keys;

        public ICollection<TValue> Values => data.Values;

        public int Count => data.Count;

        public bool IsReadOnly => true;

        public void Add(TKey key, TValue value) => throw new NotImplementedException("This dictionary is immutable");
        
        public void Add(KeyValuePair<TKey, TValue> item) => throw new NotImplementedException("This dictionary is immutable");
        
        public void Clear() => throw new NotImplementedException("This dictionary is immutable");

        public bool Contains(KeyValuePair<TKey, TValue> item) => data.Contains(item);

        public bool ContainsKey(TKey key) => data.ContainsKey(key);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => data.CopyTo(array, arrayIndex);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => new Dictionary<TKey, TValue>(data).GetEnumerator();

        public bool Remove(TKey key) => throw new NotImplementedException("This dictionary is immutable");

        public bool Remove(KeyValuePair<TKey, TValue> item) => throw new NotImplementedException("This dictionary is immutable");

        public bool TryGetValue(TKey key, out TValue value) => data.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
