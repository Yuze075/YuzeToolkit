#nullable enable
#if YUZE_TOOLKIT_USE_UNITY_NETCODE
using System;
using Unity.Netcode;
using UnityEngine;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.BindableTool.Network
{
    [Serializable]
    public abstract class NetModifiableBase<TValue, TModify> : NetBindableBase<TValue>, IModifiable
        where TModify : IModify
    {
        protected NetModifiableBase(TValue? valueBase, bool isReadOnly, NetworkVariableReadPermission readPerm,
            NetworkVariableWritePermission writePerm, IModifiableOwner? modifiableOwner,
            ILogging? loggingParent) : base(valueBase!, readPerm, writePerm,
            loggingParent ?? modifiableOwner as ILogging)
        {
            _owner = modifiableOwner;
            this.isReadOnly = isReadOnly;
        }

        ~NetModifiableBase() => Dispose(false);
        [NonSerialized] private bool _disposed;
        private IModifiableOwner? _owner;
        [SerializeField] private bool isReadOnly;

        public IModifiableOwner? Owner
        {
            get
            {
                if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
                return _owner;
            }
        }

        public bool IsReadOnly => isReadOnly;

        public bool Modify<TModifyIn>(TModifyIn modify, IModifiableOwner? sender, object? reason)
            where TModifyIn : IModify
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            if (isReadOnly && Owner != null && sender != Owner)
            {
                Logging.LogWarning($"当前类型为:{GetType()}的IModifiable为ReadOnly状态, " +
                                   $"只有{nameof(sender)}(类型为{sender?.GetType()})和{nameof(Owner)}(类型为{Owner?.GetType()})相同才可以修改!");
                return false;
            }

            if (modify is not TModify tModify)
            {
                Logging.LogError($"{nameof(modify)}(类型为{modify.GetType()}), 不是需要的{typeof(TModify)}类型, " +
                                 $"不能修正当前对象(类型为{GetType()})!");
                return false;
            }

            var maybeNullModify = tModify;
            Owner?.CheckModify(this, ref maybeNullModify, reason);
            if (maybeNullModify == null) return false;
            Modify(maybeNullModify);
            return true;
        }

        protected abstract void Modify(TModify modify);

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _owner = null;
                _disposed = true;
            }

            base.Dispose(disposing);
        }
    }
}
#endif