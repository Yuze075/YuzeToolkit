#nullable enable
#if YUZE_TOOLKIT_USE_UNITY_NETCODE
using System;
using Unity.Netcode;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.BindableTool.Network
{
    /// <summary>
    /// <inheritdoc cref="IModifiableField{TValue}" />
    /// 并且通过<see cref="NetworkVariable{T}"/>进行网络变量的同步<br/><br/>
    /// </summary>
    /// <typeparam name="TValue">属性的数据类型</typeparam>
    [Serializable]
    public abstract class NetModifiableField<TValue> : NetModifiableBase<TValue, ModifyField<TValue>>,
        IModifiableField<TValue>
    {
        protected NetModifiableField(TValue? valueBase, bool isReadOnly, NetworkVariableReadPermission readPerm,
            NetworkVariableWritePermission writePerm, IModifiableOwner? modifiableOwner,
            ILogging? loggingParent) : base(valueBase, isReadOnly, readPerm, writePerm, modifiableOwner, loggingParent)
        {
        }

        public override TValue? Value
        {
            get => BaseValue;
            protected set
            {
                if (BaseValue != null && BaseValue.Equals(value)) return;
                BaseValue = value!;
            }
        }

        protected sealed override void Modify(ModifyField<TValue> modifyField)
        {
            if (modifyField.modifyValue != null && modifyField.modifyValue.Equals(Value)) return;
            Value = modifyField.modifyValue;
        }
    }
}
#endif