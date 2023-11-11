using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YuzeToolkit.InspectorTool
{
    [Serializable]
    public class ShowDictionaryIndex<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    {
        public ShowDictionaryIndex()
        {
#if UNITY_EDITOR
            showList = new List<ShowKeyValuePair>();
#endif
        }

        public ShowDictionaryIndex(IEnumerable<KeyValuePair<TKey, TValue>> enumerable)
        {
#if UNITY_EDITOR
            showList = new List<ShowKeyValuePair>();
#endif
            foreach (var keyValuePair in enumerable) Add(keyValuePair);
        }

#if UNITY_EDITOR
        [LabelByParent]
        [ReorderableList(fixedSize: true, draggable: false, HasLabels = false)]
        [IgnoreParent]
        [SerializeField]
        private List<ShowKeyValuePair> showList;
#endif
        private Dictionary<TKey, int>? _valuesIndex;
        private Dictionary<TKey, int> ValuesIndex => _valuesIndex ??= new Dictionary<TKey, int>();
        private List<TValue>? _values;
        public List<TValue> Values => _values ??= new List<TValue>();

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;
        public ICollection<TKey> Keys => ValuesIndex.Keys;
        ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;
        public int Count => Values.Count;
        public bool IsReadOnly => true;

        public Dictionary<TKey, TValue> GetNativeDictionary() => new(this);
        public int GetIndex(TKey key) => ValuesIndex[key];
        public TValue GetByIndex(int index) => Values[index];

        public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);
        public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);
        public bool Contains(KeyValuePair<TKey, TValue> item) => ValuesIndex.ContainsKey(item.Key);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) =>
            ((ICollection<KeyValuePair<TKey, TValue>>)GetNativeDictionary()).CopyTo(array, arrayIndex);

        public bool ContainsKey(TKey key) => ValuesIndex.ContainsKey(key);

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (ValuesIndex.TryGetValue(key, out var index))
            {
                value = Values[index];
                return true;
            }

            value = default!;
            return false;
        }

        public void Add(TKey key, TValue value)
        {
#if UNITY_EDITOR
            showList.Add(ShowKeyValuePair.GetKeyValuePair(key, value));
#endif
            ValuesIndex.Add(key, Values.Count);
            Values.Add(value);
        }


        public bool Remove(TKey key)
        {
            var index = ValuesIndex[key];

#if UNITY_EDITOR
            if (index >= 0)
            {
                showList.RemoveAt(index);
            }
#endif
            if (index < 0) return false;

            ValuesIndex.Remove(key);
            Values.RemoveAt(index);
            return true;
        }


        public TValue this[TKey key]
        {
            get => Values[ValuesIndex[key]];
            set
            {
                var index = ValuesIndex[key];
#if UNITY_EDITOR
                if (index >= 0)
                {
                    showList[index] = ShowKeyValuePair.GetKeyValuePair(key, value);
                }
#endif
                if (index >= 0)
                {
                    Values[index] = value;
                }
            }
        }

        public void Clear()
        {
#if UNITY_EDITOR
            showList.Clear();
#endif
            ValuesIndex.Clear();
            Values.Clear();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var (key, index) in ValuesIndex)
            {
                yield return new KeyValuePair<TKey, TValue>(key, Values[index]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}