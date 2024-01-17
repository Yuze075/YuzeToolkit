#nullable enable
namespace YuzeToolkit.BindableTool
{
    /// <summary>
    /// <inheritdoc cref="IModifiable" />
    /// <br/><br/>
    /// </summary>
    public interface IProperty : IModifiable
    {
        /// <summary>
        /// 当超出范围时的处理逻辑
        /// </summary>
        EOutOfRangeType OutOfRangeType { get; }
        
        /// <summary>
        /// 移除绑定的<see cref="ModifyProperty"/>
        /// </summary>
        void RemoveModify(ModifyProperty modifyProperty);
        
        /// <summary>
        /// 重新检测对应的属性修正
        /// </summary>
        void ReCheckValue();
    }
    
    /// <summary>
    /// <inheritdoc cref="IProperty" />
    /// 属性接口, 通过一个数值类型进行属性数值的管理<br/>
    /// 可以通过<see cref="MultModifyProperty"/>和<see cref="IProperty{TValue}"/>对<see cref="AddModifyProperty"/>的基础值进行修正<br/>
    /// 优先级越高越后修正, 在同一优先级先进行所有的<see cref="MultModifyProperty"/>修正, 再进行所有的<see cref="IModifiable"/>修正<br/><br/>
    /// </summary>
    public interface IProperty<out TValue> : IProperty, IBindable<TValue>
    {
        /// <summary>
        /// 数值最小值
        /// </summary>
        TValue Min { get; }

        /// <summary>
        /// 数值最大值
        /// </summary>
        TValue Max { get; }
    }
    
    public enum EOutOfRangeType
    {
        /// <summary>
        /// 不做任何处理, 继续进行计算直到结束
        /// </summary>
        None,

        /// <summary>
        /// 当超出范围时, 直接停止返回超出范围的值
        /// </summary>
        Stop,

        /// <summary>
        /// 当超出范围时, 将当前值调整至范围内, 并继续进行计算
        /// </summary>
        Clamp
    }
}