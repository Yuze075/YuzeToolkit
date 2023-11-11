using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YuzeToolkit.InspectorTool
{
    [Serializable]
    public class ShowList<T> : IList<T>, IReadOnlyList<T>
    {
        public ShowList()
        {
#if UNITY_EDITOR
            _showList = new List<IShowValue>();
#endif
        }

        public ShowList(IEnumerable<T> enumerable)
        {
#if UNITY_EDITOR
            _showList = new List<IShowValue>();
#endif
            foreach (var t in enumerable) Add(t);
        }

        public ShowList(int capacity)
        {
#if UNITY_EDITOR
            _showList = new List<IShowValue>(capacity);
#endif
            _nativeList = new List<T>(capacity);
        }

#if UNITY_EDITOR
        [LabelByParent]
        [IgnoreParent]
        [SerializeReference]
        [ReorderableList(fixedSize: true, draggable: false, HasLabels = false)]
        private List<IShowValue> _showList;
#endif
        private List<T>? _nativeList;
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
#if UNITY_EDITOR
            _showList.Insert(index, IShowValue.GetShowValue(item, -1));
#endif
            NativeList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
#if UNITY_EDITOR
            _showList.RemoveAt(index);
#endif
            NativeList.RemoveAt(index);
        }

        public T this[int index]
        {
            get => NativeList[index];
            set
            {
#if UNITY_EDITOR
                _showList[index] = IShowValue.GetShowValue(value, -1);
#endif
                NativeList[index] = value;
            }
        }

        public void Clear()
        {
#if UNITY_EDITOR
            _showList.Clear();
#endif
            NativeList.Clear();
        }

        public IEnumerator<T> GetEnumerator() => NativeList.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}