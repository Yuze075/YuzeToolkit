using System;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.BindableTool
{
    /// <summary>
    /// 所有可绑定变量的底层接口<br/><br/>
    /// </summary>
    public interface IBindable : IDisposable
    {
        /// <summary>
        /// 最终的变量值
        /// </summary>
        object? Value { get; }

        /// <summary>
        /// 注册数值改变的回调, 获取到旧数值和新数值
        /// </summary>
        IDisposable RegisterChange(ValueChange<object> valueChange);

        /// <summary>
        /// 注册数值改变的回调, 获取到旧数值和新数值, 并且注册时就获得缓存的数值
        /// </summary>
        IDisposable RegisterChangeBuff(ValueChange<object> valueChange);

        /// <summary>
        /// 设置<see cref="ILogTool"/>的Parent, 用于打印追踪
        /// </summary>
        void SetLogParent(ILogTool parent);
    }
}