using System;

namespace YuzeToolkit.DriverTool
{
    internal class ActionUpdateWrapper : IUpdate
    {
        private readonly Action _action;

        public ActionUpdateWrapper(Action action, int priority, OrderType type)
        {
            _action = action;
            UpdatePriority = priority;
            Type = type;
        }

        public void OnUpdate() => _action();
        public int UpdatePriority { get; }
        public OrderType Type { get; }
    }

    internal class ActionFixedUpdateWrapper : IFixedUpdate
    {
        private readonly Action _action;

        public ActionFixedUpdateWrapper(Action action, int priority, OrderType type)
        {
            _action = action;
            UpdatePriority = priority;
            Type = type;
        }

        public void OnFixedUpdate() => _action.Invoke();
        public int UpdatePriority { get; }
        public OrderType Type { get; }
    }

    internal class ActionLateUpdateWrapper : ILateUpdate
    {
        private readonly Action _action;

        public ActionLateUpdateWrapper(Action action, int priority, OrderType type)
        {
            _action = action;
            UpdatePriority = priority;
            Type = type;
        }

        public void OnLateUpdate() => _action.Invoke();
        public int UpdatePriority { get; }
        public OrderType Type { get; }
    }
}