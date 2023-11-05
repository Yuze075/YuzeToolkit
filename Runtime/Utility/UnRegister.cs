using System;

namespace YuzeToolkit
{
    public class UnRegister : IDisposable
    {
        private Action? _disposeAction;

        public UnRegister(Action? disposeAction) => _disposeAction = disposeAction;
        public UnRegister(IDisposable? disposable) => _disposeAction = disposable != null ? disposable.Dispose : null;

        public void Dispose()
        {
            _disposeAction?.Invoke();
            _disposeAction = null;
        }
    }
}