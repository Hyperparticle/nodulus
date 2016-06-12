using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Core.Data;

namespace Assets.Scripts.Utility
{
    public class MultiMap<K1, K2, V>
    {
        private readonly IDictionary<K1, IDictionary<K2, V>> _multimap = new Dictionary<K1, IDictionary<K2, V>>();
        private readonly ICollection<V> _valueSet = new HashSet<V>();

        public void Add(K1 key1, K2 key2, V value)
        {
            this[key1].Add(key2, value);
            _valueSet.Add(value);
        }

        public bool ContainsKeys(K1 key1, K2 key2)
        {
            return this[key1].ContainsKey(key2);
        }

        public IEnumerable<V> GetValues(K1 key1)
        {
            return this[key1].Values;
        }

        public IEnumerable<K1> Keys
        {
            get { return _multimap.Keys; }
        }

        public IEnumerable<IDictionary<K2, V>> Values
        {
            get { return _multimap.Values; }
        }

        public IEnumerable<V> AllValues
        {
            get { return new HashSet<V>(_valueSet); }
        }

        public IDictionary<K2, V> this[K1 key1]
        {
            get
            {
                IDictionary<K2, V> map;
                map = _multimap.TryGetValue(key1, out map) ? map : new Dictionary<K2, V>();
                _multimap[key1] = map;
                return map;
            }
        }

        public V this[K1 key1, K2 key2]
        {
            get { return this[key1][key2]; }
        }

        public void Clear()
        {
            foreach (var value in Values)
            {
                value.Clear();
            }
        }

        public void ClearAll()
        {
            _multimap.Clear();
        }
    }
}
