#nullable enable
#if YUZE_TOOLKIT_USE_UNITY_NETCODE
using System;
using System.Diagnostics.CodeAnalysis;
using Unity.Netcode;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.BindableTool.Network
{
    [Serializable]
    public abstract class NetBindableBase<TValue> : NetworkVariable<TValue>, IBindable<TValue>
    {
        protected NetBindableBase(TValue? valueBase, NetworkVariableReadPermission readPerm,
            NetworkVariableWritePermission writePerm, ILogging? loggingParent) : base(valueBase!, readPerm, writePerm)
        {
            Logging = new Logging(new[] { GetType().FullName }, loggingParent);
            OnValueChanged += (value, newValue) => { ValueChange?.Invoke(value, newValue); };
        }

        ~NetBindableBase() => Dispose(false);
        [NonSerialized] private bool _disposed;
        protected Logging Logging { get; set; }
        object? IBindable.Value => Value;
        public new abstract TValue? Value { get; protected set; }

        protected TValue? BaseValue
        {
            get
            {
                if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
                return base.Value;
            }
            set
            {
                if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
                base.Value = value!;
            }
        }

        #region Register

        protected ValueChange<TValue>? ValueChange;

        [return: NotNullIfNotNull("valueChange")]
        public virtual IDisposable? RegisterChange(ValueChange<TValue>? valueChange)
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            if (valueChange == null) return null;
            ValueChange += valueChange;
            return UnRegister.Create(change => ValueChange += change, valueChange);
        }

        [return: NotNullIfNotNull("valueChange")]
        public virtual IDisposable? RegisterChangeBuff(ValueChange<TValue>? valueChange)
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            if (valueChange == null) return null;
            valueChange.Invoke(default, Value);
            ValueChange += valueChange;
            return UnRegister.Create(change => ValueChange += change, valueChange);
        }

        #endregion

        #region IDisposable

        public sealed override void Dispose()
        {
            Dispose(true);
            base.Dispose();
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            ValueChange = null;
            _disposed = true;
        }

        #endregion

        public static implicit operator TValue?(NetBindableBase<TValue> bindableBase) => bindableBase.Value;
    }
}
#endif