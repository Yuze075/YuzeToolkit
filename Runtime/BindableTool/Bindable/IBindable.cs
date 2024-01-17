#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.BindableTool
{
    /// <summary>
    /// 所有可绑定变量的底层接口<br/><br/>
    /// 继承自<see cref="IDisposable"/>, 用于释放绑定的回调函数, 和对<see cref="Value"/>值的修改<br/><br/>
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
        [return: NotNullIfNotNull("valueChange")]
        IDisposable? RegisterChange(ValueChange<object>? valueChange);

        /// <summary>
        /// 注册数值改变的回调, 获取到旧数值和新数值, 并且注册时就获得缓存的数值
        /// </summary>
        [return: NotNullIfNotNull("valueChange")]
        IDisposable? RegisterChangeBuff(ValueChange<object>? valueChange);
    }
}