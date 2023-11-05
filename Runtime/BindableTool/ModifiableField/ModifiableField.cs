using System;
using UnityEngine;
using YuzeToolkit.LogTool;
using YuzeToolkit;

namespace YuzeToolkit.BindableTool
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <typeparam name="TValue">属性的数据类型</typeparam>
    [Serializable]
    public abstract class ModifiableField<TValue> : IModifiableField<TValue>
    {
        protected ModifiableField(TValue? value = default, bool isReadOnly = true) =>
            (this.value, this.isReadOnly) = (value, isReadOnly);

        private SLogTool? _sLogTool;
        private IModifiableOwner? _owner;

        private SLogTool LogTool => _sLogTool ??= new SLogTool(new[]
        {
            nameof(IModifiableField<TValue>),
            GetType().FullName
        });

        void IBindable.SetLogParent(ILogTool value) => LogTool.Parent = value;

        IModifiableOwner IModifiable.Owner => LogTool.IsNotNull(_owner);

        void IModifiable.SetOwner(IModifiableOwner value)
        {
            if (_owner != null)
                LogTool.Log(
                    $"类型为{GetType()}的{nameof(IModifiable)}的{nameof(_owner)}从{_owner.GetType()}替换为{value.GetType()}",
                    ELogType.Warning);
            _owner = value;
        }


        [SerializeField] private TValue? value;
        [SerializeField] private bool isReadOnly;

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

        public bool IsReadOnly => isReadOnly;

        IDisposable IModifiable.Modify(IModify modify, IModifyReason reason)
        {
            if (!this.IsSameOwnerReason(reason, LogTool)) return modify;

            if (!this.TryCastModify(modify, LogTool, out ModifyField<TValue> modifyIn)) return modify;

            if (!this.TryCheckModify(modifyIn, reason, out var modifyOut))
                return modifyIn;

            return Modify(modifyOut);
        }

        IDisposable IModifiableField<TValue>.Modify(ModifyField<TValue> modifyField, IModifyReason reason)
        {
            if (!this.IsSameOwnerReason(reason, LogTool)) return modifyField;

            if (!this.TryCheckModify(modifyField, reason, out var modifyOut))
                return modifyField;

            return Modify(modifyOut);
        }

        private IDisposable Modify(ModifyField<TValue> modifyField)
        {
            if (!this.TryCheckModifyType(modifyField, LogTool)) return modifyField;

            if (modifyField.ModifyValue != null && modifyField.ModifyValue.Equals(Value)) return modifyField;
            Value = modifyField.ModifyValue;
            return modifyField;
        }

        void IModifiable.ReCheckValue()
        {
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

        public static implicit operator TValue?(ModifiableField<TValue> modifiableField) => modifiableField.Value;
    }
}