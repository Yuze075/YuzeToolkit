#nullable enable
using System;

namespace YuzeToolkit.EventTool
{
    public sealed class EventAction<T> : IEventAction
    {
        internal event Action<T>? Action;
        internal void Invoke(T value) => Action?.Invoke(value);

        void IEventAction.Invoke<TValue>(TValue value)
        {
            if (value is not T eventValue) throw new InvalidCastException($"无法将{value}转换为{typeof(T)}类型!");
            Action?.Invoke(eventValue);
        }

        void IEventAction.Reset() => Action = null;
    }

    public interface IEventAction
    {
        protected internal void Invoke<TValue>(TValue value);
        void Reset();
    }
}