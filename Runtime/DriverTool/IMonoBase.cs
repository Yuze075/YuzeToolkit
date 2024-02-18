#nullable enable
using System;

namespace YuzeToolkit.DriverTool
{
    /// <summary>
    /// 在Unity的更新逻辑中的优先级
    /// </summary>
    public enum EOrderType
    {
        /// <summary>
        /// 在所有unity更新逻辑之前（除了UnityEvent
        /// </summary>
        First,

        /// <summary>
        /// 在默认的更新逻辑之前
        /// </summary>
        Before,

        /// <summary>
        /// 在默认的更新逻辑之后
        /// </summary>
        After,

        /// <summary>
        /// 在所有unity更新逻辑之后
        /// </summary>
        End
    }

    public struct UpdateToken : IDisposable
    {
        public UpdateToken(IDisposable? update, IDisposable? fixedUpdate, IDisposable? lateUpdate)
        {
            _update = update;
            _fixedUpdate = fixedUpdate;
            _lateUpdate = lateUpdate;
        }

        private IDisposable? _update;
        private IDisposable? _fixedUpdate;
        private IDisposable? _lateUpdate;

        public void Dispose()
        {
            if (_update != null)
            {
                _update.Dispose();
                _update = null;
            }

            if (_fixedUpdate != null)
            {
                _fixedUpdate.Dispose();
                _fixedUpdate = null;
            }

            if (_lateUpdate != null)
            {
                _lateUpdate.Dispose();
                _lateUpdate = null;
            }
        }
    }

    /// <summary>
    /// 所有需要<see cref="MonoDriverBase"/>统一更新函数的接口
    /// </summary>
    public interface IMonoBase
    {
        static float DeltaTime;
        static float FixedDeltaTime;

        /// <summary>
        /// 更新类型, 在不同Unity的更新顺序中更新
        /// </summary>
        EOrderType UpdateOrderType => EOrderType.Before;

        /// <summary>
        /// 更新优先级, 越小越早更新
        /// </summary>
        int UpdatePriority => 0;
    }

    /// <summary>
    /// 绑定基于<see cref="MonoDriverBase"/>驱动更新的<see cref="MonoDriverBase.Update"/>函数
    /// </summary>
    public interface IUpdate : IMonoBase
    {
        /// <summary>
        /// 在<see cref="MonoDriverBase.Update"/>更新的函数
        /// </summary>
        void OnUpdate();
    }

    /// <summary>
    /// 绑定基于<see cref="MonoDriverBase"/>驱动更新的<see cref="MonoDriverBase.FixedUpdate"/>函数
    /// </summary>
    public interface IFixedUpdate : IMonoBase
    {
        /// <summary>
        /// 在<see cref="MonoDriverBase.FixedUpdate"/>更新的函数
        /// </summary>
        void OnFixedUpdate();
    }

    /// <summary>
    /// 绑定基于<see cref="MonoDriverBase"/>驱动更新的<see cref="MonoDriverBase.LateUpdate"/>函数
    /// </summary>
    public interface ILateUpdate : IMonoBase
    {
        /// <summary>
        /// 在<see cref="MonoDriverBase.LateUpdate"/>更新的函数
        /// </summary>
        void OnLateUpdate();
    }
}