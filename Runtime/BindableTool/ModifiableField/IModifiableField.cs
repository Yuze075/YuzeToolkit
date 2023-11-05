using System;

namespace YuzeToolkit.BindableTool
{
    /// <summary>
    /// <inheritdoc cref="IBindable{TValue}" />
    /// 用于可读字段, 特点方法可写字段<br/><br/>
    /// </summary>
    /// <typeparam name="TValue">数据类型</typeparam>
    public interface IModifiableField<TValue> : IBindable<TValue>, IModifiable
    {
        IDisposable Modify(ModifyField<TValue> modifyField, IModifyReason reason);
    }
}