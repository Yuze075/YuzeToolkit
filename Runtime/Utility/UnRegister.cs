using System;
using YuzeToolkit.PoolTool;

namespace YuzeToolkit
{
    public class UnRegister : IDisposable
    {
        private Action? _disposeAction;
        public UnRegister(Action? disposeAction) => _disposeAction = disposeAction;
        public void Dispose()
        {
            _disposeAction?.Invoke();
            _disposeAction = null;
        }
    }
}