#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace YuzeToolkit.BindableTool
{
    /// <summary>
    /// <inheritdoc cref="YuzeToolkit.BindableTool.IModify" />
    /// 对<see cref="IState"/>的状态值<see cref="IState.Value"/>的修饰<br/>
    /// 优先级越高越后修正, 在同一优先级先进行所有的<see cref="OrModifyState"/>修正, 再进行所有的<see cref="AndModifyState"/>修正<br/><br/>
    /// </summary>
    [Serializable]
    public abstract class ModifyState : IModify, IDisposable, IComparer<ModifyState>
    {
        protected ModifyState(int priority, IState state)
        {
            this.priority = priority;
            _state = state;
        }

        ~ModifyState() => Dispose(false);
        [NonSerialized] private bool _disposed;
        [SerializeField] private int priority;

        /// <summary>
        /// 修改<see cref="IState"/>的优先级, 优先级越高, 越后修正, 升序排列<br/>
        /// 同一优先级先进行所有的<see cref="OrModifyState"/>修正, 再进行所有的<see cref="AndModifyState"/>修正
        /// </summary>
        public int Priority =>
            _disposed ? default : priority; // throw new ObjectDisposedException($"{GetType().Name}已经被释放！");

        private IState? _state;

        protected void ReCheckValue()
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            _state?.ReCheckValue();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            priority = 0;
            if (disposing && _state != null) _state.RemoveModify(this);
            _state = null;
            _disposed = true;
        }

        int IComparer<ModifyState>.Compare(ModifyState x, ModifyState y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (ReferenceEquals(null, y)) return 1;
            if (ReferenceEquals(null, x)) return -1;

            if (x.Priority > y.Priority) return 1;
            if (x.Priority < y.Priority) return -1;


            return x switch
            {
                OrModifyState => y is OrModifyState ? 0 : -1,
                AndModifyState => y is AndModifyState ? 0 : 1,
                _ => 0
            };
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// 与值修饰, 和原来值进行相与
    /// </summary>
    [Serializable]
    public sealed class AndModifyState : ModifyState
    {
        public static bool Create(IState state, int priority, bool orValue, IModifiableOwner? sender,
            [NotNullWhen(true)] out AndModifyState? andModifyState, object? reason = null)
        {
            andModifyState = new AndModifyState(priority, orValue, state);
            if (state.Modify(andModifyState, sender, reason)) return true;
            andModifyState = null;
            return false;
        }

        private AndModifyState(int priority, bool andValue, IState state) : base(priority, state) =>
            this.andValue = andValue;

        ~AndModifyState() => Dispose(false);
        [NonSerialized] private bool _disposed;
        [SerializeField] private bool andValue;

        /// <summary>
        /// 与值修正
        /// </summary>
        public bool AndValue
        {
            get => _disposed ? default : andValue; // throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            set
            {
                if (_disposed) return; // throw new ObjectDisposedException($"{GetType().Name}已经被释放！");

                if (andValue == value) return;
                andValue = value;
                ReCheckValue();
            }
        }

        protected override void Dispose(bool _)
        {
            if (!_disposed)
            {
                andValue = true;
                _disposed = true;
            }

            base.Dispose(_);
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// 或值修饰, 和原来值进行相或
    /// </summary>
    [Serializable]
    public sealed class OrModifyState : ModifyState
    {
        public static bool Create(IState state, int priority, bool orValue, IModifiableOwner? sender,
            [NotNullWhen(true)] out OrModifyState? orModifyState, object? reason = null)
        {
            orModifyState = new OrModifyState(priority, orValue, state);
            if (state.Modify(orModifyState, sender, reason)) return true;
            orModifyState = null;
            return false;
        }

        private OrModifyState(int priority, bool orValue, IState state) : base(priority, state) =>
            this.orValue = orValue;

        ~OrModifyState() => Dispose(false);
        [NonSerialized] private bool _disposed;
        [SerializeField] private bool orValue;

        /// <summary>
        /// 或值修正
        /// </summary>
        public bool OrValue
        {
            get => _disposed ? default : orValue; // throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            set
            {
                if (_disposed) return; // throw new ObjectDisposedException($"{GetType().Name}已经被释放！");

                if (orValue == value) return;
                orValue = value;
                ReCheckValue();
            }
        }

        protected override void Dispose(bool _)
        {
            if (!_disposed)
            {
                orValue = false;
                _disposed = true;
            }

            base.Dispose(_);
        }
    }
}