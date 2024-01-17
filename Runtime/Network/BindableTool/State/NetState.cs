#nullable enable
#if YUZE_TOOLKIT_USE_UNITY_NETCODE
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.BindableTool.Network
{
    /// <summary>
    /// <inheritdoc cref="YuzeToolkit.BindableTool.IState" />
    /// 并且通过<see cref="NetworkVariable{T}"/>进行网络变量的同步<br/>
    /// </summary>
    [Serializable]
    public abstract class NetState : NetModifiableBase<bool, ModifyState>, IState
    {
        protected NetState(bool valueBase = default, bool isReadOnly = true,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server,
            IModifiableOwner? modifiableOwner = null, ILogging? loggingParent = null) :
            base(valueBase, isReadOnly, readPerm, writePerm, modifiableOwner, loggingParent)
        {
            this.valueBase = valueBase;
        }

        ~NetState() => Dispose(false);
        [NonSerialized] private bool _disposed;
        [NonSerialized] private bool _isDisposing;

        [SerializeField] private bool valueBase;

        public override bool Value
        {
            get
            {
                if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
                return BaseValue;
            }
            protected set
            {
                if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
                if (BaseValue == value) return;
                BaseValue = value;
            }
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

        [ReorderableList(fixedSize: true, draggable: false, HasLabels = false)] [SerializeReference]
        private List<ModifyState>? modifyStates;

        private List<ModifyState> ModifyStates => modifyStates ??= new List<ModifyState>();

        protected sealed override void Modify(ModifyState modifyState)
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            var index = ModifyStates.BinarySearch(modifyState, modifyState);
            ModifyStates.Insert(index >= 0 ? index : ~index, modifyState);
            ReCheckValue();
        }

        void IState.RemoveModify(ModifyState modifyState)
        {
            if (_disposed || _isDisposing) return;
            if (ModifyStates.Remove(modifyState)) ReCheckValue();
        }

        public void ReCheckValue()
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            var retValue = valueBase;
            if (ModifyStates.Count == 0)
            {
                Value = retValue;
                return;
            }

            var priority = ModifyStates[0].Priority;
            var orValue = false;
            var andValue = true;

            foreach (var modify in ModifyStates)
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

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (modifyStates != null)
                {
                    if (disposing)
                    {
                        _isDisposing = true;
                        for (var i = modifyStates.Count - 1; i >= 0; i--) modifyStates[i].Dispose();
                    }

                    modifyStates.Clear();
                }

                valueBase = default;
                _disposed = true;
            }

            base.Dispose(disposing);
        }
    }
}
#endif