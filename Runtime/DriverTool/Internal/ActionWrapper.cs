#nullable enable
using System;

namespace YuzeToolkit.DriverTool
{
    internal class ActionUpdateWrapper : IUpdate
    {
        public ActionUpdateWrapper(Action action, int priority, EOrderType updateOrderType)
        {
            _action = action;
            _updatePriority = priority;
            _updateOrderType = updateOrderType;
        }

        public void OnUpdate() => _action();
        private readonly Action _action;
        private readonly int _updatePriority;
        private readonly EOrderType _updateOrderType;
        int IMonoBase.UpdatePriority => _updatePriority;
        EOrderType IMonoBase.UpdateOrderType => _updateOrderType;
    }

    internal class ActionFixedUpdateWrapper : IFixedUpdate
    {
        public ActionFixedUpdateWrapper(Action action, int priority, EOrderType updateOrderType)
        {
            _action = action;
            _updatePriority = priority;
            _updateOrderType = updateOrderType;
        }

        public void OnFixedUpdate() => _action();
        private readonly Action _action;
        private readonly int _updatePriority;
        private readonly EOrderType _updateOrderType;
        int IMonoBase.UpdatePriority => _updatePriority;
        EOrderType IMonoBase.UpdateOrderType => _updateOrderType;
    }

    internal class ActionLateUpdateWrapper : ILateUpdate
    {
        public ActionLateUpdateWrapper(Action action, int priority, EOrderType updateOrderType)
        {
            _action = action;
            _updatePriority = priority;
            _updateOrderType = updateOrderType;
        }

        public void OnLateUpdate() => _action();
        private readonly Action _action;
        private readonly int _updatePriority;
        private readonly EOrderType _updateOrderType;
        int IMonoBase.UpdatePriority => _updatePriority;
        EOrderType IMonoBase.UpdateOrderType => _updateOrderType;
    }
}