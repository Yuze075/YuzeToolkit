#nullable enable
using System;

namespace YuzeToolkit.EventTool
{
    public interface IEventInterceptor
    {
        /// <summary>
        /// 用于拦截事件，如果返回值未null则不会触发事件
        /// </summary>
        /// <param name="eventValue">事件泛型值, 可以通过类型转换知道具体为什么类型的值(不为空)</param>
        /// <returns>返回true则继续执行事件，否则不执行</returns>
        bool CheckEvent<TEvent>(ref TEvent eventValue);
    }

    public interface IEventHandler<in T>
    {
        void HandlerEvent(T eventValue);
    }

    public interface IEventSender<in T>
    {
        bool SendEvent(T eventValue);
        bool SendEventAll(T eventValue);
    }

    public interface IEventRegister<out T>
    {
        void AddEvent(Action<T> onEvent);
        void RemoveEvent(Action<T> onEvent);
    }
}