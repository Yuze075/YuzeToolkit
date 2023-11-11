using System;
using UnityEngine;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.BindableTool
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <typeparam name="TValue">属性的数据类型</typeparam>
    [Serializable]
    public class Field<TValue> : IField<TValue>
    {
        public Field(TValue? value = default) => this.value = value;

        private SLogTool? _sLogTool;
        protected ILogTool LogTool => _sLogTool ??= SLogTool.Create(GetLogTags);

        protected virtual string[] GetLogTags => new[]
        {
            nameof(Field<TValue>),
            GetType().FullName
        };

        void IBindable.SetLogParent(ILogTool parent) => ((SLogTool)LogTool).Parent = parent;

        [SerializeField] private TValue? value;

        public TValue? Value
        {
            get => value;
            set
            {
                if (this.value != null && this.value.Equals(value)) return;
                _valueChange?.Invoke(this.value, value);
                this.value = value;
            }
        }

        #region Register

        private ValueChange<TValue>? _valueChange;

        public IDisposable RegisterChange(ValueChange<TValue> valueChange)
        {
            _valueChange += valueChange;
            return new UnRegister(() => { _valueChange -= valueChange; });
        }

        public IDisposable RegisterChangeBuff(ValueChange<TValue> valueChange)
        {
            valueChange.Invoke(default, Value);
            return RegisterChange(valueChange);
        }

        #endregion

        #region IDisposable

        void IDisposable.Dispose()
        {
            SLogTool.Release(ref _sLogTool);
            Value = default;
            _valueChange = null;
        }

        #endregion

        public static implicit operator TValue?(Field<TValue> field) => field.Value;
    }
}