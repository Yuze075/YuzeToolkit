using System;

namespace YuzeToolkit.MonoDriver
{
    internal class ActionUpdateWrapper : IUpdate
    {
        private readonly Action _action;
        private readonly int _priority;
        private readonly OrderType _type;

        public ActionUpdateWrapper(Action action, int priority, OrderType type)
        {
            _action = action;
            _priority = priority;
            _type = type;
        }

        public void OnUpdate() => _action?.Invoke();
        public int Priority => _priority;
        public OrderType Type => _type;
    }

    internal class ActionFixedUpdateWrapper : IFixedUpdate
    {
        private readonly Action _action;
        private readonly int _priority;
        private readonly OrderType _type;

        public ActionFixedUpdateWrapper(Action action, int priority, OrderType type)
        {
            _action = action;
            _priority = priority;
            _type = type;
        }

        public void OnFixedUpdate() => _action?.Invoke();
        public int Priority => _priority;
        public OrderType Type => _type;
    }

    internal class ActionLateUpdateWrapper : ILateUpdate
    {
        private readonly Action _action;
        private readonly int _priority;
        private readonly OrderType _type;

        public ActionLateUpdateWrapper(Action action, int priority, OrderType type)
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