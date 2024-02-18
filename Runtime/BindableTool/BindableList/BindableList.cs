#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YuzeToolkit.BindableTool
{
    [Serializable]
    public class BindableList<TValue> : IBindableList<TValue>
    {
        public BindableList()
        {
            list = new List<TValue>();
        }

        public BindableList(IEnumerable<TValue> enumerable)
        {
            list = new List<TValue>(enumerable);
        }

#if YUZE_USE_EDITOR_TOOLBOX
        [ReorderableList(draggable: false, fixedSize: true), IgnoreParent, LabelByParent]
#endif
        [SerializeField]
        private List<TValue> list;

        private BindableListChange<TValue>? _fieldListChange;
        private ListChange<TValue>? _listChange;

        public void AddBindableListChange(BindableListChange<TValue>? bindableListChange)
        {
            if (bindableListChange == null) return;
            _fieldListChange += bindableListChange;
        }

        public void RemoveBindableListChange(BindableListChange<TValue>? bindableListChange)
        {
            if (bindableListChange == null) return;
            _fieldListChange -= bindableListChange;
        }

        public void AddListChange(ListChange<TValue>? listChange)
        {
            if (listChange == null) return;
            _listChange += listChange;
        }

        public void RemoveListChange(ListChange<TValue>? listChange)
        {
            if (listChange == null) return;
            _listChange -= listChange;
        }

        #region ICollection

        public int Count => list.Count;

        public bool IsReadOnly => true;

        public void Add(TValue item)
        {
            Insert(Count, item);
        }

        public void Clear()
        {
            list.Clear();
            _listChange?.Invoke(EventType.Clear, default!, -1);
            _fieldListChange?.Invoke(this);
        }

        public bool Contains(TValue item)
        {
            return list.Contains(item);
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public bool Remove(TValue item)
        {
            var index = IndexOf(item);
            if (index < 0) return false;
            RemoveAt(index);
            return true;
        }

        #endregion

        #region IList

        public int IndexOf(TValue item)
        {
            return list.IndexOf(item);
        }

        public void Insert(int index, TValue item)
        {
            list.Insert(index, item);
            _listChange?.Invoke(EventType.Add, item, index);
            _fieldListChange?.Invoke(this);
        }

        public void RemoveAt(int index)
        {
            var item = list[index];
            list.RemoveAt(index);
            _listChange?.Invoke(EventType.Remove, item, index);
            _fieldListChange?.Invoke(this);
        }

        public TValue this[int index]
        {
            get => list[index];
            set
            {
                var baseValue = list[index];
                if (baseValue != null && baseValue.Equals(value)) return;
                list[index] = value;
                _listChange?.Invoke(EventType.Value, value, index, baseValue);
                _fieldListChange?.Invoke(this);
            }
        }

        #endregion

        public virtual void Reset()
        {
            list.Clear();
            _fieldListChange = null;
            _listChange = null;
        }

        public List<TValue>.Enumerator GetEnumerator() => list.GetEnumerator();
        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}