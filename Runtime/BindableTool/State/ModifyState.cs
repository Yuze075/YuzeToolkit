using System;
using UnityEngine;
using YuzeToolkit.LogTool;
using YuzeToolkit.SerializeTool;

namespace YuzeToolkit.BindableTool
{
    /// <summary>
    /// <inheritdoc/>
    /// 对<see cref="IState"/>的状态值<see cref="IState.Value"/>的修饰<br/>
    /// 优先级越高越后修正, 在同一优先级先进行所有的<see cref="OrModifyState"/>修正, 再进行所有的<see cref="AndModifyState"/>修正<br/><br/>
    /// </summary>
    [Serializable]
    public abstract class ModifyState : IModify
    {
        protected ModifyState(Type tryModifyType) => this.tryModifyType = tryModifyType;
        protected ModifyState(Type tryModifyType, int priority) : this(tryModifyType) => this.priority = priority;

        [SerializeField] private int priority;
        [SerializeField] private SerializeType tryModifyType;
        public Type? TryModifyType => tryModifyType;

        /// <summary>
        /// 修改<see cref="IState"/>的优先级, 优先级越高, 越后修正, 升序排列<br/>
        /// 同一优先级先进行所有的<see cref="OrModifyState"/>修正, 再进行所有的<see cref="AndModifyState"/>修正
        /// </summary>
        public int Priority => priority;

        private IDisposable? _disposable;
        private IState? _state;
        private bool _isInit;

        protected void ReCheckValue() => _state?.ReCheckValue();

        public void Init(IDisposable disposable, IState state)
        {
            if (_isInit)
            {
                LogSys.Log($"重复初始化修正变量{GetType()}, 当前修正State: {_state?.GetType()}, 新增修正State: {state.GetType()}");
                return;
            }

            _isInit = true;
            _disposable = disposable;
            _state = state;
        }

        public void Dispose()
        {
            if (!_isInit) return;
            _isInit = false;
            _disposable?.Dispose();
            _disposable = null;
            _state = null;
        }
    }
}