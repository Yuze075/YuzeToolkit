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
    public struct ShowIndexMap<TKey, TValue> : IEquatable<ShowIndexMap<TKey, TValue>>
    {
        public ShowIndexMap(IEnumerable<KeyValuePair<TKey, TValue>> enumerable)
        {
#if UNITY_EDITOR && YUZE_INSPECTOR_TOOL_USE_SHOW_VALUE
            showList = new List<ShowKeyValuePair>();
#endif
            _valuesIndex = new Dictionary<TKey, int>();
            _values = new List<TValue>();
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
#endif
        [NonSerialized] private Dictionary<TKey, int>? _valuesIndex;
        private Dictionary<TKey, int> ValuesIndex => _valuesIndex ??= new Dictionary<TKey, int>();
        private List<TValue>? _values;
        public List<TValue> Values => _values ??= new List<TValue>();

        public ICollection<TKey> Keys => ValuesIndex.Keys;
        public int Count => Values.Count;
        public bool IsReadOnly => true;
        public int GetIndex(TKey key) => ValuesIndex.ContainsKey(key) ? ValuesIndex[key] : -1;
        public TValue GetByIndex(int index) => Values[index];

        public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);
        public bool Contains(KeyValuePair<TKey, TValue> item) => ValuesIndex.ContainsKey(item.Key);
        public bool ContainsKey(TKey key) => ValuesIndex.ContainsKey(key);

#pragma warning disable CS8767 // 参数类型中引用类型的为 Null 性与隐式实现的成员不匹配(可能是由于为 Null 性特性)。
        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
#pragma warning restore CS8767 // 参数类型中引用类型的为 Null 性与隐式实现的成员不匹配(可能是由于为 Null 性特性)。
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
#if UNITY_EDITOR && YUZE_INSPECTOR_TOOL_USE_SHOW_VALUE
            ShowList.Add(ShowKeyValuePair.GetKeyValuePair(key, value));
#endif
            ValuesIndex.Add(key, Values.Count);
            Values.Add(value);
        }

        public TValue this[TKey key]
        {
            get => Values[ValuesIndex[key]];
            set
            {
                var index = ValuesIndex[key];
#if UNITY_EDITOR && YUZE_INSPECTOR_TOOL_USE_SHOW_VALUE
                if (index >= 0)
                {
                    ShowList[index] = ShowKeyValuePair.GetKeyValuePair(key, value);
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
#if UNITY_EDITOR && USE_SHOW_VALUE
            showList?.Clear();
#endif
            _valuesIndex?.Clear();
            _values?.Clear();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var (key, index) in ValuesIndex)
            {
                yield return new KeyValuePair<TKey, TValue>(key, Values[index]);
            }
        }

        public bool Equals(ShowIndexMap<TKey, TValue> other) =>
            _valuesIndex == other._valuesIndex && _values == other._values;

        public override bool Equals(object? obj) => obj is ShowIndexMap<TKey, TValue> other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(_valuesIndex, _values);

        public static bool operator ==(ShowIndexMap<TKey, TValue> left, ShowIndexMap<TKey, TValue> right) =>
            left._valuesIndex == right._valuesIndex && left._values == right._values;

        public static bool operator !=(ShowIndexMap<TKey, TValue> left, ShowIndexMap<TKey, TValue> right)=>
            left._valuesIndex != right._valuesIndex || left._values != right._values;
    }
}