using System;

namespace YuzeToolkit.Event
{
    /// <summary>
    /// 能注册和发送事件的基类接口
    /// </summary>
    public interface ICanEvents
    {
        /// <summary>
        /// 发送事件，会将事件发送到所有可以接受的函数中<br/>
        /// 例如：<code>Send(UIModelBase)</code>也会触发绑定为<code>Action{IUIModel}</code>的函数(注: UIModelBase继承自IUIModel
        /// </summary>
        void SendEvent<TBase, T>() where T : TBase, new();

        /// <summary>
        /// 发送事件，会将事件发送到所有可以接受的函数中<br/>
        /// 例如：<code>Send(UIModelBase)</code>也会触发绑定为<code>Action{IUIModel}</code>的函数(注: UIModelBase继承自IUIModel
        /// </summary>
        void SendEvent<T>() where T : new();

        /// <summary>
        /// 发送事件，会将事件发送到所有可以接受的函数中<br/>
        /// 例如：<code>Send(UIModelBase)</code>也会触发绑定为<code>Action{IUIModel}</code>的函数(注: UIModelBase继承自IUIModel
        /// </summary>
        void SendEvent<T>(T eventValue);

        /// <summary>
        /// 发送事件，只将事件发送到当前泛型类的函数中<br/>
        /// 例如：<code>Send(UIModelBase)</code>只会触发绑定为<code>Action{UIModelBase}</code>的函数
        /// </summary>
        void SendEventOnce<TBase, T>() where T : TBase, new();

        /// <summary>
        /// 发送事件，只将事件发送到当前泛型类的函数中<br/>
        /// 例如：<code>Send(UIModelBase)</code>只会触发绑定为<code>Action{UIModelBase}</code>的函数
        /// </summary>
        void SendEventOnce<T>() where T : new();

        /// <summary>
        /// 发送事件，只将事件发送到当前泛型类的函数中<br/>
        /// 例如：<code>Send(UIModelBase)</code>只会触发绑定为<code>Action{UIModelBase}</code>的函数
        /// </summary>
        void SendEventOnce<T>(T eventValue);

        /// <summary>
        /// 注册绑定对应类型是事件回调函数, 会接受所有可能触发事件回调的参数<br/>
        /// 例如：<code>Send(UIModelBase)</code>也会触发绑定为<code>Action{IUIModel}</code>的函数(注: UIModelBase继承自IUIModel
        /// </summary>
        /// <returns>返回一个<see cref="IDisposable"/>接口，调用<see cref="IDisposable.Dispose"/>方法结束回调的监听</returns>
        IDisposable RegisterEvent<T>(Action<T> onEvent);
    }
}