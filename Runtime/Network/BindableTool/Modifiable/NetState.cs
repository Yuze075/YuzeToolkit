#nullable enable
#if YUZE_USE_UNITY_NETCODE
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace YuzeToolkit.BindableTool.Network
{
    /// <inheritdoc cref="YuzeToolkit.BindableTool.IState{TModifyState}" />
    [Serializable]
    public abstract class NetState : NetworkVariable<bool>, IState<IModifyState>
    {
        protected NetState(
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) :
            base(default, readPerm, writePerm)
        {
            OnValueChanged = InvokeValueChanged;
        }

        protected NetState(bool valueBase, IModifiableOwner? modifiableOwner = null,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) :
            base(valueBase, readPerm, writePerm)
        {
            this.valueBase = valueBase;
            OnValueChanged = InvokeValueChanged;
        }

        /// <summary>
        /// 初始化基于Unity机制序列化的<see cref="NetState"/>
        /// </summary>
        public void SetOnly(bool valueBase, IModifiableOwner? modifiableOwner = null)
        {
            OnValueChanged = null;
            this.valueBase = valueBase;
            base.Value = valueBase;
            ModifiableOwner = modifiableOwner;
            OnValueChanged = InvokeValueChanged;
        }

        [SerializeField] private bool valueBase;
        private ValueChange<bool>? _valueChange;
        public IModifiableOwner? ModifiableOwner { get; private set; }

        public new bool Value
        {
            get => base.Value;
            private set => base.Value = value;
        }

        public bool this[int priority, bool removeSmall = true]
        {
            get
            {
                var retValue = valueBase;
                if (ModifyStates.Count == 0) return retValue;

                if (!removeSmall && priority < ModifyStates[0].Priority) return retValue;
                var modifyPriority = removeSmall ? priority : ModifyStates[0].Priority;
                var orValue = false;
                var andValue = true;
                foreach (var modifyState in ModifyStates)
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

                    switch (modifyState.ModifyStateType)
                    {
                        case EModifyStateType.Or:
                            orValue |= modifyState.ModifyValue;
                            break;
                        case EModifyStateType.And:
                            andValue &= modifyState.ModifyValue;
                            break;
                    }
                }

                retValue |= orValue;
                retValue &= andValue;
                return retValue;
            }
        }

        #region Modify

#if YUZE_USE_EDITOR_TOOLBOX
        [ReorderableList(fixedSize: true, draggable: false)]
#endif
        [SerializeReference]
        private List<IModifyState>? _modifyStates;

        private List<IModifyState> ModifyStates => _modifyStates ??= new List<IModifyState>();

        public bool Modify(IModifyState modify, object? sender, object? reason = null)
        {
            if (!this.CheckModify(modify, out var modifyState, sender, reason)) return false;

            var index = ModifyStates.BinarySearch(modifyState, modifyState);
            ModifyStates.Insert(index >= 0 ? index : ~index, modifyState);
            ReCheckValue();
            return true;
        }

        public void RemoveModify(IModifyState modifyState)
        {
            if (ModifyStates.Remove(modifyState)) ReCheckValue();
        }

        public void ReCheckValue()
        {
            var retValue = valueBase;
            if (ModifyStates.Count == 0)
            {
                Value = retValue;
                return;
            }

            var priority = ModifyStates[0].Priority;
            var orValue = false;
            var andValue = true;

            foreach (var modifyState in ModifyStates)
            {
                if (modifyState.Priority != priority)
                {
                    retValue |= orValue;
                    retValue &= andValue;

                    priority = modifyState.Priority;
                    orValue = false;
                    andValue = true;
                }

                switch (modifyState.ModifyStateType)
                {
                    case EModifyStateType.Or:
                        orValue |= modifyState.ModifyValue;
                        break;
                    case EModifyStateType.And:
                        andValue &= modifyState.ModifyValue;
                        break;
                }
            }

            retValue |= orValue;
            retValue &= andValue;
            Value = retValue;
        }

        #endregion

        private void InvokeValueChanged(bool value, bool newValue) => _valueChange?.Invoke(value, newValue);

        public void AddValueChange(ValueChange<bool>? valueChange)
        {
            if (valueChange != null) _valueChange += valueChange;
        }

        public void RemoveValueChange(ValueChange<bool>? valueChange)
        {
            if (valueChange != null) _valueChange -= valueChange;
        }

        public void Reset()
        {
            _valueChange = null;
            ModifiableOwner = null;
            valueBase = default;
            base.Value = default;
            _modifyStates?.Clear();
        }

        public static implicit operator bool(NetState netState) => netState.Value;
    }
}
#endif