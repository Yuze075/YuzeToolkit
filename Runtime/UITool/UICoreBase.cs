#nullable enable
using System.Diagnostics.CodeAnalysis;
using YuzeToolkit.EventTool;
using YuzeToolkit.IoCTool;

namespace YuzeToolkit.UITool
{
    /// <summary>
    /// 框架的核心底层接口，用于将各个组件统一组合在一起<br/>
    /// 可以注册不同的组件，例如: <see cref="IUIModelBase"/> 等，也可以获取对应的组件（通过<see cref="Rule"/>下的拓展方法）<br/>
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
    public interface IUICore : IEventNode
    {
        TModel? GetModel<TModel>() where TModel : IUIModelBase;
        TUtility? GetUtility<TUtility>() where TUtility : IUIUtility;

        bool TryGetModel<TModel>([MaybeNullWhen(false), NotNullWhen(true)] out TModel model)
            where TModel : IUIModelBase;

        bool TryGetUtility<TUtility>([MaybeNullWhen(false), NotNullWhen(true)] out TUtility utility)
            where TUtility : IUIUtility;

        // ReSharper disable Unity.PerformanceAnalysis
        void SendCommand(ICommand<Empty> command);

        // ReSharper disable Unity.PerformanceAnalysis
        TResult SendCommand<TResult>(ICommand<TResult> command);
    }

    /// <inheritdoc cref="IUICore" />
    public abstract class UICoreBase : Container, IUICore
    {
        public override void Inject(object tryInjectValue)
        {
            base.Inject(tryInjectValue);
            if (tryInjectValue is not IBelongUICore belongUICore) return;
            belongUICore.Core = this;
        }

        EventActionDictionary IEventNode.EventActions { get; } = new();
        public TModel? GetModel<TModel>() where TModel : IUIModelBase => Get<TModel>();
        public TUtility? GetUtility<TUtility>() where TUtility : IUIUtility => Get<TUtility>();

        public bool TryGetModel<TModel>([MaybeNullWhen(false), NotNullWhen(true)] out TModel model)
            where TModel : IUIModelBase => TryGet(out model);

        public bool TryGetUtility<TUtility>([MaybeNullWhen(false), NotNullWhen(true)] out TUtility utility)
            where TUtility : IUIUtility => TryGet(out utility);

        #region ICommand

        public void SendCommand(ICommand<Empty> command)
        {
            Inject(command);
            ExecuteCommand(command);
        }

        public TResult SendCommand<TResult>(ICommand<TResult> command)
        {
            Inject(command);
            return ExecuteCommand(command);
        }

        /// <summary>
        /// 可以重新这个方法用于拦截不同的<see cref="ICommand{T}"/>
        /// </summary>
        protected virtual TResult ExecuteCommand<TResult>(ICommand<TResult> command) => command.Execute();

        #endregion

        protected override void OnDestroy()
        {
            this.ClearAllEvents();
            base.OnDestroy();
        }
    }
}