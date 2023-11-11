using System;
using System.Collections.Generic;
using UnityEngine;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.BindableTool
{
    /// <inheritdoc/>
    [Serializable]
    public class State : IState
    {
        protected State(bool value)
        {
            valueBase = value;
            this.value = value;
        }

        private SLogTool? _sLogTool;
        private IModifiableOwner? _owner;

        protected ILogTool LogTool => _sLogTool ??= SLogTool.Create(GetLogTags);

        protected virtual string[] GetLogTags => new[]
        {
            nameof(IState),
            GetType().FullName
        };

        void IBindable.SetLogParent(ILogTool parent) => ((SLogTool)LogTool).Parent = parent;

        IModifiableOwner IModifiable.Owner => LogTool.IsNotNull(_owner);

        void IModifiable.SetOwner(IModifiableOwner value)
        {
            if (_owner != null)
                LogTool.Log(
                    $"类型为{GetType()}的{nameof(IModifiable)}的{nameof(_owner)}从{_owner.GetType()}替换为{value.GetType()}");
            _owner = value;
        }

        [SerializeField] private bool value;
        [SerializeField] private bool valueBase;
        [SerializeField] private bool isReadOnly;

        public bool Value
        {
            get => value;
            private set
            {
                if (this.value == value) return;
                _onValueChange?.Invoke(this.value, value);
                this.value = value;
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
                if (modifyStates.Remove(modifyState)) ReCheckValue();
            }), this);
            ReCheckValue();
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
            return new UnRegister(() => { _onValueChange -= onValueChange; });
        }

        public IDisposable RegisterChangeBuff(ValueChange<bool> onValueChange)
        {
            onValueChange(!Value, Value);
            return RegisterChange(onValueChange);
        }

        #endregion

        #region IDisposable

        void IDisposable.Dispose()
        {
            SLogTool.Release(ref _sLogTool);
            modifyStates.Clear();
            ReCheckValue();
            _onValueChange = null;
        }

        #endregion

        public static implicit operator bool(State state) => state.Value;
    }
}