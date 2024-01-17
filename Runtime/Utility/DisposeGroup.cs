#nullable enable
using System;
using System.Collections.Generic;
using YuzeToolkit.LogTool;
using YuzeToolkit.PoolTool;

namespace YuzeToolkit
{
    public struct DisposeGroup : IDisposable
    {
        private bool _isDisposed;
        private List<IDisposable>? _disposables;
        private List<IDisposable> Disposables => _disposables ??= ListPool<IDisposable>.Get();

        /// <summary>
        /// 添加一个<see cref="IDisposable"/>到列表中, 在触发<see cref="DisposeGroup"/>.<see cref="DisposeGroup.Dispose"/>时释放
        /// </summary>
        public void Add(IDisposable? disposable) // todo 只有当这个对象被释放之后，所有被引用的内存空间才会释放
        {
            if (_isDisposed)
            {
                LogSys.LogError("对象已经释放了, 无法继续添加IDisposable!");
                return;
            }

            if (disposable == null) return;
            if (ReferenceEquals(disposable, Null)) return;
            Disposables.Add(disposable);
        }

        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;

            // 如果列表为空直接返回
            if (_disposables == null) return;

            var count = _disposables.Count;
            for (var i = 0; i < count; i++)
            {
                var disposable = _disposables[i];
                disposable?.Dispose();
            }

            _disposables.Clear();
            ListPool<IDisposable>.RefRelease(ref _disposables);
        }

        public static IDisposable Null { get; } = new NullDisposable();

        private class NullDisposable : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }
}