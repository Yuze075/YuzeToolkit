#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;

namespace YuzeToolkit.InspectorTool
{
#if UNITY_EDITOR && YUZE_INSPECTOR_TOOL_USE_SHOW_VALUE
    [Serializable]
#endif
    public struct ShowList<T> : IEquatable<ShowList<T>>
    {
        public ShowList(IEnumerable<T> enumerable)
        {
#if UNITY_EDITOR && YUZE_INSPECTOR_TOOL_USE_SHOW_VALUE
            _showList = new List<IShowValue>();
#endif
            _nativeList = new List<T>();
            foreach (var t in enumerable) Add(t);
        }

        public ShowList(int capacity)
        {
#if UNITY_EDITOR && YUZE_INSPECTOR_TOOL_USE_SHOW_VALUE
            _showList = new List<IShowValue>(capacity);
#endif
            _nativeList = new List<T>(capacity);
        }

#if UNITY_EDITOR && YUZE_INSPECTOR_TOOL_USE_SHOW_VALUE
        [Disable]
        [LabelByParent]
        [IgnoreParent]
        [SerializeReference]
        [ReorderableList(fixedSize: true, draggable: false, HasLabels = false)]
        private List<IShowValue>? _showList;

        private List<IShowValue> ShowValueList => _showList ??= new List<IShowValue>();
#endif
        [NonSerialized] private List<T>? _nativeList;
        public List<T> NativeList => _nativeList ??= new List<T>();
        public int Count => NativeList.Count;
        public bool IsReadOnly => true;

        public int IndexOf(T item) => NativeList.IndexOf(item);
        public bool Contains(T item) => NativeList.Contains(item);
        public void CopyTo(T[] array, int arrayIndex) => NativeList.CopyTo(array, arrayIndex);

        public void Add(T item) => Insert(Count, item);

        public bool Remove(T item)
        {
            var index = NativeList.IndexOf(item);
            if (index < 0) return false;
            RemoveAt(index);
            return true;
        }

        public void Insert(int index, T item)
        {
#if UNITY_EDITOR && YUZE_INSPECTOR_TOOL_USE_SHOW_VALUE
            ShowValueList.Insert(index, IShowValue.GetShowValue(item, -1));
#endif
            NativeList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
#if UNITY_EDITOR && YUZE_INSPECTOR_TOOL_USE_SHOW_VALUE
            ShowValueList.RemoveAt(index);
#endif
            NativeList.RemoveAt(index);
        }

        public T this[int index]
        {
            get => NativeList[index];
            set
            {
#if UNITY_EDITOR && YUZE_INSPECTOR_TOOL_USE_SHOW_VALUE
                ShowValueList[index] = IShowValue.GetShowValue(value, -1);
#endif
                NativeList[index] = value;
            }
        }

        public void Clear()
        {
#if UNITY_EDITOR && YUZE_INSPECTOR_TOOL_USE_SHOW_VALUE
            _showList?.Clear();
#endif
            _nativeList?.Clear();
        }

        public IEnumerator<T> GetEnumerator() => NativeList.GetEnumerator();

        public bool Equals(ShowList<T> other) => _nativeList == other._nativeList;
        public override bool Equals(object? obj) => obj is ShowList<T> other && Equals(other);
        public override int GetHashCode() => _nativeList?.GetHashCode() ?? 0;
        public static bool operator ==(ShowList<T> left, ShowList<T> right) => left._nativeList == right._nativeList;
        public static bool operator !=(ShowList<T> left, ShowList<T> right) => left._nativeList != right._nativeList;
    }
}