namespace YuzeToolkit.Utility
{
    /// <summary>
    /// 在Unity的更新逻辑中的优先级
    /// </summary>
    public enum OrderType
    {
        /// <summary>
        /// 在所有unity更新逻辑之前（除了UnityEvent
        /// </summary>
        First,

        /// <summary>
        /// 在默认的更新逻辑之前
        /// </summary>
        Before,

        /// <summary>
        /// 在默认的更新逻辑之后
        /// </summary>
        After,

        /// <summary>
        /// 在所有unity更新逻辑之后
        /// </summary>
        End
    }
}