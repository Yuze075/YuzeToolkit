#nullable enable
using System;
using UnityEngine;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.BindableTool
{
    [Serializable]
    public sealed class ConvertBindable<TValue> : BindableBase<TValue>
    {
        public static ConvertBindable<TValue> Create<TBase>(IBindable<TBase> bindable, Func<TBase?, TValue?> predicate)
        {
            var convertBindable = new ConvertBindable<TValue>();
            convertBindable._disposable = bindable.RegisterChangeBuff((oldValue, newValue) =>
            {
                convertBindable.value = predicate(newValue);
                convertBindable.ValueChange?.Invoke(predicate(oldValue), predicate(newValue));
            });
            return convertBindable;
        }

        private ConvertBindable(ILogging? loggingParent = null) : base(loggingParent)
        {
        }

        ~ConvertBindable() => Dispose(false);
        [NonSerialized] private bool _disposed;
        [SerializeField] private TValue? value;
        private IDisposable? _disposable;

        public override TValue? Value
        {
            get => value;
            protected set => throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing) _disposable?.Dispose();
                _disposed = true;
            }

            base.Dispose(disposing);
        }
    }
}