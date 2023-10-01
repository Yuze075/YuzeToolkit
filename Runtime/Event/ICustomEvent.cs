using System;

namespace YuzeToolkit.Event
{
    /// <summary>
    /// 自定义的事件类型转化，可以让事件额外绑定其他的数据类型<br/>
    /// 直接在<see cref="DoCustomEvent"/>中触发对应事件即可
    /// </summary>
    public interface ICustomEvent
    {
        void DoCustomEvent(ICanEvents canEvents);
    }
}