#if USE_UNITY_NETCODE
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using YuzeToolkit.BindableTool;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.Network.BindableTool
{
    /// <summary>
    /// <inheritdoc cref="YuzeToolkit.BindableTool.IState" />
    /// 并且通过<see cref="NetworkVariable{T}"/>进行网络变量的同步<br/>
    /// </summary>
    [Serializable]
    public class NetworkState : NetworkVariable<bool>, IState
    {
        protected NetworkState(bool valueBase,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) :
            base(valueBase, readPerm, writePerm)
        {
            this.valueBase = valueBase;
            OnValueChanged += (oldValue, newValue) => { _onValueChange?.Invoke(oldValue, newValue); };
        }

        private SLogTool? _sLogTool;
        private IModifiableOwner? _owner;

        private SLogTool LogTool => _sLogTool ??= new SLogTool(new[]
        {
            nameof(IState),
            GetType().FullName
        });

        void IBindable.SetLogParent(ILogTool value) => LogTool.Parent = value;

        IModifiableOwner IModifiable.Owner => LogTool.IsNotNull(_owner);

        void IModifiable.SetOwner(IModifiableOwner value)
        {
            if (_owner != null)
                LogTool.Log(
                    $"类型为{GetType()}的{nameof(IModifiable)}的{nameof(_owner)}从{_owner.GetType()}替换为{value.GetType()}");
            _owner = value;
        }

        [SerializeField] private bool valueBase;
        [SerializeField] private bool isReadOnly;

        public new bool Value
        {
            get => base.Value;
            private set
            {
                if (base.Value == value) return;
                base.Value = value;
            }
        }

        public bool IsReadOnly => isReadOnly;

        public bool this[int priority, bool removeSmall = true]
        {
            get
            {
                var retValue = valueBase;
                if (modifyStates.Count == 0) return retValue;

                if (!removeSmall && priority < modifyStates[0].Priority) return retValue;
                var modifyPriority = removeSmall ? priority : modifyStates[0].Priority;
                var orValue = false;
                var andValue = true;
                foreach (var modifyState in modifyStates)
                {
                    if (removeSmall && modifyState.Priority < modifyPriority) continue;

                    if (modifyState.Priority != modifyPriority)
                    {
                        if (!removeSmall && priority < modifyState.Priority) break;
                        retValue |= orValue;
                        retValue &= andValue;

                        modifyPriority = modifyState.Priority;
                        orValue = false;
                        andValue = true;
                    }

                    switch (modifyState)
                    {
                        case OrModifyState orModifyState:
                            orValue |= orModifyState.OrValue;
                            break;
                        case AndModifyState andModifyState:
                            andValue &= andModifyState.AndValue;
                            break;
                    }
                }

                retValue |= orValue;
                retValue &= andValue;
                return retValue;
            }
        }

        #region Modify

        [SerializeReference] private List<ModifyState> modifyStates = new();

        IDisposable IModifiable.Modify(IModify modify, IModifyReason reason)
        {
            if (!this.IsSameOwnerReason(reason, LogTool)) return modify;

            if (!this.TryCastModify(modify, LogTool, out ModifyState modifyIn)) return modify;

            if (!this.TryCheckModify(modifyIn, reason, out var modifyOut))
                return modifyIn;

            return Modify(modifyOut);
        }

        IDisposable IState.Modify(ModifyState modifyState, IModifyReason reason)
        {
            if (!this.IsSameOwnerReason(reason, LogTool)) return modifyState;

            if (!this.TryCheckModify(modifyState, reason, out var modifyOut))
                return modifyState;

            return Modify(modifyOut);
        }

        private IDisposable Modify(ModifyState modifyState)
        {
            if (!this.TryCheckModifyType(modifyState, LogTool)) return modifyState;

            var index = modifyStates.BinarySearch(modifyState, ModifyStateComparer.Comparer);
            modifyStates.Insert(index >= 0 ? index : ~index, modifyState);


            modifyState.Init(new UnRegister(() =>
            {
                if (modifyStates.Remove(modifyState))
                    ReCheckValue();
            }), this);
            ReCheckValue();
            _disposeGroup.Add(modifyState);
            return modifyState;
        }

        public void ReCheckValue()
        {
            var retValue = valueBase;
            if (modifyStates.Count == 0)
            {
                Value = retValue;
                return;
            }

            var priority = modifyStates[0].Priority;
            var orValue = false;
            var andValue = true;

            foreach (var modify in modifyStates)
            {
                if (modify.Priority != priority)
                {
                    retValue |= orValue;
                    retValue &= andValue;

                    priority = modify.Priority;
                    orValue = false;
                    andValue = true;
                }

                switch (modify)
                {
                    case OrModifyState orModify:
                        orValue |= orModify.OrValue;
                        break;
                    case AndModifyState andModify:
                        andValue &= andModify.AndValue;
                        break;
                }
            }

            retValue |= orValue;
            retValue &= andValue;
            Value = retValue;
        }

        #endregion

        #region RegisterChange

        private ValueChange<bool>? _onValueChange;

        public IDisposable RegisterChange(ValueChange<bool> onValueChange)
        {
            _onValueChange += onValueChange;
            return _disposeGroup.UnRegister(() => { _onValueChange -= onValueChange; });
        }

        public IDisposable RegisterChangeBuff(ValueChange<bool> onValueChange)
        {
            onValueChange(!Value, Value);
            return RegisterChange(onValueChange);
        }

        #endregion

        #region IDisposable

        private DisposeGroup _disposeGroup;

        void IDisposable.Dispose()
        {
            Value = valueBase;
            _disposeGroup.Dispose();
        }

        #endregion

        public static implicit operator bool(NetworkState networkState) => networkState.Value;
    }
}
#endif