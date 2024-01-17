#nullable enable
using System;
using UnityEngine;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.BindableTool
{
    /// <summary>
    /// <inheritdoc cref="YuzeToolkit.BindableTool.IModifiableField{TValue}" />
    /// </summary>
    /// <typeparam name="TValue">属性的数据类型</typeparam>
    [Serializable]
    public abstract class ModifiableField<TValue> : ModifiableBase<TValue, ModifyField<TValue>>,
        IModifiableField<TValue>
    {
        protected ModifiableField(TValue? value = default, bool isReadOnly = true,
            IModifiableOwner? modifiableOwner = null, ILogging? loggingParent = null) :
            base(isReadOnly, modifiableOwner, loggingParent) => this.value = value;

        ~ModifiableField() => Dispose(false);
        [NonSerialized] private bool _disposed;
        [SerializeField] private TValue? value;

        public sealed override TValue? Value
        {
            get
            {
                if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
                return value;
            }
            protected set
            {
                if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
                if (this.value != null && this.value.Equals(value)) return;
                ValueChange?.Invoke(this.value, value);
                this.value = value;
            }
        }

        protected sealed override void Modify(ModifyField<TValue> modifyField)
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            if (modifyField.modifyValue != null && modifyField.modifyValue.Equals(Value)) return;
            Value = modifyField.modifyValue;
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                value = default;
                _disposed = true;
            }

            base.Dispose(disposing);
        }
    }
}