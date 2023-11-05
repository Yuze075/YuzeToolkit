using System;

namespace YuzeToolkit.BindableTool
{
    /// <summary>
    /// 所有修饰<see cref="BindableSystem.ModifyBindable"/>的底层接口, 可以通过<see cref="BindableSystem"/>对对应的属性值进行修饰<br/>
    /// 可以调用<see cref="IDisposable"/>可以解除对<see cref="IBindable"/>的修饰<br/><br/>
    /// </summary>
    public interface IModify : IDisposable
    {
        /// <summary>
        /// 具体需要修改的<see cref="IModifiable"/>的类型<see cref="Type"/><br/>
        /// 如果为null, 则没有修饰效果
        /// </summary>
        Type? TryModifyType { get; }
    }
}