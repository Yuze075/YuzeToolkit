using System;
using System.Collections.Generic;
using YuzeToolkit.Log;

namespace YuzeToolkit.Utility
{
    public class DisposeGroup : IDisposable
    {
        public DisposeGroup()
        {
            _disposables = new List<IDisposable>();
        }
        
        public DisposeGroup(int capacity)
        {
            _disposables = new List<IDisposable>(capacity);
        }
        private bool _isDispose;
        private readonly List<IDisposable> _disposables;
        public void Add(IDisposable disposable)
        {
            if(_isDispose)
            {
                LogSys.Error("对象已经释放了, 无法继续添加IDisposable!");
                return;
            }
            _disposables.Add(disposable);
        }
        public void Dispose()
        {
            if(_isDispose) return;
            foreach (var disposable in _disposables)
            {
                disposable?.Dispose();
            }
            _disposables.Clear();
            _isDispose = true;
        }
    }
}