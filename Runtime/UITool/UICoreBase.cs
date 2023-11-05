using System;
using YuzeToolkit.EventTool;
using YuzeToolkit.IoCTool;

namespace YuzeToolkit.UITool
{
    /// <summary>
    /// 框架的核心底层接口，用于将各个组件统一组合在一起<br/>
    /// 可以注册不同的组件，例如: <see cref="IModel"/> 等，也可以获取对应的组件（通过<see cref="Rule"/>下的拓展方法）<br/>
    /// 框架核心逻辑为泛型单例，继承泛型实现之后，每个泛型单独用于一个自己的<see cref="IUICore"/>，所有应该给每个面板单独继承一个<see cref="IUICore"/><br/>
    /// <code>
    /// 框架：
    /// Controller控制(MonoBehaviour) ←（代码中双向绑定）→ View视觉部分(UGUI/xxUI)
    ///     ↓（依赖
    /// Architecture
    ///     -Model数据: 存储UI显示数据
    ///     -Command命令: 用于View面板的操作响应到Model的操作
    ///     -Event事件: Model数据的更新, 更新到View
    ///     -Utility组件: 用于Command访问外部逻辑
    /// 流程(中间都通过Architecture中继)：
    /// Controller --Command-->Model （从操控到数据）
    /// Model --Event--> Controller(View) （从数据到显示）
    /// Controller --Command--> Utility （访问外部逻辑）
    /// </code>
    /// </summary>
    public interface IUICore : ICanEvents, IDisposable
    {
        internal TModel? GetModel<TModel>() where TModel : IModel;
        internal TUtility? GetUtility<TUtility>() where TUtility : IUtility;

        internal TModel GetNotNullModel<TModel>(string? name = null, string? message = null,
            bool additionalCheck = true) where TModel : IModel;

        internal TUtility GetNotNullUtility<TUtility>(string? name = null, string? message = null,
            bool additionalCheck = true) where TUtility : IUtility;

        internal bool TryGetModel<TModel>(out TModel model) where TModel : IModel;
        internal bool TryGetUtility<TUtility>(out TUtility utility) where TUtility : IUtility;

        // ReSharper disable Unity.PerformanceAnalysis
        internal void SendCommand(ICommand<Empty> command);

        // ReSharper disable Unity.PerformanceAnalysis
        internal TResult SendCommand<TResult>(ICommand<TResult> command);
    }

    /// <inheritdoc cref="IUICore" />
    public abstract class UICoreBase : Container, IUICore
    {
        public override void Inject(object tryInjectValue)
        {
            base.Inject(tryInjectValue);
            if (tryInjectValue is not IBelongUICore belongUICore) return;
            belongUICore.SetCore(this);
        }

        TModel? IUICore.GetModel<TModel>() where TModel : default => Get<TModel>();
        TUtility? IUICore.GetUtility<TUtility>() where TUtility : default => Get<TUtility>();

        TModel IUICore.GetNotNullModel<TModel>(string? name, string? message, bool additionalCheck)
            where TModel : default => IsNotNull(Get<TModel>(), name, message, additionalCheck);

        TUtility IUICore.GetNotNullUtility<TUtility>(string? name, string? message, bool additionalCheck)
            where TUtility : default => IsNotNull(Get<TUtility>(), name, message, additionalCheck);

        bool IUICore.TryGetModel<TModel>(out TModel model) => TryGet(out model);
        bool IUICore.TryGetUtility<TUtility>(out TUtility utility) => TryGet(out utility);

        #region ICommand

        void IUICore.SendCommand(ICommand<Empty> command)
        {
            Inject(command);
            ExecuteCommand(command);
        }

        TResult IUICore.SendCommand<TResult>(ICommand<TResult> command)
        {
            Inject(command);
            return ExecuteCommand(command);
        }

        /// <summary>
        /// 可以重新这个方法用于拦截不同的<see cref="ICommand{T}"/>
        /// </summary>
        protected virtual TResult ExecuteCommand<TResult>(ICommand<TResult> command) => command.Execute();

        #endregion

        #region Event

        private EventSystem? _events;
        private EventSystem EventSystem => _events ?? new EventSystem(this);
        public void SendEvent<TBase, T>() where T : TBase, new() => DoSendEvent<TBase>(new T());
        public void SendEvent<T>() where T : new() => DoSendEvent(new T());
        public void SendEvent<T>(T eventValue) => DoSendEvent(eventValue);
        public void SendEventOnce<TBase, T>() where T : TBase, new() => DoSendEventOnce<TBase>(new T());
        public void SendEventOnce<T>() where T : new() => DoSendEventOnce(new T());
        public void SendEventOnce<T>(T eventValue) => DoSendEventOnce(eventValue);
        public IDisposable RegisterEvent<T>(Action<T> onEvent) => EventSystem.RegisterEvent(onEvent);
        public IDisposable RegisterEvent(ICanEvents events) => EventSystem.RegisterEvent(events);

        /// <summary>
        /// 可以拦截对应的Event事件
        /// </summary>
        protected virtual void DoSendEvent<TEvent>(TEvent eventValue) => EventSystem.SendEvent(eventValue);

        /// <summary>
        /// 可以拦截对应的Event事件
        /// </summary>
        protected virtual void DoSendEventOnce<TEvent>(TEvent eventValue) => EventSystem.SendEventOnce(eventValue);

        #endregion
    }
}