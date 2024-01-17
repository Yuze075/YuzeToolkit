#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.BindableTool
{
    /// <inheritdoc cref="YuzeToolkit.BindableTool.IState" />
    [Serializable]
    public abstract class State : ModifiableBase<bool, ModifyState>, IState
    {
        protected State(bool value = default, bool isReadOnly = true, IModifiableOwner? modifiableOwner = null,
            ILogging? loggingParent = null) : base(isReadOnly, modifiableOwner, loggingParent)
        {
            valueBase = value;
            this.value = value;
        }

        ~State() => Dispose(false);
        [NonSerialized] private bool _disposed;
        [NonSerialized] private bool _isDisposing;
        [SerializeField] private bool value;
        [SerializeField] private bool valueBase;

        public sealed override bool Value
        {
            get
            {
                if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
                return value;
            }
            protected set
            {
                if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
                if (this.value == value) return;
                ValueChange?.Invoke(this.value, value);
                this.value = value;
            }
        }

        public bool this[int priority, bool removeSmall = true]
        {
            get
            {
                if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
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

                value = default;
                valueBase = default;
                _disposed = true;
            }

            base.Dispose(disposing);
        }
    }
}