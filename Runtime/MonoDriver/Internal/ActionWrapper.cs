using System;

namespace YuzeToolkit.Utility
{
    internal class UpdateWrapper : IUpdate
    {
        private readonly Action _action;
        private readonly int _priority;
        private readonly OrderType _type;

        public UpdateWrapper(Action action, int priority, OrderType type)
        {
            _action = action;
            _priority = priority;
            _type = type;
        }

        public void OnUpdate() => _action?.Invoke();
        public int Priority => _priority;
        public OrderType Type => _type;
    }

    internal class FixedUpdateWrapper : IFixedUpdate
    {
        private readonly Action _action;
        private readonly int _priority;
        private readonly OrderType _type;

        public FixedUpdateWrapper(Action action, int priority, OrderType type)
        {
            _action = action;
            _priority = priority;
            _type = type;
        }

        public void OnFixedUpdate() => _action?.Invoke();
        public int Priority => _priority;
        public OrderType Type => _type;
    }

    internal class LateUpdateWrapper : ILateUpdate
    {
        private readonly Action _action;
        private readonly int _priority;
        private readonly OrderType _type;

        public LateUpdateWrapper(Action action, int priority, OrderType type)
        {
            _action = action;
            _priority = priority;
            _type = type;
        }

        public void OnLateUpdate() => _action?.Invoke();
        public int Priority => _priority;
        public OrderType Type => _type;
    }
}