using YuzeToolkit.LogTool;

namespace YuzeToolkit.UITool
{
    /// <summary>
    /// 用于空返回值的<see cref="ICommand{TResult}"/>
    /// </summary>
    public struct Empty
    {
    }

    /// <summary>
    /// 命令接口的接口，可以封装各种命令，用于调用和处理逻辑（此命令带有返回值）<br/>
    /// 通常命令用于处理相关<see cref="IUIController"/>到<see cref="IUIModel"/>的各种操控逻辑
    /// </summary>
    public interface ICommand<out TResult> : ICanSendEvent, ICanSendCommand, ICanGetModel, ICanGetUtility
    {
        TResult Execute();
    }

    /// <inheritdoc/>
    public abstract class CommandBase : ICommand<Empty>
    {
        private IUICore? _core;
        IUICore IBelongUICore.Core => _core.IsNotNull(message: $"CommandType: {GetType()}");
        public void SetCore(IUICore core) => _core = core;

        Empty ICommand<Empty>.Execute()
        {
            Execute();
            return default;
        }

        protected abstract void Execute();
    }

    /// <inheritdoc/>
    public abstract class CommandBase<TResult> : ICommand<TResult>
    {
        private IUICore? _core;
        IUICore IBelongUICore.Core => _core.IsNotNull(message: $"CommandType: {GetType()}");
        public void SetCore(IUICore core) => _core = core;
        TResult ICommand<TResult>.Execute() => Execute();
        protected abstract TResult Execute();
    }
}