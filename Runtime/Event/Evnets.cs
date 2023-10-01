using System;
using System.Collections.Generic;
using UnityEngine;
using YuzeToolkit.Utility;
using ILogger = YuzeToolkit.Log.ILogger;
using Logger = YuzeToolkit.Log.Logger;

namespace YuzeToolkit.Event
{
    /// <summary>
    /// 一个简易的事件系统，可以绑定对应类型事件<see cref="RegisterEvent{T}"/>，同时发送对应类型事件<see cref="SendEvent{T}(T)"/><br/>
    /// 继承自<see cref="IDisposable"/>接口，调用<see cref="IDisposable.Dispose()"/>方法可以解除所有事件绑定
    /// </summary>
    public class Events : IDisposable, IEvents
    {
        public Events()
        {
            Logger.DefaultTags = new[] { "Events" };
        }

        public Events(ILogger logger)
        {
            Logger.Parent = logger;
            Logger.DefaultTags = new[] { "Events" };
        }

        public readonly Logger Logger = new();
        private readonly Dictionary<Type, IEventAction> _eventActions = new();
        private readonly List<ICanEvents> _eventsList = new();
        private readonly List<IDisposable> _unRegisters = new();

        public void SendEvent<TBase, T>() where T : TBase, new() => SendEvent<TBase>(new T(), typeof(TBase));
        public void SendEvent<T>() where T : new() => SendEvent(new T(), typeof(T));
        public void SendEvent<T>(T eventValue) => SendEvent(eventValue, typeof(T));

        public void SendEventOnce<TBase, T>() where T : TBase, new() => SendEventOnce<TBase>(new T(), typeof(TBase));
        public void SendEventOnce<T>() where T : new() => SendEventOnce(new T(), typeof(T));
        public void SendEventOnce<T>(T eventValue) => SendEventOnce(eventValue, typeof(T));

        private void SendEvent<T>(T eventValue, Type type)
        {
            if (eventValue == null)
            {
                Logger.Log($"{type.Name}类型的eventValue事件值为空！", LogType.Warning);
                return;
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
                if (!_eventActions.TryGetValue(eventType, out var eventAction)) continue;
                if (!eventAction.Invoke(eventValue))
                {
                    Logger.Log($"{type.Name}无法转化为{eventType.Name}类型, 触发事件错误!", LogType.Error);
                }
            }

            foreach (var events in _eventsList)
            {
                events.SendEvent(eventValue);
            }

            if (eventValue is ICustomEvent customEventType)
                customEventType.DoCustomEvent(this);
        }

        private void SendEventOnce<T>(T eventValue, Type eventType)
        {
            if (eventValue == null)
            {
                Logger.Log($"{eventType.Name}类型的eventValue事件值为空！", LogType.Warning);
                return;
            }

            if (_eventActions.TryGetValue(eventType, out var eventAction))
            {
                if (eventAction is EventAction<T> tEventAction)
                    tEventAction.Invoke(eventValue);
                else
                {
                    Logger.Log($"{eventType.Name}和{typeof(T).Name}对应错误, 触发事件错误!", LogType.Error);
                }
            }

            foreach (var events in _eventsList)
            {
                events.SendEventOnce(eventValue);
            }
        }

        public IDisposable RegisterEvent<T>(Action<T> onEvent)
        {
            var type = typeof(T);
            if (_eventActions.TryGetValue(type, out var eventAction))
            {
                return ((EventAction<T>)eventAction).Register(onEvent);
            }

            var tEventAction = new EventAction<T>();
            _eventActions.Add(type, tEventAction);
            return tEventAction.Register(onEvent);
        }

        public IDisposable RegisterEvent(ICanEvents events)
        {
            if (events == null || events == this) return new UnRegister(null);

            _eventsList.Add(events);
            var unRegister = new UnRegister(() => _eventsList.Remove(events));
            _unRegisters.Add(unRegister);
            return unRegister;
        }

        public void Dispose()
        {
            foreach (var eventAction in _eventActions.Values)
                eventAction?.Dispose();

            foreach (var disposable in _unRegisters)
                disposable?.Dispose();

            _eventActions.Clear();
            _unRegisters.Clear();
            _eventsList.Clear();
        }

        private interface IEventAction : IDisposable
        {
            bool Invoke<T>(T value);
        }

        private class EventAction<T> : IEventAction
        {
            private Action<T> _action;
            private readonly List<IDisposable> _unRegisters = new();

            public void Invoke(T value) => _action?.Invoke(value);

            public IDisposable Register(Action<T> action)
            {
                _action += action;
                var unRegister = new UnRegister(() => _action -= action);
                _unRegisters.Add(unRegister);
                return unRegister;
            }

            public void Dispose()
            {
                foreach (var disposable in _unRegisters)
                {
                    disposable?.Dispose();
                }

                _unRegisters.Clear();
                _action = null;
            }
            
            bool IEventAction.Invoke<TValue>(TValue value)
            {
                if (value is not T tValue) return false;

                Invoke(tValue);
                return true;
            }
        }
    }
}