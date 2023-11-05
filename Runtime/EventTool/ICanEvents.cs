namespace YuzeToolkit.EventTool
{
    /// <summary>
    /// 能注册和发送事件的基类接口
    /// </summary>
    public interface ICanEvents : ICanRegisterEvent, ICanSendEvent
    {
    }
}