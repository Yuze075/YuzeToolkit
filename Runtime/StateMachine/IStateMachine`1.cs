﻿namespace YuzeToolkit.Utility
{
    public interface IStateMachine<TState> : IStateMachine where TState : IState
    {
        IState IStateMachine.CurrentState
        {
            get => CurrentState;
            set
            {
                if (value is TState state)
                {
                    CurrentState = state;
                    return;
                }

                if (value != null)
                {
                    LogSystem.Warning($"{value?.GetType()}: 不是对于的目标类型{typeof(TState)}");
                }

                CurrentState = default;
            }
        }
        public new TState CurrentState { get;  protected internal set; }
    }
}