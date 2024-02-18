#nullable enable
using YuzeToolkit.LogTool;
using UnityObject = UnityEngine.Object;

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
    /// 通常命令用于处理相关<see cref="IUIController"/>到<see cref="IUIModelBase"/>的各种操控逻辑
    /// </summary>
    public interface ICommand<out TResult> : ICanSendEvent, ICanSendCommand, ICanGetNotNullModel, ICanGetNotNullUtility
    {
        TResult Execute();
    }

    /// <inheritdoc cref="YuzeToolkit.UITool.ICommand{TResult}" />
    public abstract class CommandBase : IObjectLogging, ICommand<Empty>
    {
        private IUICore? _core;
        string[]? IObjectLogging.Tags { get; set; }
        UnityObject? IObjectLogging.Context { get; set; }

        IUICore IBelongUICore.Core
        {
            get
            {
                LogSys.IsNotNull(_core != null, nameof(_core));
                return _core;
            }
            set => this.SetLogging(_core = value);
        }

        Empty ICommand<Empty>.Execute()
        {
            Execute();
            return default;
        }

        public abstract void Execute();
    }

    /// <inheritdoc cref="YuzeToolkit.UITool.ICommand{TResult}" />
    public abstract class CommandBase<TResult> : IObjectLogging, ICommand<TResult>
    {
        private IUICore? _core;
        string[]? IObjectLogging.Tags { get; set; }
        UnityObject? IObjectLogging.Context { get; set; }

        IUICore IBelongUICore.Core
        {
            get
            {
                LogSys.IsNotNull(_core != null, nameof(_core));
                return _core;
            }
            set => this.SetLogging(_core = value);
        }

        public abstract TResult Execute();
    }
}