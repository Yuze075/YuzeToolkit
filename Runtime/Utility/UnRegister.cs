#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;

namespace YuzeToolkit
{
    public class UnRegister : IDisposable
    {
        #region Class

        private Action? _disposeAction;
        private UnRegister(Action disposeAction) => _disposeAction = disposeAction;
        ~UnRegister() => Dispose();

        public void Dispose()
        {
            if (_disposeAction == null) return;
            _disposeAction();
            _disposeAction = null;
        }

        #endregion

        /// <summary>
        /// 创建一个<see cref="IDisposable"/>对象, 在执行<see cref="IDisposable.Dispose"/>时, 执行<see cref="action"/>方法<br/>
        /// (在被GC之后, 会默认触发<see cref="IDisposable.Dispose"/>方法)
        /// </summary>
        /// <param name="action">在<see cref="IDisposable.Dispose"/>时执行的方法</param>
        public static IDisposable Create(Action action) => new UnRegister(action);

        /// <summary>
        /// 创建一个<see cref="IDisposable"/>对象, 在执行<see cref="IDisposable.Dispose"/>时, 执行<see cref="action"/>方法<br/>
        /// 并且在执行<see cref="IDisposable.Dispose"/>后, 执行<see cref="doAfterDispose"/>方法<br/>
        /// (在被GC之后, 会默认触发<see cref="IDisposable.Dispose"/>方法)
        /// </summary>
        /// <param name="action">在<see cref="IDisposable.Dispose"/>时执行的方法</param>
        /// <param name="doAfterDispose">在<see cref="IDisposable.Dispose"/>后执行的方法</param>
        public static IDisposable Create(Action action, Action? doAfterDispose) =>
            new UnRegisterWithAction(action, doAfterDispose);

        /// <summary>
        /// 创建一个<see cref="IDisposable"/>对象, 在执行<see cref="IDisposable.Dispose"/>时, 使用<see cref="value"/>参数执行<see cref="action"/>方法<br/>
        /// (在被GC之后, 会默认触发<see cref="IDisposable.Dispose"/>方法)
        /// </summary>
        /// <param name="action">在<see cref="IDisposable.Dispose"/>时执行的方法</param>
        /// <param name="value">在<see cref="IDisposable.Dispose"/>时执行的方法的参数</param>
        [return: NotNullIfNotNull("value")]
        public static IDisposable? Create<T>(Action<T> action, T? value) =>
            value == null ? null : new AUnRegister<T>(action, value);

        /// <summary>
        /// 创建一个<see cref="IDisposable"/>对象, 在执行<see cref="IDisposable.Dispose"/>时, 使用<see cref="value"/>参数执行<see cref="action"/>方法<br/>
        /// 并且在执行<see cref="IDisposable.Dispose"/>后, 执行<see cref="doAfterDispose"/>方法<br/>
        /// (在被GC之后, 会默认触发<see cref="IDisposable.Dispose"/>方法)
        /// </summary>
        /// <param name="action">在<see cref="IDisposable.Dispose"/>时执行的方法</param>
        /// <param name="value">在<see cref="IDisposable.Dispose"/>时执行的方法的参数</param>
        /// <param name="doAfterDispose">在<see cref="IDisposable.Dispose"/>后执行的方法</param>
        [return: NotNullIfNotNull("value")]
        public static IDisposable? Create<T>(Action<T> action, T? value, Action? doAfterDispose) =>
            value == null ? null : new AUnRegisterWithAction<T>(action, value, doAfterDispose);

        /// <summary>
        /// 创建一个<see cref="IDisposable"/>对象, 在执行<see cref="IDisposable.Dispose"/>时, 使用<see cref="value"/>参数执行<see cref="func"/>方法<br/>
        /// (在被GC之后, 会默认触发<see cref="IDisposable.Dispose"/>方法)
        /// </summary>
        /// <param name="func">在<see cref="IDisposable.Dispose"/>时执行的方法</param>
        /// <param name="value">在<see cref="IDisposable.Dispose"/>时执行的方法的参数</param>
        [return: NotNullIfNotNull("value")]
        public static IDisposable? Create<T>(Func<T, bool> func, T? value) =>
            value == null ? null : new FUnRegister<T>(func, value);

        /// <summary>
        /// 创建一个<see cref="IDisposable"/>对象, 在执行<see cref="IDisposable.Dispose"/>时, 使用<see cref="value"/>参数执行<see cref="func"/>方法<br/>
        /// 并且在执行<see cref="IDisposable.Dispose"/>后, 使用<see cref="func"/>的返回值执行<see cref="doAfterDispose"/>方法<br/>
        /// (在被GC之后, 会默认触发<see cref="IDisposable.Dispose"/>方法)
        /// </summary>
        /// <param name="func">在<see cref="IDisposable.Dispose"/>时执行的方法</param>
        /// <param name="value">在<see cref="IDisposable.Dispose"/>时执行的方法的参数</param>
        /// <param name="doAfterDispose">在<see cref="IDisposable.Dispose"/>后执行的方法</param>
        [return: NotNullIfNotNull("value")]
        public static IDisposable? Create<T>(Func<T, bool> func, T? value, Action<bool>? doAfterDispose)
            => value == null ? null : new FUnRegisterWithAction<T>(func, value, doAfterDispose);

        /// <summary>
        /// 创建一个<see cref="IDisposable"/>对象, 在执行<see cref="IDisposable.Dispose"/>时, 执行<see cref="action"/>方法<br/>
        /// (在装箱之后, 即使被GC释放, 也不会默认触发<see cref="IDisposable.Dispose"/>方法)
        /// </summary>
        /// <param name="action">在<see cref="IDisposable.Dispose"/>时执行的方法</param>
        public static SUnRegister CreateStruct(Action action) => new(action);

        /// <summary>
        /// 创建一个<see cref="IDisposable"/>对象, 在执行<see cref="IDisposable.Dispose"/>时, 执行<see cref="action"/>方法<br/>
        /// 并且在执行<see cref="IDisposable.Dispose"/>后, 执行<see cref="doAfterDispose"/>方法<br/>
        /// (在装箱之后, 即使被GC释放, 也不会默认触发<see cref="IDisposable.Dispose"/>方法)
        /// </summary>
        /// <param name="action">在<see cref="IDisposable.Dispose"/>时执行的方法</param>
        /// <param name="doAfterDispose">在<see cref="IDisposable.Dispose"/>后执行的方法</param>
        public static SUnRegisterWithAction CreateStruct(Action action, Action? doAfterDispose) =>
            new(action, doAfterDispose);

        /// <summary>
        /// 创建一个<see cref="IDisposable"/>对象, 在执行<see cref="IDisposable.Dispose"/>时, 使用<see cref="value"/>参数执行<see cref="action"/>方法<br/>
        /// (在装箱之后, 即使被GC释放, 也不会默认触发<see cref="IDisposable.Dispose"/>方法)
        /// </summary>
        /// <param name="action">在<see cref="IDisposable.Dispose"/>时执行的方法</param>
        /// <param name="value">在<see cref="IDisposable.Dispose"/>时执行的方法的参数</param>
        public static SAUnRegister<T> CreateStruct<T>(Action<T> action, T? value) =>
            value == null ? default : new SAUnRegister<T>(action, value);

        /// <summary>
        /// 创建一个<see cref="IDisposable"/>对象, 在执行<see cref="IDisposable.Dispose"/>时, 使用<see cref="value"/>参数执行<see cref="action"/>方法<br/>
        /// 并且在执行<see cref="IDisposable.Dispose"/>后, 执行<see cref="doAfterDispose"/>方法<br/>
        /// (在装箱之后, 即使被GC释放, 也不会默认触发<see cref="IDisposable.Dispose"/>方法)
        /// </summary>
        /// <param name="action">在<see cref="IDisposable.Dispose"/>时执行的方法</param>
        /// <param name="value">在<see cref="IDisposable.Dispose"/>时执行的方法的参数</param>
        /// <param name="doAfterDispose">在<see cref="IDisposable.Dispose"/>后执行的方法</param>
        public static SAUnRegisterWithAction<T> CreateStruct<T>(Action<T> action, T? value, Action? doAfterDispose) =>
            value == null ? default : new SAUnRegisterWithAction<T>(action, value, doAfterDispose);

        /// <summary>
        /// 创建一个<see cref="IDisposable"/>对象, 在执行<see cref="IDisposable.Dispose"/>时, 使用<see cref="value"/>参数执行<see cref="func"/>方法<br/>
        /// (在装箱之后, 即使被GC释放, 也不会默认触发<see cref="IDisposable.Dispose"/>方法)
        /// </summary>
        /// <param name="func">在<see cref="IDisposable.Dispose"/>时执行的方法</param>
        /// <param name="value">在<see cref="IDisposable.Dispose"/>时执行的方法的参数</param>
        public static SFUnRegister<T> CreateStruct<T>(Func<T, bool> func, T? value) =>
            value == null ? default : new SFUnRegister<T>(func, value);

        /// <summary>
        /// 创建一个<see cref="IDisposable"/>对象, 在执行<see cref="IDisposable.Dispose"/>时, 使用<see cref="value"/>参数执行<see cref="func"/>方法<br/>
        /// 并且在执行<see cref="IDisposable.Dispose"/>后, 使用<see cref="func"/>的返回值执行<see cref="doAfterDispose"/>方法<br/>
        /// (在装箱之后, 即使被GC释放, 也不会默认触发<see cref="IDisposable.Dispose"/>方法)
        /// </summary>
        /// <param name="func">在<see cref="IDisposable.Dispose"/>时执行的方法</param>
        /// <param name="value">在<see cref="IDisposable.Dispose"/>时执行的方法的参数</param>
        /// <param name="doAfterDispose">在<see cref="IDisposable.Dispose"/>后执行的方法</param>
        public static SFUnRegisterWithAction<T> CreateStruct<T>(Func<T, bool> func, T? value,
            Action<bool>? doAfterDispose) =>
            value == null ? default : new SFUnRegisterWithAction<T>(func, value, doAfterDispose);

        private sealed class UnRegisterWithAction : IDisposable
        {
            private Action? _disposeAction;
            private Action? _doAfterDispose;

            public UnRegisterWithAction(Action disposeAction, Action? doAfterDispose)
            {
                _disposeAction = disposeAction;
                _doAfterDispose = doAfterDispose;
            }

            ~UnRegisterWithAction() => Dispose();

            public void Dispose()
            {
                if (_disposeAction == null) return;
                _disposeAction();
                _disposeAction = null;

                if (_doAfterDispose == null) return;
                _doAfterDispose();
                _doAfterDispose = null;
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

        private sealed class AUnRegisterWithAction<T> : IDisposable
        {
            private Action<T>? _disposeAction;
            private T? _value;
            private Action? _doAfterDispose;

            public AUnRegisterWithAction(Action<T> disposeAction, T value, Action? doAfterDispose)
            {
                _disposeAction = disposeAction;
                _value = value;
                _doAfterDispose = doAfterDispose;
            }

            ~AUnRegisterWithAction() => Dispose();

            public void Dispose()
            {
                if (_disposeAction == null || _value == null) return;
                _disposeAction(_value);
                _disposeAction = null;
                _value = default;

                if (_doAfterDispose == null) return;
                _doAfterDispose();
                _doAfterDispose = null;
            }
        }

        private sealed class FUnRegister<T> : IDisposable
        {
            private Func<T, bool>? _disposeAction;
            private T? _value;

            public FUnRegister(Func<T, bool> disposeAction, T value)
            {
                _disposeAction = disposeAction;
                _value = value;
            }

            ~FUnRegister() => Dispose();

            public void Dispose()
            {
                if (_disposeAction == null || _value == null) return;
                _disposeAction(_value);
                _disposeAction = null;
                _value = default;
            }
        }

        private sealed class FUnRegisterWithAction<T> : IDisposable
        {
            private Func<T, bool>? _disposeAction;
            private T? _value;
            private Action<bool>? _doAfterDispose;

            public FUnRegisterWithAction(Func<T, bool> disposeAction, T value, Action<bool>? doAfterDispose)
            {
                _disposeAction = disposeAction;
                _value = value;
                _doAfterDispose = doAfterDispose;
            }

            ~FUnRegisterWithAction() => Dispose();

            public void Dispose()
            {
                if (_disposeAction == null || _value == null) return;
                var value = _disposeAction(_value);
                _disposeAction = null;
                _value = default;

                if (_doAfterDispose == null) return;
                _doAfterDispose(value);
                _doAfterDispose = null;
            }
        }

        public struct SUnRegister : IDisposable
        {
            private Action? _disposeAction;
            public SUnRegister(Action disposeAction) => _disposeAction = disposeAction;

            public void Dispose()
            {
                if (_disposeAction == null) return;
                _disposeAction();
                _disposeAction = null;
            }
        }

        public struct SUnRegisterWithAction : IDisposable
        {
            private Action? _disposeAction;
            private Action? _doAfterDispose;

            public SUnRegisterWithAction(Action disposeAction, Action? doAfterDispose)
            {
                _disposeAction = disposeAction;
                _doAfterDispose = doAfterDispose;
            }

            public void Dispose()
            {
                if (_disposeAction == null) return;
                _disposeAction();
                _disposeAction = null;

                if (_doAfterDispose == null) return;
                _doAfterDispose();
                _doAfterDispose = null;
            }
        }

        // ReSharper disable once InconsistentNaming
        public struct SAUnRegister<T> : IDisposable
        {
            private Action<T>? _disposeAction;
            private T? _value;

            public SAUnRegister(Action<T> disposeAction, T value)
            {
                _disposeAction = disposeAction;
                _value = value;
            }

            public void Dispose()
            {
                if (_disposeAction == null || _value == null) return;
                _disposeAction(_value);
                _disposeAction = null;
                _value = default;
            }
        }

        // ReSharper disable once InconsistentNaming
        public struct SAUnRegisterWithAction<T> : IDisposable
        {
            private Action<T>? _disposeAction;
            private T? _value;
            private Action? _doAfterDispose;

            public SAUnRegisterWithAction(Action<T> disposeAction, T value, Action? doAfterDispose)
            {
                _disposeAction = disposeAction;
                _value = value;
                _doAfterDispose = doAfterDispose;
            }

            public void Dispose()
            {
                if (_disposeAction == null || _value == null) return;
                _disposeAction(_value);
                _disposeAction = null;
                _value = default;

                if (_doAfterDispose == null) return;
                _doAfterDispose();
                _doAfterDispose = null;
            }
        }

        // ReSharper disable once InconsistentNaming
        public struct SFUnRegister<T> : IDisposable
        {
            private Func<T, bool>? _disposeAction;
            private T? _value;

            public SFUnRegister(Func<T, bool> disposeAction, T value)
            {
                _disposeAction = disposeAction;
                _value = value;
            }

            public void Dispose()
            {
                if (_disposeAction == null || _value == null) return;
                _disposeAction(_value);
                _disposeAction = null;
                _value = default;
            }
        }

        // ReSharper disable once InconsistentNaming
        public struct SFUnRegisterWithAction<T> : IDisposable
        {
            private Func<T, bool>? _disposeAction;
            private T? _value;
            private Action<bool>? _doAfterDispose;

            public SFUnRegisterWithAction(Func<T, bool> disposeAction, T value, Action<bool>? doAfterDispose)
            {
                _disposeAction = disposeAction;
                _value = value;
                _doAfterDispose = doAfterDispose;
            }

            public void Dispose()
            {
                if (_disposeAction == null || _value == null) return;
                var value = _disposeAction(_value);
                _disposeAction = null;
                _value = default;

                if (_doAfterDispose == null) return;
                _doAfterDispose(value);
                _doAfterDispose = null;
            }
        }
    }
}