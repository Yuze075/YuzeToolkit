#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace YuzeToolkit.BindableTool
{
    /// <summary>
    /// <see cref="ModifyState"/>修正类型
    /// </summary>
    public enum EModifyStateType : byte
    {
        /// <summary>
        /// 或值修正
        /// </summary>
        Or,

        /// <summary>
        /// 与值修正
        /// </summary>
        And
    }

    public interface IModifyState : IComparer<IModifyState>
    {
        /// <summary>
        /// 优先级越小越先修正, 在同一优先级先进行所有的<see cref="EModifyStateType.Or"/>修正,
        /// 再进行所有的<see cref="EModifyStateType.And"/>修正
        /// </summary>
        public int Priority { get; }

        /// <summary>
        /// 修正类型
        /// </summary>
        public EModifyStateType ModifyStateType { get; }

        /// <summary>
        /// 修正值
        /// </summary>
        public bool ModifyValue { get; }

        int IComparer<IModifyState>.Compare(IModifyState x, IModifyState y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (ReferenceEquals(null, y)) return 1;
            if (ReferenceEquals(null, x)) return -1;

            if (x.Priority > y.Priority) return 1;
            if (x.Priority < y.Priority) return -1;

            return x.ModifyStateType switch
            {
                EModifyStateType.Or => y.ModifyStateType == EModifyStateType.Or ? 0 : -1,
                EModifyStateType.And => y.ModifyStateType == EModifyStateType.And ? 0 : 1,
                _ => 0
            };
        }
    }

    /// <summary>
    /// 对<see cref="IState{TModifyState}"/>的<see cref="IState{TModifyState}.Value"/>进行修正<br/>
    /// 优先级越小越先修正, 在同一优先级先进行所有的<see cref="EModifyStateType.Or"/>修正,
    /// 再进行所有的<see cref="EModifyStateType.And"/>修正<br/><br/>
    /// </summary>
    [Serializable]
    public sealed class ModifyState : IModifyState, IDisposable
    {
        public static ModifyState Create(int priority, EModifyStateType modifyStateType, bool modifyValue,
            IState<IModifyState> state, object? sender = null, object? reason = null)
        {
            var modifyState = new ModifyState(priority, modifyStateType, modifyValue, state);
            state.Modify(modifyState, sender, reason);
            return modifyState;
        }

        public static bool TryCreate(int priority, EModifyStateType modifyStateType, bool modifyValue,
            IReadOnlyBindable bindable, [MaybeNullWhen(false)] out ModifyState modifyState,
            object? sender = null, object? reason = null)
        {
            if (bindable is not IState<IModifyState> state)
            {
                modifyState = null;
                return false;
            }

            modifyState = new ModifyState(priority, modifyStateType, modifyValue, state);
            state.Modify(modifyState, sender, reason);
            return true;
        }

        private ModifyState(int priority, EModifyStateType modifyStateType, bool modifyValue,
            IState<IModifyState> state)
        {
            this.priority = priority;
            this.modifyStateType = modifyStateType;
            this.modifyValue = modifyValue;
            _state = state;
        }

        [SerializeField] private int priority;
        [SerializeField] private EModifyStateType modifyStateType;
        [SerializeField] private bool modifyValue;
        private IState<IModifyState>? _state;
        public int Priority => priority;

        public EModifyStateType ModifyStateType
        {
            get => modifyStateType;
            set
            {
                if (modifyStateType == value) return;
                modifyStateType = value;
                _state?.ReCheckValue();
            }
        }

        public bool ModifyValue
        {
            get => modifyValue;
            set
            {
                if (modifyValue == value) return;
                modifyValue = value;
                _state?.ReCheckValue();
            }
        }

        /// <summary>
        /// 释放修正资源, 结束对<see cref="IState{TModifyState}"/>的修正
        /// </summary>
        public void Dispose()
        {
            priority = 0;
            modifyStateType = EModifyStateType.Or;
            modifyValue = false;
            _state?.RemoveModify(this);
            _state = null;
        }
    }

    /// <inheritdoc cref="YuzeToolkit.BindableTool.IState{TModifyState}" />
    [Serializable]
    public abstract class State : IState<IModifyState>
    {
        protected State()
        {
        }

        protected State(bool value, IModifiableOwner? modifiableOwner = null)
        {
            valueBase = value;
            this.value = value;
            ModifiableOwner = modifiableOwner;
        }

        /// <summary>
        /// 初始化基于Unity机制序列化的<see cref="State"/>
        /// </summary>
        public void SetOnly(bool valueBase, IModifiableOwner? modifiableOwner = null)
        {
            this.valueBase = valueBase;
            value = valueBase;
            ModifiableOwner = modifiableOwner;
        }

        [SerializeField] private bool value;
        [SerializeField] private bool valueBase;
        private ValueChange<bool>? _valueChange;
        public IModifiableOwner? ModifiableOwner { get; private set; }

        public bool Value
        {
            get => value;
            private set
            {
                if (this.value == value) return;
                _valueChange?.Invoke(this.value, value);
                this.value = value;
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
        [ReorderableList(fixedSize: true, draggable: false), ReferencePicker]
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
            value = default;
            _modifyStates?.Clear();
        }

        public static implicit operator bool(State state) => state.Value;
    }
}