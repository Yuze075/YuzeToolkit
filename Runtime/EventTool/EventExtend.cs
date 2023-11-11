using System;

namespace YuzeToolkit.EventTool
{
    public static class EventExtend
    {
        // #region IEventSystemOwner
        //
        // public static IDisposable RegisterEvent<T>(this IEventSystemOwner self, Action<T> onEvent) =>
        //     self.EventSystem.RegisterEvent(onEvent);
        //
        // public static void SendEvent<TBase, T>(this IEventSystemOwner self) where T : TBase, new() =>
        //     self.EventSystem.SendEvent<TBase>(new T());
        //
        // public static void SendEvent<T>(this IEventSystemOwner self) where T : new() =>
        //     self.EventSystem.SendEvent(new T());
        //
        // public static void SendEvent<T>(this IEventSystemOwner self, T eventValue) =>
        //     self.EventSystem.SendEvent(eventValue);
        //
        // public static void SendEventOnce<TBase, T>(this IEventSystemOwner self) where T : TBase, new() =>
        //     self.EventSystem.SendEventOnce<TBase>(new T());
        //
        // public static void SendEventOnce<T>(this IEventSystemOwner self) where T : new() =>
        //     self.EventSystem.SendEventOnce(new T());
        //
        // public static void SendEventOnce<T>(this IEventSystemOwner self, T eventValue) =>
        //     self.EventSystem.SendEventOnce(eventValue);
        //
        // #endregion
    }
}