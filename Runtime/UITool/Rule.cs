#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using YuzeToolkit.EventTool;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.UITool
{
    /// <summary>
    /// MVC中的<see cref="IUIModel"/>层，用于存储数据，将逻辑和数据分离<br/>
    /// <see cref="ICommand{TResult}"/>会<see cref="IUIModel"/>进行操作；
    /// <see cref="IUIModel"/>通过Event想<see cref="IUIController"/>传递信息
    /// </summary>
    public interface IUIModel : ICanSendEvent, ICanRegisterEvent, ICanGetNotNullModel, ICanGetNotNullUtility
    {
    }

    /// <summary>
    /// 控制器的接口，产生<see cref="ICommand{TResult}"/>对<see cref="IUIModel"/>中数据操作;
    /// 同时接受<see cref="IUIModel"/>发送的Event, 进行显示的更新<br/>
    /// 通常View（UGUI）和<see cref="IUIController"/>会处于统一层级，不做过多的区分。
    /// </summary>
    public interface IUIController : ICanSendCommand, ICanRegisterEvent, ICanGetModel
    {
    }

    /// <summary>
    /// 外部逻辑处理层，用于对接MVC的外部逻辑<br/>
    /// 会默认进行Inject的初始化注入
    /// </summary>
    public interface IUIUtility
    {
    }

    /// <summary>
    /// 是否归属于<see cref="UICoreBase"/>, 用于拓展方法的实现<br/>
    /// 通常<see cref="IUIModel"/>,<see cref="IUIController"/>,<see cref="ICommand{TResult}"/>都继承自这个接口<br/>
    /// 但<see cref="IUIUtility"/>不继承自这个接口, 作为外部访问接口不直接属于<see cref="IUICore"/><br/>
    /// (虽然大部分情况下都属于<see cref="IUICore"/>, 但<see cref="IUIUtility"/>不需要使用拓展方法就没有继承)
    /// </summary>
    public interface IBelongUICore
    {
        /// <summary>
        /// 绑定的对应的<see cref="IUICore"/>核心, 默认都应该为显式接口实现<br/>
        /// 默认不应该直接调用Get和Set方法(Get方法应该由Rule拓展方法调用, Set方法应该由依赖注入直接进行绑定)
        /// </summary>
        IUICore Core { get; set; }
    }

    public interface ICanSendCommand : IBelongUICore
    {
    }

    public interface ICanRegisterEvent : IBelongUICore
    {
    }

    public interface ICanSendEvent : IBelongUICore
    {
    }

    public interface ICanGetModel : IBelongUICore
    {
    }
    
    public interface ICanGetNotNullModel : ICanGetModel
    {
    }

    public interface ICanGetUtility : IBelongUICore
    {
    }
    
    public interface ICanGetNotNullUtility : ICanGetUtility
    {
    }

    public static class Rule
    {
        public static void SendCommand<TCommand>(this ICanSendCommand self) where TCommand : ICommand<Empty>, new() =>
            self.Core.SendCommand(new TCommand());

        public static TResult SendCommand<TCommand, TResult>(this ICanSendCommand self)
            where TCommand : ICommand<TResult>, new() => self.Core.SendCommand(new TCommand());

        public static TResult SendCommand<TResult>(this ICanSendCommand self, ICommand<TResult> command) =>
            self.Core.SendCommand(command);

        public static void SendCommand(this ICanSendCommand self, ICommand<Empty> command) =>
            self.Core.SendCommand(command);

        public static void SendEventAll<TBase, T>(this ICanSendEvent self) where T : TBase, new() =>
            self.Core.SendEventAll<TBase, T>();

        public static void SendEventAll<T>(this ICanSendEvent self) where T : new() =>
            self.Core.SendEventAll<T>();

        public static void SendEventAll<T>(this ICanSendEvent self, T eventValue) =>
            self.Core.SendEventAll(eventValue);

        public static void SendEvent<TBase, T>(this ICanSendEvent self) where T : TBase, new() =>
            self.Core.SendEvent<TBase, T>();

        public static void SendEvent<T>(this ICanSendEvent self) where T : new() =>
            self.Core.SendEvent<T>();

        public static void SendEvent<T>(this ICanSendEvent self, T eventValue) =>
            self.Core.SendEvent(eventValue);

        [return: NotNullIfNotNull("onEvent")]
        public static IDisposable? RegisterEvent<T>(this ICanRegisterEvent self, Action<T>? onEvent) =>
            self.Core.RegisterEvent(onEvent);

        public static TModel? GetModel<TModel>(this ICanGetModel self) where TModel : IUIModel =>
            self.Core.GetModel<TModel>();

        public static TUtility? GetUtility<TUtility>(this ICanGetUtility self) where TUtility : IUIUtility =>
            self.Core.GetUtility<TUtility>();
    }
}