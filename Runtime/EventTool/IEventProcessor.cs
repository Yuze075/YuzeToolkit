#nullable enable
namespace YuzeToolkit.EventTool
{
    public interface IEventProcessor
    {
        /// <summary>
        /// 接受各种事件
        /// </summary>
        void HandleEvent<TEvent>(TEvent eventValue);
    }
    
    public interface IBeforeEventProcessor : IEventProcessor
    {
        /// <summary>
        /// 用于拦截事件，如果返回值未null则不会触发事件
        /// </summary>
        /// <param name="eventValue">事件泛型值, 可以通过类型转换知道具体为什么类型的值(不为空)</param>
        /// <returns>返回true则继续执行事件，否则不执行</returns>
        bool BeforeHandlerEvent<TEvent>(ref TEvent eventValue);
    }
}