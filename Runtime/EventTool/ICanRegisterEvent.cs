using System;

namespace YuzeToolkit.EventTool
{
    public interface ICanRegisterEvent
    {
        /// <summary>
        /// 注册绑定对应类型是事件回调函数, 会接受所有可能触发事件回调的参数<br/>
        /// 例如：<code>Send(UIModelBase)</code>也会触发绑定为<code>Action{IUIModel}</code>的函数(注: UIModelBase继承自IUIModel
        /// </summary>
        /// <returns>返回一个<see cref="IDisposable"/>接口，调用<see cref="IDisposable.Dispose"/>方法结束回调的监听</returns>
        IDisposable RegisterEvent<T>(Action<T> onEvent);
    }
}