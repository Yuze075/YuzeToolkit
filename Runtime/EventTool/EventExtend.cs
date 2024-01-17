#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;

namespace YuzeToolkit.EventTool
{
    public static class EventExtend
    {
        #region SendEvent

        /// <summary>
        /// 发送<see cref="T"/>类型的事件(会触发所有可以放入T类型的<see cref="Action{T}"/>函数, 包括注册为T的子类的函数)
        /// </summary>
        /// <param name="self">实体值</param>
        /// <param name="sendToChildrenLayers">向多少层绑定的子<see cref="EventSystem"/>发生事件, 默认为0</param>
        /// <param name="checkEventProcessors">是否触发绑定的<see cref="IEventProcessor"/>的事件检查和触发</param>
        public static void SendEventAll<TBase, T>(this IEventSystemOwner self, int sendToChildrenLayers = 0,
            bool checkEventProcessors = true) where T : TBase, new() =>
            self.EventSystem.SendEventAll<TBase, T>(sendToChildrenLayers, checkEventProcessors);

        /// <summary>
        /// 发送<see cref="T"/>类型的事件(会触发所有可以放入T类型的<see cref="Action{T}"/>函数, 包括注册为T的子类的函数)
        /// </summary>
        /// <param name="self">实体值</param>
        /// <param name="sendToChildrenLayers">向多少层绑定的子<see cref="EventSystem"/>发生事件, 默认为0</param>
        /// <param name="checkEventProcessors">是否触发绑定的<see cref="IEventProcessor"/>的事件检查和触发</param>
        public static void SendEventAll<T>(this IEventSystemOwner self, int sendToChildrenLayers = 0,
            bool checkEventProcessors = true) where T : new() =>
            self.EventSystem.SendEventAll<T>(sendToChildrenLayers, checkEventProcessors);

        /// <summary>
        /// 发送<see cref="T"/>类型的事件(会触发所有可以放入T类型的<see cref="Action{T}"/>函数, 包括注册为T的子类的函数)
        /// </summary>
        /// <param name="self">实体值</param>
        /// <param name="eventValue">实体值</param>
        /// <param name="sendToChildrenLayers">向多少层绑定的子<see cref="EventSystem"/>发生事件, 默认为0</param>
        /// <param name="checkEventProcessors">是否触发绑定的<see cref="IEventProcessor"/>的事件检查和触发</param>
        public static void SendEventAll<T>(this IEventSystemOwner self, T eventValue, int sendToChildrenLayers = 0,
            bool checkEventProcessors = true) =>
            self.EventSystem.SendEventAll(eventValue, sendToChildrenLayers, checkEventProcessors);

        /// <summary>
        /// 发送<see cref="T"/>类型的事件(只会触发注册类型为<see cref="Action{T}"/>类型的函数, 不会触发子类函数)
        /// </summary>
        /// <param name="self">实体值</param>
        /// <param name="sendToChildrenLayers">向多少层绑定的子<see cref="EventSystem"/>发生事件, 默认为0</param>
        /// <param name="checkEventProcessors">是否触发绑定的<see cref="IEventProcessor"/>的事件检查和触发</param>
        public static void SendEvent<TBase, T>(this IEventSystemOwner self, int sendToChildrenLayers = 0,
            bool checkEventProcessors = true) where T : TBase, new() =>
            self.EventSystem.SendEvent<TBase, T>(sendToChildrenLayers, checkEventProcessors);

        /// <summary>
        /// 发送<see cref="T"/>类型的事件(只会触发注册类型为<see cref="Action{T}"/>类型的函数, 不会触发子类函数)
        /// </summary>
        /// <param name="self">实体值</param>
        /// <param name="sendToChildrenLayers">向多少层绑定的子<see cref="EventSystem"/>发生事件, 默认为0</param>
        /// <param name="checkEventProcessors">是否触发绑定的<see cref="IEventProcessor"/>的事件检查和触发</param>
        public static void SendEvent<T>(this IEventSystemOwner self, int sendToChildrenLayers = 0,
            bool checkEventProcessors = true) where T : new() =>
            self.EventSystem.SendEvent<T>(sendToChildrenLayers, checkEventProcessors);

        /// <summary>
        /// 发送<see cref="T"/>类型的事件(只会触发注册类型为<see cref="Action{T}"/>类型的函数, 不会触发子类函数)
        /// </summary>
        /// <param name="self">实体值</param>
        /// <param name="eventValue">事件值</param>
        /// <param name="sendToChildrenLayers">向多少层绑定的子<see cref="EventSystem"/>发生事件, 默认为0</param>
        /// <param name="checkEventProcessors">是否触发绑定的<see cref="IEventProcessor"/>的事件检查和触发</param>
        public static void SendEvent<T>(this IEventSystemOwner self, T eventValue, int sendToChildrenLayers = 0,
            bool checkEventProcessors = true) =>
            self.EventSystem.SendEvent(eventValue, sendToChildrenLayers, checkEventProcessors);

        #endregion

        #region RegisterEvent

        public static void AddEvent<T>(this IEventSystemOwner self, Action<T> onEvent) =>
            self.EventSystem.AddEvent(onEvent);

        [return: NotNullIfNotNull("onEvent")]
        public static IDisposable? RegisterEvent<T>(this IEventSystemOwner self, Action<T>? onEvent) =>
            self.EventSystem.RegisterEvent(onEvent);

        public static void RemoveEvent<T>(this IEventSystemOwner self, Action<T> onEvent) =>
            self.EventSystem.RemoveEvent(onEvent);

        #endregion

        #region RegisterEventSystem

        public static void AddEventSystem(this IEventSystemOwner self, EventSystem eventSystem) =>
            self.EventSystem.AddEventSystem(eventSystem);

        public static IDisposable? RegisterEventSystem(this IEventSystemOwner self, EventSystem? eventSystem) =>
            self.EventSystem.RegisterEventSystem(eventSystem);

        public static bool RemoveEventSystem(this IEventSystemOwner self, EventSystem eventSystem) =>
            self.EventSystem.RemoveEventSystem(eventSystem);

        public static void AddEventSystem(this IEventSystemOwner self, IEventSystemOwner eventSystemOwner) =>
            self.EventSystem.AddEventSystem(eventSystemOwner.EventSystem);

        [return: NotNullIfNotNull("eventSystemOwner")]
        public static IDisposable? RegisterEventSystem(this IEventSystemOwner self, IEventSystemOwner? eventSystemOwner)
            => eventSystemOwner == null ? null : self.EventSystem.RegisterEventSystem(eventSystemOwner.EventSystem);

        public static bool RemoveEventSystem(this IEventSystemOwner self, IEventSystemOwner eventSystemOwner) =>
            self.EventSystem.RemoveEventSystem(eventSystemOwner.EventSystem);

        #endregion

        #region RegisterEventProcessor

        public static void AddEventProcessor(this IEventSystemOwner self, IEventProcessor eventProcessor) =>
            self.EventSystem.AddEventProcessor(eventProcessor);

        [return: NotNullIfNotNull("eventProcessor")]
        public static IDisposable? RegisterEventProcessor(this IEventSystemOwner self, IEventProcessor? eventProcessor)
            => self.EventSystem.RegisterEventProcessor(eventProcessor);

        public static bool RemoveEventProcessor(this IEventSystemOwner self, IEventProcessor eventProcessor) =>
            self.EventSystem.RemoveEventProcessor(eventProcessor);

        #endregion
    }
}