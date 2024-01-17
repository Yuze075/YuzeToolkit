#nullable enable
#if YUZE_TOOLKIT_USE_UNITY_NETCODE
using System;
using Unity.Netcode;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.BindableTool.Network
{
    /// <summary>
    /// <inheritdoc cref="IField{TValue}" />
    /// 并且通过<see cref="NetworkVariable{T}"/>进行网络变量的同步<br/><br/>
    /// </summary>
    /// <typeparam name="TValue">属性的数据类型</typeparam>
    [Serializable]
    public class NetField<TValue> : NetBindableBase<TValue>, IField<TValue>
    {
        public NetField(TValue? valueBase = default,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server,
            ILogging? loggingParent = null) :
            base(valueBase!, readPerm, writePerm, loggingParent)
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

        public void SetValue(TValue? value) => Value = value;
    }
}
#endif