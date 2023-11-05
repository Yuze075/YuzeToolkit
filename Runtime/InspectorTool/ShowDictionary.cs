using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YuzeToolkit.InspectorTool
{
    [Serializable]
    public class ShowDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    {
        public ShowDictionary()
        {
#if UNITY_EDITOR
            showList = new List<ShowKeyValuePair>();
#endif
        }

        public ShowDictionary(IEnumerable<KeyValuePair<TKey, TValue>> enumerable)
        {
#if UNITY_EDITOR
            showList = new List<ShowKeyValuePair>();
#endif
            foreach (var keyValuePair in enumerable) Add(keyValuePair);
        }

#if UNITY_EDITOR
        [ReorderableList(fixedSize: true, draggable: false, HasLabels = false)] [IgnoreParent] [SerializeField]
        private List<ShowKeyValuePair> showList;

        private Dictionary<TKey, int>? _showListIndex;
        private Dictionary<TKey, int> ShowListIndex => _showListIndex ??= new Dictionary<TKey, int>();
#endif
        private Dictionary<TKey, TValue>? _nativeDictionary;
        public Dictionary<TKey, TValue> NativeDictionary => _nativeDictionary ??= new Dictionary<TKey, TValue>();

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;
        public ICollection<TKey> Keys => NativeDictionary.Keys;
        public ICollection<TValue> Values => NativeDictionary.Values;

        public int Count => NativeDictionary.Count;
        public bool IsReadOnly => true;

        public bool ContainsKey(TKey key) => NativeDictionary.ContainsKey(key);
        public bool Contains(KeyValuePair<TKey, TValue> item) => ContainsKey(item.Key);
        public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);
        public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);
        public bool TryGetValue(TKey key, out TValue value) => NativeDictionary.TryGetValue(key, out value);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) =>
            ((ICollection<KeyValuePair<TKey, TValue>>)NativeDictionary).CopyTo(array, arrayIndex);

        public void Add(TKey key, TValue value)
        {
#if UNITY_EDITOR
            ShowListIndex.Add(key, showList.Count);
            showList.Add(ShowKeyValuePair.GetKeyValuePair(key, value));
#endif
            NativeDictionary.Add(key, value);
        }

        public bool Remove(TKey key)
        {
#if UNITY_EDITOR
            var index = ShowListIndex[key];
            if (index >= 0)
            {
                ShowListIndex.Remove(key);
                showList.RemoveAt(index);
            }
#endif
            return NativeDictionary.Remove(key);
        }

        public TValue this[TKey key]
        {
            get => NativeDictionary[key];
            set
            {
#if UNITY_EDITOR
                var index = ShowListIndex[key];
                if (index >= 0)
                {
                    showList[index] = ShowKeyValuePair.GetKeyValuePair(key, value);
                }
#endif
                NativeDictionary[key] = value;
            }
        }

        public void Clear()
        {
#if UNITY_EDITOR
            showList.Clear();
            ShowListIndex.Clear();
#endif
            NativeDictionary.Clear();
        }


        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => NativeDictionary.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}