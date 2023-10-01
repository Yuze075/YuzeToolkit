using System;

namespace YuzeToolkit.MonoDriver
{
    internal class FuncUpdateWrapper<T> : IUpdate
    {
        private readonly Func<T> _func;
        private readonly int _priority;
        private readonly OrderType _type;

        public FuncUpdateWrapper(Func<T> func, int priority, OrderType type)
        {
            _func = func;
            _priority = priority;
            _type = type;
        }

        public void OnUpdate() => _func?.Invoke();
        public int Priority => _priority;
        public OrderType Type => _type;
    }

    internal class FuncFixedUpdateWrapper<T> : IFixedUpdate
    {
        private readonly Func<T> _func;
        private readonly int _priority;
        private readonly OrderType _type;

        public FuncFixedUpdateWrapper(Func<T> func, int priority, OrderType type)
        {
            _func = func;
            _priority = priority;
            _type = type;
        }

        public void OnFixedUpdate() => _func?.Invoke();
        public int Priority => _priority;
        public OrderType Type => _type;
    }

    internal class FuncLateUpdateWrapper<T> : ILateUpdate
    {
        private readonly Func<T> _func;
        private readonly int _priority;
        private readonly OrderType _type;

        public FuncLateUpdateWrapper(Func<T> func, int priority, OrderType type)
        {
            _func = func;
            _priority = priority;
            _type = type;
        }

        public void OnLateUpdate() => _func?.Invoke();
        public int Priority => _priority;
        public OrderType Type => _type;
    }
}