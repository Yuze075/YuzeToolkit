using System;
using System.Collections.Generic;

namespace YuzeToolkit.Utility
{
    /// <summary>
    /// 状态机接口
    /// </summary>
    public interface IStateMachine
    {
        /// <summary>
        /// 储存不同的状态
        /// </summary>
        Dictionary<Type, IState> States { get; protected internal set; }

        /// <summary>
        /// 当前状态
        /// </summary>
        IState CurrentState { get; protected internal set; }

        /// <summary>
        /// 设置是否在运行
        /// </summary>
        public bool IsRunning { get; protected internal set; }
    }
}