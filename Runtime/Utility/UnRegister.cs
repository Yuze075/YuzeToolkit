using System;

namespace YuzeToolkit.Utility
{
    public struct UnRegister : IDisposable
    {
        private Action _action;

        public UnRegister(Action action) => _action = action;

        public void Dispose()
        {
            _action?.Invoke();
            _action = null;
        }
    }
}