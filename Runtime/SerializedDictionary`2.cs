using System.Collections.Generic;
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace YuzeToolkit.Framework.Utility
{
    [AttributeUsage(AttributeTargets.Field)]
    public class KvAttribute : PropertyAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class SdAttribute : PropertyAttribute
    {
    }

    [Serializable]
    public class SerializedDictionary<TK, TV> : IDictionary<TK, TV>, ISerializationCallbackReceiver
    {
        [Serializable]
        private struct KeyValuePair
        {
            [SerializeField] private TK key;
            [SerializeField] private TV value;

            public KeyValuePair(TK key, TV value)
            {
                this.key = key;
                this.value = value;
            }

            public TK Key
            {
                get => key;
                set => key = value;
            }

            public TV Value
            {
                get => value;
                set => this.value = value;
            }
        }

        public SerializedDictionary()
        {
            capacity = 10;
            pairs = new List<KeyValuePair>(capacity);
            _dictionary = new Dictionary<TK, TV>(capacity);
        }

        public SerializedDictionary(int capacity)
        {
            this.capacity = capacity;
            pairs = new List<KeyValuePair>(capacity);
            _dictionary = new Dictionary<TK, TV>(capacity);
        }

        public SerializedDictionary(Dictionary<TK, TV> dictionary)
        {
            capacity = dictionary.Count;
            pairs = new List<KeyValuePair>(capacity);
            _dictionary = new Dictionary<TK, TV>(dictionary);
            foreach (var kv in dictionary)
            {
                pairs.Add(new KeyValuePair(kv.Key, kv.Value));
            }
        }

        [SerializeField] [Kv] private List<KeyValuePair> pairs;
        [SerializeField, HideInInspector] private bool error;
        [SerializeField, HideInInspector] public int capacity;
        private readonly Dictionary<TK, TV> _dictionary;
        public bool Error => error;

        public void Clear()
        {
            pairs.Clear();
            _dictionary.Clear();
        }

        public Dictionary<TK, TV> ToNativeDictionary()
        {
            return new Dictionary<TK, TV>(_dictionary);
        }

        #region ISerializationCallbackReceiver

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            _dictionary.Clear();
            _dictionary.EnsureCapacity(capacity);
            error = false;

            foreach (var kv in pairs)
            {
                var key = kv.Key;
                if (key != null && !ContainsKey(key))
                {
                    _dictionary.Add(key, kv.Value);
                }
                else
                {
                    error = true;
                }
            }
        }

        #endregion

        #region IDictionary<TK, TV>

        public TV this[TK key]
        {
            get => _dictionary[key];
            set
            {
                if (_dictionary.ContainsKey(key))
                {
                    var count = pairs.Count;
                    for (var i = 0; i < count; i++)
                    {
                        var kv = pairs[i];
                        if (!EqualityComparer<TK>.Default.Equals(kv.Key, key)) continue;
                        pairs[i] = new KeyValuePair(key, value);
                        break;
                    }

                    _dictionary[key] = value;
                }
                else
                {
                    _dictionary.Add(key, value);
                    pairs.Add(new KeyValuePair(key, value));
                }
            }
        }

        public ICollection<TK> Keys => _dictionary.Keys;

        public ICollection<TV> Values => _dictionary.Values;

        public void Add(TK key, TV value)
        {
            pairs.Add(new KeyValuePair(key, value));
            _dictionary.Add(key, value);
        }

        public bool ContainsKey(TK key)
        {
            return _dictionary.ContainsKey(key);
        }

        public bool Remove(TK key)
        {
            if (!_dictionary.Remove(key)) return false;
            var count = pairs.Count;
            for (var i = 0; i < count; i++)
            {
                var kv = pairs[i];
                if (!EqualityComparer<TK>.Default.Equals(kv.Key, key)) continue;
                pairs.RemoveAt(i);
                break;
            }

            return true;
        }

        public bool TryGetValue(TK key, out TV value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        #endregion

        #region ICollection<KeyValuePair<TK, TV>>

        public int Count => _dictionary.Count;

        public bool IsReadOnly => false;

        void ICollection<KeyValuePair<TK, TV>>.Add(KeyValuePair<TK, TV> pair)
        {
            Add(pair.Key, pair.Value);
        }

        void ICollection<KeyValuePair<TK, TV>>.Clear()
        {
            Clear();
        }

        bool ICollection<KeyValuePair<TK, TV>>.Contains(KeyValuePair<TK, TV> pair)
        {
            return TryGetValue(pair.Key, out var value) &&
                   EqualityComparer<TV>.Default.Equals(value, pair.Value);
        }

        void ICollection<KeyValuePair<TK, TV>>.CopyTo(KeyValuePair<TK, TV>[] array, int index)
        {
            (_dictionary as ICollection).CopyTo(array, index);
        }

        bool ICollection<KeyValuePair<TK, TV>>.Remove(KeyValuePair<TK, TV> pair)
        {
            if (TryGetValue(pair.Key, out var value) &&
                EqualityComparer<TV>.Default.Equals(value, pair.Value)) return Remove(pair.Key);
            return false;
        }

        #endregion

        #region IEnumerable<KeyValuePair<TK, TV>>

        IEnumerator<KeyValuePair<TK, TV>> IEnumerable<KeyValuePair<TK, TV>>.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        #endregion

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        #endregion
    }
}