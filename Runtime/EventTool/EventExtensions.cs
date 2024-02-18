#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace YuzeToolkit.EventTool
{
    public static class EventExtensions
    {
        #region IEventNode

        public static void ClearAllEvents(this IEventNode self) => self.EventActions.Clear();

        public static EventSenderAndRegister<T> GetSenderAndRegister<T>(this IEventNode self) => new(self);

        public readonly struct EventSenderAndRegister<T> : IEventSender<T>, IEventRegister<T>
        {
            public EventSenderAndRegister(IEventNode eventNode) => _eventNode = eventNode;
            private readonly IEventNode _eventNode;
            public bool SendEvent(T eventValue) => _eventNode.SendEvent(eventValue);
            public bool SendEventAll(T eventValue) => _eventNode.SendEventAll(eventValue);
            public void AddEvent(Action<T> onEvent) => _eventNode.AddEvent(onEvent);
            public void RemoveEvent(Action<T> onEvent) => _eventNode.RemoveEvent(onEvent);
        }

        #endregion

        #region IEventNodeSender

        /// <summary>
        /// 发送<see cref="T"/>类型的事件(只会触发注册类型为<see cref="Action{T}"/>类型的函数, 不会触发子类函数)
        /// </summary>
        public static bool SendEvent<T>(this IEventNode self, T eventValue)
        {
            if (eventValue == null)
                throw new NullReferenceException($"[{nameof(SendEvent)}]在{self}的{typeof(T)}类型事件不能传入空值!");

            if (self.EventInterceptors != null &&
                self.EventInterceptors.Any(interceptor => !interceptor.CheckEvent(ref eventValue)))
                return false;

            if (self.EventActions.TryGet(typeof(T), out var eventAction))
                ((EventAction<T>)eventAction).Invoke(eventValue);
            return true;
        }

        /// <summary>
        /// 发送<see cref="T"/>类型的事件(只会触发注册类型为<see cref="Action{T}"/>类型的函数, 不会触发子类函数)
        /// </summary>
        public static bool SendEvent<TBase, T>(this IEventNode self) where T : TBase, new() =>
            self.SendEvent<TBase>(new T());

        /// <summary>
        /// 发送<see cref="T"/>类型的事件(只会触发注册类型为<see cref="Action{T}"/>类型的函数, 不会触发子类函数)
        /// </summary>
        public static bool SendEvent<T>(this IEventNode self) where T : new() => self.SendEvent(new T());

        /// <summary>
        /// 发送<see cref="T"/>类型的事件(会触发所有可以放入T类型的<see cref="Action{T}"/>函数, 包括注册为T的子类的函数)
        /// </summary>
        public static bool SendEventAll<T>(this IEventNode self, T eventValue)
        {
            if (eventValue == null)
                throw new NullReferenceException($"[{nameof(SendEventAll)}]在{self}的{typeof(T)}类型事件不能传入空值!");

            if (self.EventInterceptors != null &&
                self.EventInterceptors.Any(interceptor => !interceptor.CheckEvent(ref eventValue)))
                return false;

            var eventType = typeof(T);
            if (self.EventActions.TryGet(eventType, out var eventAction))
                ((EventAction<T>)eventAction).Invoke(eventValue);

            foreach (var interfaceType in eventType.GetInterfaces())
                if (self.EventActions.TryGet(interfaceType, out var interfaceEventAction))
                    interfaceEventAction.Invoke(interfaceEventAction);

            while (eventType.BaseType != null)
                if (self.EventActions.TryGet(eventType = eventType.BaseType, out var baseEventAction))
                    baseEventAction.Invoke(baseEventAction);

            return true;
        }

        /// <summary>
        /// 发送<see cref="T"/>类型的事件(会触发所有可以放入T类型的<see cref="Action{T}"/>函数, 包括注册为T的子类的函数)
        /// </summary>
        public static bool SendEventAll<TBase, T>(this IEventNode self) where T : TBase, new() =>
            self.SendEventAll<TBase>(new T());

        /// <summary>
        /// 发送<see cref="T"/>类型的事件(会触发所有可以放入T类型的<see cref="Action{T}"/>函数, 包括注册为T的子类的函数)
        /// </summary>
        public static bool SendEventAll<T>(this IEventNode self) where T : new() => self.SendEventAll(new T());

        public static EventSender<T> GetSender<T>(this IEventNode self) => new(self);

        public readonly struct EventSender<T> : IEventSender<T>
        {
            public EventSender(IEventNode eventNode) => _eventNode = eventNode;
            private readonly IEventNode _eventNode;
            public bool SendEvent(T eventValue) => _eventNode.SendEvent(eventValue);
            public bool SendEventAll(T eventValue) => _eventNode.SendEventAll(eventValue);
        }

        #endregion

        #region IEventNodeRegister

        public static void AddEvent<T>(this IEventNode self, Action<T>? onEvent)
        {
            if (onEvent == null) return;
            self.EventActions.GetOrCreate<T>().Action += onEvent;
        }

        public static void RemoveEvent<T>(this IEventNode self, Action<T>? onEvent)
        {
            if (onEvent == null) return;
            self.EventActions.GetOrCreate<T>().Action -= onEvent;
        }

        [return: NotNullIfNotNull("onEvent")]
        public static IDisposable? RegisterEvent<T>(this IEventNode self, Action<T>? onEvent)
        {
            if (onEvent == null) return null;
            self.AddEvent(onEvent);
            return DisposableExtensions.UnRegister(self.RemoveEvent, onEvent);
        }

        [return: NotNullIfNotNull("eventHandler")]
        public static IDisposable? RegisterEvent<T>(this IEventNode self,
            IEventHandler<T>? eventHandler)
        {
            if (eventHandler == null) return null;
            self.AddEvent<T>(eventHandler.HandlerEvent);
            return DisposableExtensions.UnRegister(self.RemoveEvent, new Action<T>(eventHandler.HandlerEvent));
        }

        public static EventRegister<T> GetRegister<T>(this IEventNode self) => new(self);

        public readonly struct EventRegister<T> : IEventRegister<T>
        {
            public EventRegister(IEventNode eventNode) => _eventNode = eventNode;
            private readonly IEventNode _eventNode;
            public void AddEvent(Action<T> onEvent) => _eventNode.AddEvent(onEvent);
            public void RemoveEvent(Action<T> onEvent) => _eventNode.RemoveEvent(onEvent);
        }

        #endregion

        #region IEventSender

        /// <summary>
        /// 发送<see cref="T"/>类型的事件(会触发所有可以放入T类型的<see cref="Action{T}"/>函数, 包括注册为T的子类的函数)
        /// </summary>
        public static void SendEventAll<TBase, T>(this IEventSender<TBase> self) where T : TBase, new() =>
            self.SendEventAll(new T());

        /// <summary>
        /// 发送<see cref="T"/>类型的事件(会触发所有可以放入T类型的<see cref="Action{T}"/>函数, 包括注册为T的子类的函数)
        /// </summary>
        public static void SendEventAll<T>(this IEventSender<T> self) where T : new() => self.SendEventAll(new T());

        /// <summary>
        /// 发送<see cref="T"/>类型的事件(只会触发注册类型为<see cref="Action{T}"/>类型的函数, 不会触发子类函数)
        /// </summary>
        public static void SendEvent<TBase, T>(this IEventSender<TBase> self) where T : TBase, new() =>
            self.SendEvent(new T());

        /// <summary>
        /// 发送<see cref="T"/>类型的事件(只会触发注册类型为<see cref="Action{T}"/>类型的函数, 不会触发子类函数)
        /// </summary>
        public static void SendEvent<T>(this IEventSender<T> self) where T : new() => self.SendEvent(new T());

        #endregion

        #region IEventRegister

        [return: NotNullIfNotNull("onEvent")]
        public static IDisposable? RegisterEvent<T>(this IEventRegister<T> self, Action<T>? onEvent)
        {
            if (onEvent == null) return null;
            self.AddEvent(onEvent);
            return DisposableExtensions.UnRegister(self.RemoveEvent, onEvent);
        }

        public static void AddEvent<T>(this IEventRegister<T> self, IEventHandler<T>? eventHandler)
        {
            if (eventHandler == null) return;
            self.AddEvent(eventHandler.HandlerEvent);
        }

        [return: NotNullIfNotNull("eventHandler")]
        public static IDisposable? RegisterEvent<T>(this IEventRegister<T> self, IEventHandler<T>? eventHandler)
        {
            if (eventHandler == null) return null;
            self.AddEvent(eventHandler.HandlerEvent);
            return DisposableExtensions.UnRegister(self.RemoveEvent, new Action<T>(eventHandler.HandlerEvent));
        }

        public static void RemoveEvent<T>(this IEventRegister<T> self, IEventHandler<T>? eventHandler)
        {
            if (eventHandler == null) return;
            self.RemoveEvent(eventHandler.HandlerEvent);
        }

        #endregion
    }
}