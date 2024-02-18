#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace YuzeToolkit.EventTool
{
    public struct EventActionDictionary
    {
        private Dictionary<Type, IEventAction>? _eventActions;

        internal bool TryGet(Type eventType, [MaybeNullWhen(false)] out IEventAction eventAction)
        {
            if (_eventActions != null) return _eventActions.TryGetValue(eventType, out eventAction);
            eventAction = null;
            return false;
        }

        internal EventAction<T> GetOrCreate<T>()
        {
            _eventActions ??= DictionaryPool<Type, IEventAction>.Get();
            if (_eventActions.TryGetValue(typeof(T), out var eventAction)) return (EventAction<T>)eventAction;
            return (EventAction<T>)(_eventActions[typeof(T)] = new EventAction<T>());
        }

        internal void Clear()
        {
            if (_eventActions == null) return;
            foreach (var eventAction in _eventActions.Values) eventAction.Reset();
            DictionaryPool<Type, IEventAction>.RefRelease(ref _eventActions);
        }
    }
}