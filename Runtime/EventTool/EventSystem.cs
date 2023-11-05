using System;
using System.Collections.Generic;
using UnityEngine;
using YuzeToolkit.InspectorTool;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.EventTool
{
    /// <summary>
    /// 一个简易的事件系统，可以绑定对应类型事件<see cref="RegisterEvent{T}"/>，同时发送对应类型事件<see cref="SendEvent{T}(T)"/><br/>
    /// 继承自<see cref="IDisposable"/>接口，调用<see cref="IDisposable.Dispose()"/>方法可以解除所有事件绑定 todo 解决事件回环的可能问题
    /// </summary>
    [Serializable]
    public class EventSystem : IDisposable, IEvents
    {
        public EventSystem()
        {
        }

        public EventSystem(ILogTool parent) => _sLogTool = new SLogTool(parent);

        private SLogTool? _sLogTool;

        public ILogTool LogTool => _sLogTool ??= new SLogTool(new[]
        {
            nameof(EventSystem)
        });

        private readonly Dictionary<Type, IEventAction> _eventActions = new();
        [SerializeField] [IgnoreParent] private ShowList<ICanEvents> eventsList = new();
        private DisposeGroup _disposeGroup;

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
                LogTool.Log($"{type.Name}类型的eventValue事件值为空！", ELogType.Warning);
                return;
            }

            var eventTypes = new List<Type>
            {
                type
            };

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
                    LogTool.Log($"{type.Name}无法转化为{eventType.Name}类型, 触发事件错误!", ELogType.Error);
                }
            }

            foreach (var events in eventsList)
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
                LogTool.Log($"{eventType.Name}类型的eventValue事件值为空！", ELogType.Warning);
                return;
            }

            if (_eventActions.TryGetValue(eventType, out var eventAction))
            {
                if (eventAction is EventAction<T> tEventAction)
                    tEventAction.Invoke(eventValue);
                else
                {
                    LogTool.Log($"{eventType.Name}和{typeof(T).Name}对应错误, 触发事件错误!", ELogType.Error);
                }
            }

            foreach (var events in eventsList)
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
            eventsList.Add(events);
            return _disposeGroup.UnRegister(() => eventsList.Remove(events));
        }

        public void Dispose()
        {
            foreach (var eventAction in _eventActions.Values)
                eventAction?.Dispose();

            _disposeGroup.Dispose();
            _eventActions.Clear();
            eventsList.Clear();
        }

        private interface IEventAction : IDisposable
        {
            bool Invoke<T>(T value);
        }

        private class EventAction<T> : IEventAction
        {
            private Action<T>? _action;
            private DisposeGroup _disposeGroup;
            public void Invoke(T value) => _action?.Invoke(value);

            public IDisposable Register(Action<T> action)
            {
                _action += action;
                return _disposeGroup.UnRegister(() => _action -= action);
            }

            public void Dispose()
            {
                _disposeGroup.Dispose();
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