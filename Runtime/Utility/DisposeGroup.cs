using System;
using System.Collections.Generic;
using YuzeToolkit.LogTool;
using YuzeToolkit.PoolTool;

namespace YuzeToolkit
{
    public struct DisposeGroup : IDisposable
    {
        private bool _isDisposed;
        private List<IDisposable?>? _disposables;
        private List<IDisposable?> Disposables => _disposables ??= ListPool<IDisposable?>.Get();

        /// <summary>
        /// 添加一个<see cref="IDisposable"/>到列表中, 在触发<see cref="DisposeGroup"/>.<see cref="DisposeGroup.Dispose"/>时释放
        /// </summary>
        public void Add(IDisposable? disposable) // todo 只有当这个对象被释放之后，所有被引用的内存空间才会释放
        {
            if (_isDisposed)
            {
                LogSys.Error("对象已经释放了, 无法继续添加IDisposable!");
                return;
            }

            Disposables.Add(disposable);
        }

        /// <summary>
        /// 注册一个委托<paramref name="action"/>, 在触发<see cref="DisposeGroup"/>.<see cref="DisposeGroup.Dispose"/>时触发<br/>
        /// 同时在触发返回值<see cref="IDisposable"/>.<see cref="IDisposable.Dispose"/>时, 也会触发<paramref name="action"/>
        /// </summary>
        /// <returns><see cref="IDisposable"/>接口, 可以直接触发释放对应的<paramref name="action"/>,
        /// 并且从<see cref="DisposeGroup"/>中移除</returns>
        public IDisposable UnRegister(Action? action)
        {
            if (_isDisposed)
            {
                LogSys.Error("对象已经释放了, 无法继续添加action!");
                return new UnRegister(action);
            }

            var unRegister = new UnRegister(action);
            Disposables.Add(unRegister);
            return unRegister;
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
            ListPool<IDisposable?>.Release(_disposables);
            _disposables = null;
        }
    }
}