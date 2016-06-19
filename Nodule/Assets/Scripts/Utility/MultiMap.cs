using System.Collections.Generic;

namespace Assets.Scripts.Utility
{
    public class MultiMap<TKey1, TKey2, TValue>
    {
        private readonly IDictionary<TKey1, IDictionary<TKey2, TValue>> _multimap = new Dictionary<TKey1, IDictionary<TKey2, TValue>>();
        private readonly ICollection<TValue> _valueSet = new HashSet<TValue>();

        public void Add(TKey1 key1, TKey2 key2, TValue value)
        {
            this[key1].Add(key2, value);
            _valueSet.Add(value);
        }

        public void AddAll(MultiMap<TKey1, TKey2, TValue> map)
        {
            foreach (var pair in map._multimap)
            {
                _multimap.Add(pair.Key, pair.Value);
            }
        }

        public bool Remove(TKey1 key1)
        {
            return _multimap.Remove(key1);
        }

        public bool ContainsKeys(TKey1 key1, TKey2 key2)
        {
            return this[key1].ContainsKey(key2);
        }

        public IEnumerable<TValue> GetValues(TKey1 key1)
        {
            return this[key1].Values;
        }

        public IEnumerable<TKey1> Keys
        {
            get { return _multimap.Keys; }
        }

        public IEnumerable<IDictionary<TKey2, TValue>> Values
        {
            get { return _multimap.Values; }
        }

        public IEnumerable<TValue> AllValues
        {
            get { return new HashSet<TValue>(_valueSet); }
        }

        public IDictionary<TKey2, TValue> this[TKey1 key1]
        {
            get
            {
                IDictionary<TKey2, TValue> map;
                map = _multimap.TryGetValue(key1, out map) ? map : new Dictionary<TKey2, TValue>();
                _multimap[key1] = map;
                return map;
            }
        }

        public TValue this[TKey1 key1, TKey2 key2]
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
