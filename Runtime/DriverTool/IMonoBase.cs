namespace YuzeToolkit.DriverTool
{
    /// <summary>
    /// 所有需要<see cref="MonoDriverBase"/>统一更新函数的接口
    /// </summary>
    public interface IMonoBase
    {
        public static float S_DeltaTime;
        public static float S_FixedDeltaTime;

        /// <summary>
        /// 更新类型, 在不同Unity的更新顺序中更新
        /// </summary>
        OrderType Type { get; }

        /// <summary>
        /// 更新优先级, 越小越早更新
        /// </summary>
        int UpdatePriority  { get; }
    }

    /// <summary>
    /// 绑定基于<see cref="MonoDriverBase"/>驱动更新的<see cref="MonoDriverBase.Update"/>函数
    /// </summary>
    public interface IUpdate : IMonoBase
    {
        /// <summary>
        /// 在<see cref="MonoDriverBase.Update"/>更新的函数
        /// </summary>
        void OnUpdate();
    }

    /// <summary>
    /// 绑定基于<see cref="MonoDriverBase"/>驱动更新的<see cref="MonoDriverBase.FixedUpdate"/>函数
    /// </summary>
    public interface IFixedUpdate : IMonoBase
    {
        /// <summary>
        /// 在<see cref="MonoDriverBase.FixedUpdate"/>更新的函数
        /// </summary>
        void OnFixedUpdate();
    }

    /// <summary>
    /// 绑定基于<see cref="MonoDriverBase"/>驱动更新的<see cref="MonoDriverBase.LateUpdate"/>函数
    /// </summary>
    public interface ILateUpdate : IMonoBase
    {
        /// <summary>
        /// 在<see cref="MonoDriverBase.LateUpdate"/>更新的函数
        /// </summary>
        void OnLateUpdate();
    }
}