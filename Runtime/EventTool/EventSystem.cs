#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using YuzeToolkit.InspectorTool;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.EventTool
{
    /// <summary>
    /// 一个简易的事件系统，可以绑定对应类型事件<see cref="RegisterEvent{T}"/>，同时发送对应类型事件<see cref="SendEventAll{T}(T,int,bool)"/><br/>
    /// 继承自<see cref="IDisposable"/>接口，调用<see cref="IDisposable.Dispose()"/>方法可以解除所有事件绑定 todo 解决事件回环的可能问题
    /// </summary>
#if UNITY_EDITOR && YUZE_INSPECTOR_TOOL_USE_SHOW_VALUE && YUZE_EVENT_TOOL_USE_SHOW_VALUE
    [Serializable]
#endif
    public sealed class EventSystem : IDisposable
    {
        private static string[] s_eventSystemTags = { nameof(EventSystem) };
        public EventSystem()
        {
        }

        public EventSystem(ILogging? loggingParent)
        {
            Logging = new Logging(s_eventSystemTags, loggingParent);
        }

        ~EventSystem() => Dispose(false);
        [NonSerialized] private bool _disposed;
        private Logging Logging { get; set; }

        private Dictionary<Type, IEventAction>? _eventActions;
        private Dictionary<Type, IEventAction> EventActions => _eventActions ??= new Dictionary<Type, IEventAction>();

#if UNITY_EDITOR && YUZE_INSPECTOR_TOOL_USE_SHOW_VALUE && YUZE_EVENT_TOOL_USE_SHOW_VALUE
        [Title("事件处理器")] [SerializeField] [IgnoreParent]
        private ShowList<IEventProcessor> eventProcessors;

        [Title("子事件系统")] [SerializeField] [IgnoreParent]
        private ShowList<EventSystem> childrenEventSystems;

        private List<IEventProcessor> EventProcessors => eventProcessors.NativeList;
        private List<EventSystem> ChildrenEventSystems => childrenEventSystems.NativeList;
#else
        private List<IEventProcessor>? eventProcessors;
        private List<EventSystem>? childrenEventSystems;
        private List<IEventProcessor> EventProcessors => eventProcessors ??= new List<IEventProcessor>();
        private List<EventSystem> ChildrenEventSystems => childrenEventSystems ??= new List<EventSystem>();
#endif


        #region SendEvent

        /// <summary>
        /// 发送<see cref="T"/>类型的事件(会触发所有可以放入T类型的<see cref="Action{T}"/>函数, 包括注册为T的子类的函数)
        /// </summary>
        /// <param name="sendToChildrenLayers">向多少层绑定的子<see cref="EventSystem"/>发生事件, 默认为0</param>
        /// <param name="checkEventProcessors">是否触发绑定的<see cref="IEventProcessor"/>的事件检查和触发</param>
        public void SendEventAll<TBase, T>(int sendToChildrenLayers = 0, bool checkEventProcessors = true)
            where T : TBase, new() =>
            SendEventAll<TBase>(new T(), typeof(TBase), sendToChildrenLayers, checkEventProcessors);

        /// <summary>
        /// 发送<see cref="T"/>类型的事件(会触发所有可以放入T类型的<see cref="Action{T}"/>函数, 包括注册为T的子类的函数)
        /// </summary>
        /// <param name="sendToChildrenLayers">向多少层绑定的子<see cref="EventSystem"/>发生事件, 默认为0</param>
        /// <param name="checkEventProcessors">是否触发绑定的<see cref="IEventProcessor"/>的事件检查和触发</param>
        public void SendEventAll<T>(int sendToChildrenLayers = 0, bool checkEventProcessors = true) where T : new() =>
            SendEventAll(new T(), typeof(T), sendToChildrenLayers, checkEventProcessors);

        /// <summary>
        /// 发送<see cref="T"/>类型的事件(会触发所有可以放入T类型的<see cref="Action{T}"/>函数, 包括注册为T的子类的函数)
        /// </summary>
        /// <param name="eventValue">事件值</param>
        /// <param name="sendToChildrenLayers">向多少层绑定的子<see cref="EventSystem"/>发生事件, 默认为0</param>
        /// <param name="checkEventProcessors">是否触发绑定的<see cref="IEventProcessor"/>的事件检查和触发</param>
        public void SendEventAll<T>(T eventValue, int sendToChildrenLayers = 0, bool checkEventProcessors = true) =>
            SendEventAll(eventValue, typeof(T), sendToChildrenLayers, checkEventProcessors);

        /// <summary>
        /// 发送<see cref="T"/>类型的事件(只会触发注册类型为<see cref="Action{T}"/>类型的函数, 不会触发子类函数)
        /// </summary>
        /// <param name="sendToChildrenLayers">向多少层绑定的子<see cref="EventSystem"/>发生事件, 默认为0</param>
        /// <param name="checkEventProcessors">是否触发绑定的<see cref="IEventProcessor"/>的事件检查和触发</param>
        public void SendEvent<TBase, T>(int sendToChildrenLayers = 0, bool checkEventProcessors = true)
            where T : TBase, new() =>
            SendEvent<TBase>(new T(), typeof(TBase), sendToChildrenLayers, checkEventProcessors);

        /// <summary>
        /// 发送<see cref="T"/>类型的事件(只会触发注册类型为<see cref="Action{T}"/>类型的函数, 不会触发子类函数)
        /// </summary>
        /// <param name="sendToChildrenLayers">向多少层绑定的子<see cref="EventSystem"/>发生事件, 默认为0</param>
        /// <param name="checkEventProcessors">是否触发绑定的<see cref="IEventProcessor"/>的事件检查和触发</param>
        public void SendEvent<T>(int sendToChildrenLayers = 0, bool checkEventProcessors = true) where T : new() =>
            SendEvent(new T(), typeof(T), sendToChildrenLayers, checkEventProcessors);

        /// <summary>
        /// 发送<see cref="T"/>类型的事件(只会触发注册类型为<see cref="Action{T}"/>类型的函数, 不会触发子类函数)
        /// </summary>
        /// <param name="eventValue">事件值</param>
        /// <param name="sendToChildrenLayers">向多少层绑定的子<see cref="EventSystem"/>发生事件, 默认为0</param>
        /// <param name="checkEventProcessors">是否触发绑定的<see cref="IEventProcessor"/>的事件检查和触发</param>
        public void SendEvent<T>(T eventValue, int sendToChildrenLayers = 0, bool checkEventProcessors = true) =>
            SendEvent(eventValue, typeof(T), sendToChildrenLayers, checkEventProcessors);

        private void SendEventAll<T>(T eventValue, Type type, int sendToChildrenLayers, bool checkEventProcessors)
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            if (eventValue == null)
            {
                Logging.LogWarning($"{type.Name}类型的eventValue事件值为空！");
                return;
            }

            if (checkEventProcessors && eventProcessors != default)
            {
                var eventHandlersCount = eventProcessors.Count;
                for (var index = 0; index < eventHandlersCount; index++)
                {
                    if (eventProcessors[index] is IBeforeEventProcessor beforeEventProcessor &&
                        !beforeEventProcessor.BeforeHandlerEvent(ref eventValue)) return;
                }

                for (var index = 0; index < eventHandlersCount; index++)
                    eventProcessors[index].HandleEvent(eventValue);
            }

            var eventTypes = new List<Type> { type };

            eventTypes.AddRange(type.GetInterfaces());
            while (type.BaseType != null)
            {
                eventTypes.Add(type.BaseType);
                type = type.BaseType;
            }

            foreach (var eventType in eventTypes)
            {
                if (_eventActions == null || !_eventActions.TryGetValue(eventType, out var eventAction))
                    continue;

                if (!eventAction.Invoke(eventValue))
                    Logging.LogError($"{type.Name}无法转化为{eventType.Name}类型, 触发事件错误!");
            }

            if (sendToChildrenLayers <= 0) return;
            sendToChildrenLayers--;

            if (childrenEventSystems == default) return;
            foreach (var eventSystem in childrenEventSystems)
                eventSystem.SendEvent(eventValue, type, sendToChildrenLayers, checkEventProcessors);
        }

        private void SendEvent<T>(T eventValue, Type eventType, int sendToChildrenLayers, bool checkEventProcessors)
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            if (eventValue == null)
            {
                Logging.LogWarning($"{eventType.Name}类型的eventValue事件值为空！");
                return;
            }

            if (checkEventProcessors && eventProcessors != default)
            {
                var eventHandlersCount = eventProcessors.Count;
                for (var index = 0; index < eventHandlersCount; index++)
                {
                    if (eventProcessors[index] is IBeforeEventProcessor beforeEventProcessor &&
                        !beforeEventProcessor.BeforeHandlerEvent(ref eventValue)) return;
                }

                for (var index = 0; index < eventHandlersCount; index++)
                    eventProcessors[index].HandleEvent(eventValue);
            }

            if (_eventActions != null && _eventActions.TryGetValue(eventType, out var eventAction))
            {
                if (eventAction is EventAction<T> tEventAction) tEventAction.Invoke(eventValue);
                else Logging.LogError($"{eventType.Name}和{typeof(T).Name}对应错误, 触发事件错误!");
            }

            if (sendToChildrenLayers <= 0) return;
            sendToChildrenLayers--;

            if (childrenEventSystems == default) return;
            foreach (var eventSystem in childrenEventSystems)
                eventSystem.SendEvent(eventValue, eventType, sendToChildrenLayers, checkEventProcessors);
        }

        #endregion

        #region RegisterEvent

        public void AddEvent<T>(Action<T> onEvent)
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            var type = typeof(T);
            if (EventActions.TryGetValue(type, out var eventAction))
            {
                ((EventAction<T>)eventAction).Add(onEvent);
                return;
            }

            var tEventAction = new EventAction<T>();
            EventActions.Add(type, tEventAction);
            tEventAction.Add(onEvent);
        }

        [return: NotNullIfNotNull("onEvent")]
        public IDisposable? RegisterEvent<T>(Action<T>? onEvent)
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            var type = typeof(T);
            if (EventActions.TryGetValue(type, out var eventAction))
            {
                return ((EventAction<T>)eventAction).Register(onEvent);
            }

            var tEventAction = new EventAction<T>();
            EventActions.Add(type, tEventAction);
            return tEventAction.Register(onEvent);
        }

        public void RemoveEvent<T>(Action<T> onEvent)
        {
            if (_disposed) return;
            if (_eventActions != default && _eventActions.TryGetValue(typeof(T), out var eventAction))
                ((EventAction<T>)eventAction).Remove(onEvent);
        }

        #endregion

        #region RegisterEventSystem

        public void AddEventSystem(EventSystem eventSystem)
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            ChildrenEventSystems.Add(eventSystem);
        }

        [return: NotNullIfNotNull("eventSystem")]
        public IDisposable? RegisterEventSystem(EventSystem? eventSystem)
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            if (eventSystem == null) return null;
            ChildrenEventSystems.Add(eventSystem);
            return UnRegister.Create(ChildrenEventSystems.Remove, eventSystem);
        }

        public bool RemoveEventSystem(EventSystem eventSystem)
        {
            if (_disposed) return false;
            return childrenEventSystems != default && childrenEventSystems.Remove(eventSystem);
        }

        #endregion

        #region RegisterEventProcessor

        public void AddEventProcessor(IEventProcessor eventProcessor)
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            EventProcessors.Add(eventProcessor);
        }

        [return: NotNullIfNotNull("eventProcessor")]
        public IDisposable? RegisterEventProcessor(IEventProcessor? eventProcessor)
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            if (eventProcessor == null) return null;
            EventProcessors.Add(eventProcessor);
            return UnRegister.Create(EventProcessors.Remove, eventProcessor);
        }

        public bool RemoveEventProcessor(IEventProcessor eventProcessor)
        {
            if (_disposed) return false;
            return eventProcessors != default && eventProcessors.Remove(eventProcessor);
        }

        #endregion

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool _)
        {
            if (_disposed) return;
            if (_eventActions != null)
            {
                foreach (var eventAction in _eventActions.Values) eventAction.Clear();
                _eventActions.Clear();
                _eventActions = null;
            }

            if (eventProcessors != default) eventProcessors.Clear();
            if (childrenEventSystems != default) childrenEventSystems.Clear();
            _disposed = true;
        }

        #region class

        private interface IEventAction
        {
            bool Invoke<T>(T value);
            void Clear();
        }

        private sealed class EventAction<T> : IEventAction
        {
            public bool Invoke<TValue>(TValue value)
            {
                if (value is not T tValue) return false;
                _action?.Invoke(tValue);
                return true;
            }

            private Action<T>? _action;
            public void Add(Action<T> action) => _action += action;
            public void Remove(Action<T> action) => _action -= action;

            [return: NotNullIfNotNull("action")]
            public IDisposable? Register(Action<T>? action)
            {
                if (action == null) return null;
                _action += action;
                return UnRegister.Create(Remove, action);
            }

            public void Clear() => _action = null;
        }

        #endregion
    }
}