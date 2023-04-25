using System.Collections.Generic;
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace YuzeToolkit.Framework.Utility
{
    [AttributeUsage(AttributeTargets.Field)]
    public class PairAttribute : PropertyAttribute
    {
    }
    
    [AttributeUsage(AttributeTargets.Field)]
    public class SerializedDictionaryAttribute : PropertyAttribute
    {
    }

    [Serializable]
    public class SerializedDictionary<TK, TV> : IDictionary<TK, TV>,
        ISerializationCallbackReceiver
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

        [SerializeField] [Pair] private List<KeyValuePair> pairs;

        private readonly Dictionary<TK, int> _indexByKey = new();
        private readonly Dictionary<TK, TV> _dictionary = new();

        [SerializeField, HideInInspector] private bool error;


        private void UpdateIndexes(int removedIndex)
        {
            for (var i = removedIndex; i < pairs.Count; i++)
            {
                var key = pairs[i].Key;
                _indexByKey[key]--;
            }
        }

        #region ISerializationCallbackReceiver

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            _dictionary.Clear();
            _indexByKey.Clear();
            error = false;

            for (var i = 0; i < pairs.Count; i++)
            {
                var key = pairs[i].Key;
                if (key != null && !ContainsKey(key))
                {
                    _dictionary.Add(key, pairs[i].Value);
                    _indexByKey.Add(key, i);
                }
                else
                {
                    error = true;
                }
            }
        }

        #endregion

        public void Add(TK key, TV value)
        {
            pairs.Add(new KeyValuePair(key, value));
            _dictionary.Add(key, value);
            _indexByKey.Add(key, pairs.Count - 1);
        }

        public bool ContainsKey(TK key)
        {
            return _dictionary.ContainsKey(key);
        }

        public bool Remove(TK key)
        {
            if (!_dictionary.Remove(key)) return false;
            var index = _indexByKey[key];
            pairs.RemoveAt(index);
            UpdateIndexes(index);
            _indexByKey.Remove(key);
            return true;
        }

        public bool TryGetValue(TK key, out TV value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public void Clear()
        {
            pairs.Clear();
            _dictionary.Clear();
            _indexByKey.Clear();
        }

        public Dictionary<TK, TV> BuildNativeDictionary()
        {
            return new Dictionary<TK, TV>(_dictionary);
        }

        void ICollection<KeyValuePair<TK, TV>>.Add(KeyValuePair<TK, TV> pair)
        {
            Add(pair.Key, pair.Value);
        }

        bool ICollection<KeyValuePair<TK, TV>>.Contains(KeyValuePair<TK, TV> pair)
        {
            return _dictionary.TryGetValue(pair.Key, out var value) &&
                   EqualityComparer<TV>.Default.Equals(value, pair.Value);
        }

        bool ICollection<KeyValuePair<TK, TV>>.Remove(KeyValuePair<TK, TV> pair)
        {
            if (!_dictionary.TryGetValue(pair.Key, out var value)) return false;
            var isEqual = EqualityComparer<TV>.Default.Equals(value, pair.Value);
            return isEqual && Remove(pair.Key);
        }

        void ICollection<KeyValuePair<TK, TV>>.CopyTo(KeyValuePair<TK, TV>[] array, int index)
        {
            ICollection collection = _dictionary;
            collection.CopyTo(array, index);
        }

        IEnumerator<KeyValuePair<TK, TV>> IEnumerable<KeyValuePair<TK, TV>>.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }


        /// <summary>
        /// Indicates if there is a key collision in serialized pairs.
        /// Duplicated keys (pairs) won't be added to the final dictionary.
        /// This property is crucial for Editor-related functions.
        /// </summary>
        internal bool Error => error;

        public int Count => _dictionary.Count;

        public bool IsReadOnly => false;

        public ICollection<TK> Keys => _dictionary.Keys;

        public ICollection<TV> Values => _dictionary.Values;

        public TV this[TK key]
        {
            get => _dictionary[key];
            set
            {
                _dictionary[key] = value;
                if (_indexByKey.ContainsKey(key))
                {
                    var index = _indexByKey[key];
                    pairs[index] = new KeyValuePair(key, value);
                }
                else
                {
                    pairs.Add(new KeyValuePair(key, value));
                    _indexByKey.Add(key, pairs.Count - 1);
                }
            }
        }
    }
}