#nullable enable
using System;
using System.Collections.Generic;

namespace YuzeToolkit
{
    public interface IDisposeNode
    {
        protected internal DisposableList Disposables { get; }
    }

    public struct DisposableList
    {
        private List<IDisposable>? _disposables;

        /// <summary>
        /// 添加一个<see cref="IDisposable"/>到列表中, 在触发<see cref="DisposableList"/>.<see cref="DisposeAll"/>时释放
        /// </summary>
        public void Add(IDisposable? disposable)
        {
            if (disposable == null) return;
            _disposables ??= ListPool<IDisposable>.Get();
            _disposables.Add(disposable);
        }

        public void DisposeAll()
        {
            // 如果列表为空直接返回
            if (_disposables == null) return;
            foreach (var disposable in _disposables) disposable?.Dispose();
            ListPool<IDisposable>.RefRelease(ref _disposables);
        }
    }

    public static class DisposableExtensions
    {
        #region IDisposeNode

        public static void AddDisposable(this IDisposeNode self, IDisposable? disposable) =>
            self.Disposables.Add(disposable);

        public static void UnRegister<T>(this IDisposeNode self, Action<T> action, T value) =>
            self.AddDisposable(new AUnRegister<T>(action, value));

        public static void UnRegister<T>(this IDisposeNode self, Func<T, bool> func, T value) =>
            self.AddDisposable(new FUnRegister<T, bool>(func, value));

        public static void ClearAllDisposable(this IDisposeNode self) => self.Disposables.DisposeAll();

        #endregion
        
        public static void UnRegister<T>(this DisposableList self, Action<T> action, T value) =>
            self.Add(new AUnRegister<T>(action, value));

        public static void UnRegister<T>(this DisposableList self, Func<T, bool> func, T value) =>
            self.Add(new FUnRegister<T, bool>(func, value));

        /// <summary>
        /// 创建一个<see cref="IDisposable"/>对象, 在执行<see cref="IDisposable.Dispose"/>时, 使用<see cref="value"/>参数执行<see cref="action"/>方法<br/>
        /// (在被GC之后, 会默认触发<see cref="IDisposable.Dispose"/>方法)
        /// </summary>
        /// <param name="action">在<see cref="IDisposable.Dispose"/>时执行的方法</param>
        /// <param name="value">在<see cref="IDisposable.Dispose"/>时执行的方法的参数</param>
        public static IDisposable UnRegister<T>(Action<T> action, T value) => new AUnRegister<T>(action, value);

        /// <summary>
        /// 创建一个<see cref="IDisposable"/>对象, 在执行<see cref="IDisposable.Dispose"/>时, 使用<see cref="value"/>参数执行<see cref="func"/>方法<br/>
        /// (在被GC之后, 会默认触发<see cref="IDisposable.Dispose"/>方法)
        /// </summary>
        /// <param name="func">在<see cref="IDisposable.Dispose"/>时执行的方法</param>
        /// <param name="value">在<see cref="IDisposable.Dispose"/>时执行的方法的参数</param>
        public static IDisposable UnRegister<T>(Func<T, bool> func, T value) => new FUnRegister<T, bool>(func, value);

        private sealed class FUnRegister<T, TR> : IDisposable
        {
            private Func<T, TR>? _disposeFunc;
            private T? _value;

            public FUnRegister(Func<T, TR> disposeFunc, T value)
            {
                _disposeFunc = disposeFunc;
                _value = value;
            }

            ~FUnRegister() => Dispose();

            public void Dispose()
            {
                if (_disposeFunc == null || _value == null) return;
                _disposeFunc(_value);
                _disposeFunc = null;
                _value = default;
            }
        }

        private sealed class AUnRegister<T> : IDisposable
        {
            private Action<T>? _disposeAction;
            private T? _value;

            public AUnRegister(Action<T> disposeAction, T value)
            {
                _disposeAction = disposeAction;
                _value = value;
            }

            ~AUnRegister() => Dispose();

            public void Dispose()
            {
                if (_disposeAction == null || _value == null) return;
                _disposeAction(_value);
                _disposeAction = null;
                _value = default;
            }
        }
    }
}