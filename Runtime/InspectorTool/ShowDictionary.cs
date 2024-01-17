#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace YuzeToolkit.InspectorTool
{
#if UNITY_EDITOR && YUZE_INSPECTOR_TOOL_USE_SHOW_VALUE
    [Serializable]
#endif
    public struct ShowDictionary<TKey, TValue> : IEquatable<ShowDictionary<TKey, TValue>>
    {
        public ShowDictionary(IEnumerable<KeyValuePair<TKey, TValue>> enumerable)
        {
#if UNITY_EDITOR && YUZE_INSPECTOR_TOOL_USE_SHOW_VALUE
            showList = new List<ShowKeyValuePair>();
            _showListKeys = new List<TKey>();
#endif
            _nativeDictionary = new Dictionary<TKey, TValue>();
            foreach (var keyValuePair in enumerable) Add(keyValuePair);
        }

#if UNITY_EDITOR && YUZE_INSPECTOR_TOOL_USE_SHOW_VALUE
        [Disable]
        [LabelByParent]
        [ReorderableList(fixedSize: true, draggable: false, HasLabels = false)]
        [IgnoreParent]
        [SerializeField]
        private List<ShowKeyValuePair>? showList;

        private List<ShowKeyValuePair> ShowList => showList ??= new List<ShowKeyValuePair>();
        private List<TKey>? _showListKeys;
        private List<TKey> ShowListKeys => _showListKeys ??= new List<TKey>();
#endif
        [NonSerialized] private Dictionary<TKey, TValue>? _nativeDictionary;
        public Dictionary<TKey, TValue> NativeDictionary => _nativeDictionary ??= new Dictionary<TKey, TValue>();
        public ICollection<TKey> Keys => NativeDictionary.Keys;
        public ICollection<TValue> Values => NativeDictionary.Values;
        public int Count => NativeDictionary.Count;
        public bool IsReadOnly => true;

        public bool ContainsKey(TKey key) => NativeDictionary.ContainsKey(key);
        public bool Contains(KeyValuePair<TKey, TValue> item) => ContainsKey(item.Key);
        public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);
        public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);

#pragma warning disable CS8767 // 参数类型中引用类型的为 Null 性与隐式实现的成员不匹配(可能是由于为 Null 性特性)。
        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) =>
#pragma warning restore CS8767 // 参数类型中引用类型的为 Null 性与隐式实现的成员不匹配(可能是由于为 Null 性特性)。
            NativeDictionary.TryGetValue(key, out value);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) =>
            ((ICollection<KeyValuePair<TKey, TValue>>)NativeDictionary).CopyTo(array, arrayIndex);

        public void Add(TKey key, TValue value)
        {
#if UNITY_EDITOR && YUZE_INSPECTOR_TOOL_USE_SHOW_VALUE
            ShowListKeys.Add(key);
            ShowList.Add(ShowKeyValuePair.GetKeyValuePair(key, value));
#endif
            NativeDictionary.Add(key, value);
        }

        public bool Remove(TKey key)
        {
#if UNITY_EDITOR && YUZE_INSPECTOR_TOOL_USE_SHOW_VALUE
            var index = ShowListKeys.IndexOf(key);
            if (index >= 0)
            {
                ShowListKeys.Remove(key);
                ShowList.RemoveAt(index);
            }
#endif
            return NativeDictionary.Remove(key);
        }

        public TValue this[TKey key]
        {
            get => NativeDictionary[key];
            set
            {
#if UNITY_EDITOR && YUZE_INSPECTOR_TOOL_USE_SHOW_VALUE
                var index = ShowListKeys.IndexOf(key);
                if (index >= 0)
                {
                    ShowList[index] = ShowKeyValuePair.GetKeyValuePair(key, value);
                }
#endif
                NativeDictionary[key] = value;
            }
        }

        public void Clear()
        {
#if UNITY_EDITOR && YUZE_INSPECTOR_TOOL_USE_SHOW_VALUE
            showList?.Clear();
            _showListKeys?.Clear();
#endif
            _nativeDictionary?.Clear();
        }


        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => NativeDictionary.GetEnumerator();

        public bool Equals(ShowDictionary<TKey, TValue> other) => _nativeDictionary == other._nativeDictionary;
        public override bool Equals(object? obj) => obj is ShowDictionary<TKey, TValue> other && Equals(other);
        public override int GetHashCode() => _nativeDictionary?.GetHashCode() ?? 0;

        public static bool operator ==(ShowDictionary<TKey, TValue> left, ShowDictionary<TKey, TValue> right) =>
            left._nativeDictionary == right._nativeDictionary;

        public static bool operator !=(ShowDictionary<TKey, TValue> left, ShowDictionary<TKey, TValue> right) =>
            left._nativeDictionary != right._nativeDictionary;
    }
}