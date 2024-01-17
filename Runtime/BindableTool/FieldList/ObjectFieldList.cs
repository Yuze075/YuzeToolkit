#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.BindableTool
{
    [Serializable]
    public class ObjectFieldList<TValue> : IFieldList<TValue> where TValue : UnityEngine.Object
    {
        public ObjectFieldList(ILogging? loggingParent = null)
        {
            list = new List<TValue>();
            Logging = new Logging(new[] { GetType().FullName }, loggingParent);
        }

        public ObjectFieldList(IEnumerable<TValue> enumerable, ILogging? loggingParent = null)
        {
            list = new List<TValue>(enumerable);
            Logging = new Logging(new[] { GetType().FullName }, loggingParent);
        }

        ~ObjectFieldList() => Dispose(false);
        [NonSerialized] private bool _disposed;
        protected Logging Logging { get; set; }
        object IBindable.Value => list;


        [ReorderableList(draggable: false, fixedSize: true, HasLabels = false)]
        [InLineEditor]
        [SerializeField]
        [LabelByParent]
        private List<TValue> list;

        #region ICollection

        public int Count
        {
            get
            {
                if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
                return list.Count;
            }
        }

        public bool IsReadOnly => true;

        public void Add(TValue item)
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            Insert(Count, item);
        }

        public void Clear()
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            list.Clear();
            _listChange?.Invoke(EventType.Clear, default!, -1);
            _fieldListChange?.Invoke(this);
        }

        public bool Contains(TValue item)
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            return list.Contains(item);
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            list.CopyTo(array, arrayIndex);
        }

        public bool Remove(TValue item)
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            var index = IndexOf(item);
            if (index < 0) return false;
            RemoveAt(index);
            return true;
        }

        #endregion

        #region IList

        public int IndexOf(TValue item)
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            return list.IndexOf(item);
        }

        public void Insert(int index, TValue item)
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            list.Insert(index, item);
            _listChange?.Invoke(EventType.Add, item, index);
            _fieldListChange?.Invoke(this);
        }

        public void RemoveAt(int index)
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            var item = list[index];
            list.RemoveAt(index);
            _listChange?.Invoke(EventType.Remove, item, index);
            _fieldListChange?.Invoke(this);
        }

        public TValue this[int index]
        {
            get
            {
                if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
                return list[index];
            }
            set
            {
                if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
                var baseValue = list[index];
                if (baseValue != null && baseValue.Equals(value)) return;
                list[index] = value;
                _listChange?.Invoke(EventType.Value, value, index, baseValue);
                _fieldListChange?.Invoke(this);
            }
        }

        #endregion

        #region RegisterChange

        private FieldListChange<TValue>? _fieldListChange;
        private ListChange<TValue>? _listChange;

        [return: NotNullIfNotNull("valueChange")]
        IDisposable? IBindable.RegisterChange(ValueChange<object>? valueChange)
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            if (valueChange == null) return null;
            return RegisterChange(fieldList => { valueChange(fieldList, fieldList); });
        }

        [return: NotNullIfNotNull("valueChange")]
        IDisposable? IBindable.RegisterChangeBuff(ValueChange<object>? valueChange)
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            if (valueChange == null) return null;
            valueChange(null, this);
            return RegisterChange(fieldList => { valueChange(null, fieldList); });
        }

        [return: NotNullIfNotNull("valueChange")]
        public IDisposable? RegisterChange(FieldListChange<TValue>? valueChange)
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            if (valueChange == null) return null;
            _fieldListChange += valueChange;
            return UnRegister.Create(action => _fieldListChange -= action, valueChange);
        }

        [return: NotNullIfNotNull("valueChange")]
        public IDisposable? RegisterChangeBuff(FieldListChange<TValue>? valueChange)
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            if (valueChange == null) return null;
            valueChange(this);
            _fieldListChange += valueChange;
            return UnRegister.Create(action => _fieldListChange -= action, valueChange);
        }

        [return: NotNullIfNotNull("valueChange")]
        IDisposable? IBindableList<TValue>.RegisterChange(BindableListChange<TValue>? valueChange)
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            if (valueChange == null) return null;
            var fieldListChange = new FieldListChange<TValue>(valueChange);
            _fieldListChange += fieldListChange;
            return UnRegister.Create(action => _fieldListChange -= action, fieldListChange);
        }

        [return: NotNullIfNotNull("valueChange")]
        IDisposable? IBindableList<TValue>.RegisterChangeBuff(BindableListChange<TValue>? valueChange)
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            if (valueChange == null) return null;
            valueChange(this);
            var fieldListChange = new FieldListChange<TValue>(valueChange);
            _fieldListChange += fieldListChange;
            return UnRegister.Create(action => _fieldListChange -= action, fieldListChange);
        }

        [return: NotNullIfNotNull("listChange")]
        public IDisposable? RegisterListChange(ListChange<TValue>? listChange)
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            if (listChange == null) return null;
            _listChange += listChange;
            return UnRegister.Create(action => _listChange -= action, listChange);
        }

        #endregion

        #region IDisposable

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            Clear();
            _fieldListChange = null;
            _listChange = null;
            _disposed = true;
        }

        #endregion

        public IEnumerator<TValue> GetEnumerator()
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}