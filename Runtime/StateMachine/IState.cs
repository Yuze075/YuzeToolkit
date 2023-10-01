namespace YuzeToolkit.StateMachine
{
    public interface IState
    {
        /// <summary>
        /// 基础的状态机
        /// </summary>
        IStateMachine StateMachine { get; protected internal set; }
        
        /// <summary>
        /// 进入状态调用的函数
        /// </summary>
        /// <param name="beforeState">前一个状态</param>
        void Enter(IState beforeState);

        /// <summary>
        /// 状态更新函数
        /// </summary>
        void Update(float tickTime);

        /// <summary>
        /// 退出状态调用的函数
        /// </summary>
        /// <param name="afterState">后一个状态</param>
        void Exit(IState afterState);
    }
}