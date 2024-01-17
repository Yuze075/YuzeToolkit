#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.BindableTool
{
    [Serializable]
    public abstract class BindableBase<TValue> : IBindable<TValue>
    {
        protected BindableBase(ILogging? loggingParent)
        {
            Logging = new Logging(new[] { GetType().FullName }, loggingParent);
        }

        ~BindableBase() => Dispose(false);
        [NonSerialized] private bool _disposed;
        protected Logging Logging { get; set; }
        object? IBindable.Value => Value;
        public abstract TValue? Value { get; protected set; }

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

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            ValueChange = null;
            _disposed = true;
        }

        #endregion

        public static implicit operator TValue?(BindableBase<TValue> bindableBase) => bindableBase.Value;
    }
}