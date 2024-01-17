#nullable enable
using System;
using UnityEngine;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.BindableTool
{
    /// <summary>
    /// <inheritdoc cref="YuzeToolkit.BindableTool.IField{TValue}" />
    /// </summary>
    /// <typeparam name="TValue">属性的数据类型</typeparam>
    [Serializable]
    public class Field<TValue> : BindableBase<TValue>, IField<TValue>
    {
        public Field(TValue? value = default, ILogging? loggingParent = null) : base(loggingParent) =>
            this.value = value;

        ~Field() => Dispose(false);
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

        public void SetValue(TValue? value) => Value = value;

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