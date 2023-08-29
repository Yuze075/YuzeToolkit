namespace YuzeToolkit.Utility
{
    /// <summary>
    /// 被包装节点<see cref="MonoBaseWrapperList{T}"/>的类型, 用于判断是否更新
    /// </summary>
    internal enum WrapperType
    {
        /// <summary>
        /// 节点存在, 且处于激活状态, 更新对应的Update函数
        /// </summary>
        Enable,

        /// <summary>
        /// 节点存在, 且处于失活状态, 不更新对应的Update函数
        /// </summary>
        Disable,

        /// <summary>
        /// 节点为空, 可以用于添加新的<see cref="IMonoBase"/>
        /// </summary>
        Null
    }
}