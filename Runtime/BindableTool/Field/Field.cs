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

        private SLogTool? _logger;

        private SLogTool LogTool => _logger ??= new SLogTool(new[]
        {
            nameof(IField<TValue>),
            GetType().FullName
        });

        void IBindable.SetLogParent(ILogTool value) => LogTool.Parent = value;

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
            return _disposeGroup.UnRegister(() => { _valueChange -= valueChange; });
        }

        public IDisposable RegisterChangeBuff(ValueChange<TValue> valueChange)
        {
            valueChange.Invoke(default, Value);
            return RegisterChange(valueChange);
        }

        #endregion

        #region IDisposable

        private DisposeGroup _disposeGroup;

        void IDisposable.Dispose()
        {
            Value = default;
            _disposeGroup.Dispose();
        }

        #endregion

        public static implicit operator TValue?(Field<TValue> field) => field.Value;
    }
}