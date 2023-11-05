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
        public ObjectFieldList(ILogTool parent = null!)
            => (LogTool.Parent, list) = (parent, new List<TValue>());

        public ObjectFieldList(IEnumerable<TValue> enumerable, ILogTool parent = null!)
            => (LogTool.Parent, list) = (parent, new List<TValue>(enumerable));

        private SLogTool? _sLogTool;

        private SLogTool LogTool => _sLogTool ??= new SLogTool(new[]
        {
            nameof(IFieldList<TValue>),
            GetType().FullName
        });

        void IBindable.SetLogParent(ILogTool value) => LogTool.Parent = value;

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
            return _disposeGroup.UnRegister(() => { _fieldListChange -= valueChange; });
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
            return _disposeGroup.UnRegister(() => { _fieldListChange -= fieldListChange; });
        }

        IDisposable IBindableList<TValue>.RegisterChangeBuff(BindableListChange<TValue> valueChange)
        {
            valueChange.Invoke(this);
            var fieldListChange = new FieldListChange<TValue>(valueChange);
            _fieldListChange += fieldListChange;
            return _disposeGroup.UnRegister(() => { _fieldListChange -= fieldListChange; });
        }

        public IDisposable RegisterAdd(AddValue<TValue> addValue)
        {
            _addValue += addValue;
            return _disposeGroup.UnRegister(() => { _addValue -= addValue; });
        }

        public IDisposable RegisterRemove(RemoveValue<TValue> removeValue)
        {
            _removeValue += removeValue;
            return _disposeGroup.UnRegister(() => { _removeValue -= removeValue; });
        }

        public IDisposable RegisterChange(ChangeValue<TValue> changeValue)
        {
            _changeValue += changeValue;
            return _disposeGroup.UnRegister(() => { _changeValue -= changeValue; });
        }

        public IDisposable RegisterChange(ClearAllValue clearAllValue)
        {
            _clearAllValue += clearAllValue;
            return _disposeGroup.UnRegister(() => { _clearAllValue -= clearAllValue; });
        }

        #endregion

        #region IDisposable

        private DisposeGroup _disposeGroup;

        void IDisposable.Dispose() => _disposeGroup.Dispose();

        #endregion

        public IEnumerator<TValue> GetEnumerator() => list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}