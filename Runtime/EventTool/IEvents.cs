using System;

namespace YuzeToolkit.EventTool
{
    /// <summary>
    /// 能注册和发送事件的基类接口
    /// </summary>
    public interface IEvents : ICanEvents
    {
        /// <summary>
        /// 注册子的<see cref="ICanEvents"/>, 当前的<see cref="IEvents"/>触发<see cref="ICanEvents.SendEvent{T}()"/>的时候,
        /// 会向所有绑定的子<see cref="ICanEvents"/>发送相同的事件
        /// </summary>
        IDisposable RegisterEvent(ICanEvents events);
    }
}