#nullable enable
using System;

namespace YuzeToolkit.DriverTool
{
    internal class ActionUpdateWrapper : IUpdate
    {
        public ActionUpdateWrapper(Action action, int priority, EOrderType type)
        {
            _action = action;
            UpdatePriority = priority;
            Type = type;
        }

        public void OnUpdate() => _action();
        private readonly Action _action;
        public int UpdatePriority { get; }
        public EOrderType Type { get; }
    }

    internal class ActionFixedUpdateWrapper : IFixedUpdate
    {
        public ActionFixedUpdateWrapper(Action action, int priority, EOrderType type)
        {
            _action = action;
            UpdatePriority = priority;
            Type = type;
        }

        public void OnFixedUpdate() => _action();
        private readonly Action _action;
        public int UpdatePriority { get; }
        public EOrderType Type { get; }
    }

    internal class ActionLateUpdateWrapper : ILateUpdate
    {
        public ActionLateUpdateWrapper(Action action, int priority, EOrderType type)
        {
            _action = action;
            UpdatePriority = priority;
            Type = type;
        }

        public void OnLateUpdate() => _action();
        private readonly Action _action;
        public int UpdatePriority { get; }
        public EOrderType Type { get; }
    }
}