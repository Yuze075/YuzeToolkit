using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YuzeToolkit.LogTool;
using UnityObject = UnityEngine.Object;

namespace YuzeToolkit.BindableTool
{
    [Serializable]
    public class ObjectFieldList<TValue> : IFieldList<TValue> where TValue : UnityObject
    {
        public ObjectFieldList() => list = new List<TValue>();
        public ObjectFieldList(IEnumerable<TValue> enumerable) => list = new List<TValue>(enumerable);

        private SLogTool? _sLogTool;
        protected ILogTool LogTool => _sLogTool ??= SLogTool.Create(GetLogTags);

        protected virtual string[] GetLogTags => new[]
        {
            nameof(IFieldList<TValue>),
            GetType().FullName
        };

        void IBindable.SetLogParent(ILogTool parent) => ((SLogTool)LogTool).Parent = parent;

        [ReorderableList(Foldable = false, HasLabels = false)] [InLineEditor] [SerializeField]
        private List<TValue> list;

        #region ICollection

        public int Count => list.Count;
        public bool IsReadOnly => true;
        public void Add(TValue item) => Insert(Count, item);

        public void Clear()
        {
            list.Clear();
            _clearAllValue?.Invoke();
            _fieldListChange?.Invoke(this);
        }

        public bool Contains(TValue item) => list.Contains(item);
        public void CopyTo(TValue[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);

        public bool Remove(TValue item)
        {
            var index = IndexOf(item);
            if (index < 0) return false;
            RemoveAt(index);
            return true;
        }

        #endregion

        #region IList

        public int IndexOf(TValue item) => list.IndexOf(item);

        public void Insert(int index, TValue item)
        {
            list.Insert(index, item);
            _addValue?.Invoke(item, index);
            _fieldListChange?.Invoke(this);
        }

        public void RemoveAt(int index)
        {
            var item = list[index];
            list.RemoveAt(index);
            _removeValue?.Invoke(item, index);
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
                _changeValue?.Invoke(baseValue!, value, index);
                _fieldListChange?.Invoke(this);
            }
        }

        #endregion

        #region RegisterChange

        private FieldListChange<TValue>? _fieldListChange;
        private AddValue<TValue>? _addValue;
        private RemoveValue<TValue>? _removeValue;
        private ChangeValue<TValue>? _changeValue;
        private ClearAllValue? _clearAllValue;

        public IDisposable RegisterChange(FieldListChange<TValue> valueChange)
        {
            _fieldListChange += valueChange;
            return new UnRegister(() => { _fieldListChange -= valueChange; });
        }

        public IDisposable RegisterChangeBuff(FieldListChange<TValue> valueChange)
        {
            valueChange.Invoke(this);
            return RegisterChange(valueChange);
        }

        IDisposable IBindableList<TValue>.RegisterChange(BindableListChange<TValue> valueChange)
        {
            var fieldListChange = new FieldListChange<TValue>(valueChange);
            _fieldListChange += fieldListChange;
            return new UnRegister(() => { _fieldListChange -= fieldListChange; });
        }

        IDisposable IBindableList<TValue>.RegisterChangeBuff(BindableListChange<TValue> valueChange)
        {
            valueChange.Invoke(this);
            var fieldListChange = new FieldListChange<TValue>(valueChange);
            _fieldListChange += fieldListChange;
            return new UnRegister(() => { _fieldListChange -= fieldListChange; });
        }

        public IDisposable RegisterAdd(AddValue<TValue> addValue)
        {
            _addValue += addValue;
            return new UnRegister(() => { _addValue -= addValue; });
        }

        public IDisposable RegisterRemove(RemoveValue<TValue> removeValue)
        {
            _removeValue += removeValue;
            return new UnRegister(() => { _removeValue -= removeValue; });
        }

        public IDisposable RegisterChange(ChangeValue<TValue> changeValue)
        {
            _changeValue += changeValue;
            return new UnRegister(() => { _changeValue -= changeValue; });
        }

        public IDisposable RegisterChange(ClearAllValue clearAllValue)
        {
            _clearAllValue += clearAllValue;
            return new UnRegister(() => { _clearAllValue -= clearAllValue; });
        }

        #endregion

        #region IDisposable

        void IDisposable.Dispose()
        {
            SLogTool.Release(ref _sLogTool);
            Clear();
            _fieldListChange = null;
            _addValue = null;
            _removeValue = null;
            _changeValue = null;
            _clearAllValue = null;
        }

        #endregion

        public IEnumerator<TValue> GetEnumerator() => list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}