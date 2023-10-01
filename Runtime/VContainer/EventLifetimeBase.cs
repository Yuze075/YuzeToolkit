#if VContainer
using System;
using YuzeToolkit.Event;

namespace YuzeToolkit.Utility
{
    public abstract class EventLifetimeBase : LifeTimeBase, IEvents
    {
        #region Event

        private readonly Events _events = new();
        public void SendEvent<TBase, T>() where T : TBase, new() => SendEventPrivate<TBase>(new T());
        public void SendEvent<T>() where T : new() => SendEventPrivate(new T());
        public void SendEvent<T>(T eventValue) => SendEventPrivate(eventValue);
        public void SendEventOnce<TBase, T>() where T : TBase, new() => SendEventOncePrivate<TBase>(new T());
        public void SendEventOnce<T>() where T : new() => SendEventOncePrivate(new T());
        public void SendEventOnce<T>(T eventValue) => SendEventOncePrivate(eventValue);
        public IDisposable RegisterEvent<T>(Action<T> onEvent) => _events.RegisterEvent(onEvent);
        public IDisposable RegisterEvent(ICanEvents events) => _events.RegisterEvent(events);

        /// <summary>
        /// 可以拦截对应的Event事件
        /// </summary>
        protected virtual void SendEventPrivate<TEvent>(TEvent eventValue) => _events.SendEvent(eventValue);

        /// <summary>
        /// 可以拦截对应的Event事件
        /// </summary>
        protected virtual void SendEventOncePrivate<TEvent>(TEvent eventValue) => _events.SendEventOnce(eventValue);

        #endregion
    }
}
#endif