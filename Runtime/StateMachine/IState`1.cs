using YuzeToolkit.Log;
namespace YuzeToolkit.StateMachine
{
    public interface IState<T> : IState where T : IStateMachine
    {
        IStateMachine IState.StateMachine
        {
            get => StateMachine;
            set
            {
                if (value is T stateMachine)
                {
                    StateMachine = stateMachine;
                    return;
                }

                if (value != null)
                {
                    LogSys.Warning($"{value.GetType()}: 不是对于的目标类型{typeof(T)}");
                }

                StateMachine = default;
            }
        }

        void IState.Enter(IState beforeState)
        {
            if (beforeState is IState<T> state)
            {
                OnEnter(state);
                return;
            }

            if (beforeState != null)
            {
                LogSys.Warning($"{beforeState.GetType()}: 不是对于的目标类型{typeof(IState<T>)}");
            }

            OnEnter(null);
        }

        void IState.Update(float tickTime)
        {
            OnUpdate(tickTime);
            CheckChange();
        }

        void IState.Exit(IState afterState)
        {
            if (afterState is IState<T> state)
            {
                OnExit(state);
                return;
            }

            if (afterState != null)
            {
                LogSys.Warning($"{afterState.GetType()}: 不是对于的目标类型{typeof(IState<T>)}");
            }

            OnExit(null);
        }

        public new T StateMachine { get; protected internal set; }
        void OnEnter(IState<T> beforeState);
        void OnUpdate(float tickTime);
        void CheckChange();
        void OnExit(IState<T> afterState);
    }
}