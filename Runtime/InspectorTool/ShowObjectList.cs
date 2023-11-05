using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace YuzeToolkit.InspectorTool
{
    [Serializable]
    public class ShowObjectList : IList<object>, IReadOnlyList<object>
    {
        public ShowObjectList()
        {
#if UNITY_EDITOR
            _showList = new List<IShowValue>();
#endif
        }

        public ShowObjectList(IEnumerable enumerable)
        {
#if UNITY_EDITOR
            _showList = new List<IShowValue>();
#endif
            foreach (var o in enumerable) Add(o);
        }

#if UNITY_EDITOR
        [IgnoreParent] [SerializeReference] [ReorderableList(fixedSize: true, draggable: false, HasLabels = false)]
        private List<IShowValue> _showList;
#endif
        private List<object>? _nativeList;
        public List<object> NativeList => _nativeList ??= new List<object>();

        public IEnumerable<T> GetTEnumerable<T>() => NativeList.OfType<T>();
        public List<T> GetTList<T>() => NativeList.OfType<T>().ToList();
        public int Count => NativeList.Count;
        public bool IsReadOnly => true;

        public int IndexOf(object item) => NativeList.IndexOf(item);
        public bool Contains(object item) => NativeList.Contains(item);
        public void CopyTo(object[] array, int arrayIndex) => NativeList.CopyTo(array, arrayIndex);

        public void Add(object item) => Insert(Count, item);

        public bool Remove(object item)
        {
            var index = NativeList.IndexOf(item);
            if (index < 0) return false;
            RemoveAt(index);
            return true;
        }

        public void Insert(int index, object item)
        {
#if UNITY_EDITOR
            _showList.Insert(index, IShowValue.GetShowValue(item));
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

        public object this[int index]
        {
            get => NativeList[index];
            set
            {
#if UNITY_EDITOR
                _showList[index] = IShowValue.GetShowValue(value);
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

        public IEnumerator<object> GetEnumerator() => NativeList.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}