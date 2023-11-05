using System;
using System.Collections.Generic;
using YuzeToolkit.LogTool;

namespace YuzeToolkit
{
    public struct DisposeGroup : IDisposable
    {
        public DisposeGroup(int capacity)
        {
            _isDisposed = false;
            _disposables = new List<IDisposable?>(capacity);
        }

        private bool _isDisposed;
        private List<IDisposable?>? _disposables;
        private List<IDisposable?> Disposables => _disposables ??= new List<IDisposable?>();

        /// <summary>
        /// 添加一个<see cref="IDisposable"/>到列表中, 在触发<see cref="DisposeGroup"/>.<see cref="DisposeGroup.Dispose"/>时释放
        /// </summary>
        public void Add(IDisposable? disposable)
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
        /// 同时在触发返回值<see cref="IDisposable"/>.<see cref="IDisposable.Dispose"/>时, 也会触发<paramref name="action"/>, 
        /// 同时讲这个绑定的<see cref="IDisposable"/>也从<see cref="DisposeGroup"/>中移除,解除对应的引用
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

        /// <summary>
        /// 注册一个<see cref="IDisposable"/><paramref name="disposable"/>, 在触发<see cref="DisposeGroup"/>.<see cref="DisposeGroup.Dispose"/>时触发<br/>
        /// 同时在触发返回值<see cref="IDisposable"/>.<see cref="IDisposable.Dispose"/>时, 也会释放<paramref name="disposable"/>, 
        /// 同时讲这个绑定的<see cref="IDisposable"/>也从<see cref="DisposeGroup"/>中移除,解除对应的引用
        /// </summary>
        /// <returns><see cref="IDisposable"/>接口, 可以直接触发释放对应的<paramref name="disposable"/>,
        /// 并且从<see cref="DisposeGroup"/>中移除</returns>
        public IDisposable UnRegister(IDisposable? disposable)
        {
            if (_isDisposed)
            {
                LogSys.Error("对象已经释放了, 无法继续添加IDisposable!");
                return new UnRegister(() => { disposable?.Dispose(); });
            }

            var unRegister = new UnRegister(disposable);
            Disposables.Add(unRegister);
            return unRegister;
        }

        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;

            // 如果列表为空直接返回
            if (_disposables == null) return;
            _disposables.ForEach(disposable => { disposable?.Dispose(); });
            _disposables.Clear();
            // // 列表长度大于0, 一直执行列表中的IDisposable的Dispose方法并移除, 直到没有列表长度为空才停止
            // // 因为列表中存在UnRegister形式的IDisposable, 所以不能通过foreach进行移除, 必须通过while
            // while (_disposables.Count > 0)
            // {
            //     var index = _disposables.Count - 1;
            //     var disposable = _disposables[index]; // 获取列表最后一个元素的
            //
            //     if (disposable != null)
            //     {
            //         // 不会空调用Dispose方法释放资源
            //         disposable.Dispose();
            //
            //         // 从列表后方开始遍历(因为当前元素就在后方, 从后方查找通常效率最高), 通过引用找到需要移除的元素的位置i, 移除对应元素
            //         // (不直接移除Index位置的元素, 是有可能在释放默认IDisposable时, 其同时也释放了该列表中的一个UnRegister的IDisposable,
            //         // 导致元素位置发生变化, 所以最保险的就是从后方开始从新比较一边)
            //         // 释放默认的IDisposable的时间复杂度为O(1), 释放为UnRegister的IDisposable的时间复杂度为O(N)
            //         var count = _disposables.Count;
            //         for (var i = count - 1; i >= 0; i--)
            //         {
            //             var d = _disposables[i];
            //             if (!ReferenceEquals(d, disposable)) continue;
            //             _disposables.RemoveAt(i);
            //             break;
            //         }
            //     }
            //     else
            //     {
            //         // 如果为空直接移除最后一个元素(因为移除的是最后一个元素可以减少拷贝次数)
            //         _disposables.RemoveAt(index);
            //     }
            // }
        }
    }
}