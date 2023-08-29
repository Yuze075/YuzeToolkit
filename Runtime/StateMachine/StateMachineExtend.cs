using System;
using System.Collections.Generic;


namespace YuzeToolkit.Utility
{
    public static class StateMachineExtend
    {
        public static void AddState(this IStateMachine stateMachine, IState addState)
        {
            stateMachine.States ??= new Dictionary<Type, IState>();

            if (stateMachine.States.TryAdd(addState.GetType(), addState))
            {
                addState.StateMachine = stateMachine;
            }
            else
            {
                LogSystem.Warning($"{addState.GetType()}: 已经存在在状态机中了");
            }
        }

        public static void ChangeState<TChangeState>(this IStateMachine stateMachine)
        {
            if (stateMachine.States == null)
            {
                LogSystem.Warning($"{stateMachine.GetType()}: States字典为空");
                return;
            }

            var changeStateType = typeof(TChangeState);
            if (!stateMachine.States.TryGetValue(changeStateType, out var afterState))
            {
                LogSystem.Warning($"{changeStateType}: 无法找到对应类型的对象");
                return;
            }

            var beforeState = stateMachine.CurrentState;
            beforeState.Exit(afterState);
            afterState.Enter(beforeState);
            stateMachine.CurrentState = afterState;
        }

        public static void Run<TRunState>(this IStateMachine stateMachine)
        {
            if (stateMachine.IsRunning) return;
            
            if (stateMachine.States == null)
            {
                LogSystem.Warning($"{stateMachine.GetType()}: States字典为空");
                return;
            }

            if (!stateMachine.States.TryGetValue(typeof(TRunState), out var iState))
            {
                LogSystem.Warning($"{typeof(TRunState)}: 无法找到对应类型的对象");
                return;
            }

            stateMachine.CurrentState = iState;
            stateMachine.CurrentState.Enter(null);
            stateMachine.IsRunning = true;
        }

        public static void Stop(this IStateMachine stateMachine)
        {
            if (!stateMachine.IsRunning) return;
            
            stateMachine.CurrentState.Exit(null);
            stateMachine.CurrentState = null;
            stateMachine.IsRunning = false;
        }
    }
}