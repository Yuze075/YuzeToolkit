#if USE_UNITY_NETCODE
using System;
using Unity.Netcode;

namespace YuzeToolkit.Network.BindableTool
{
    /// <summary>
    /// <inheritdoc/>
    /// 使用<see cref="byte"/>的数据类型进行属性的计算
    /// </summary>
    [Serializable]
    public abstract class NetworkPropertyByte : NetworkProperty<byte>
    {
        protected NetworkPropertyByte(byte valueBase,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) : base(valueBase,
            readPerm, writePerm)
        {
        }

        public override byte Min => byte.MinValue;
        public override byte Max => byte.MaxValue;
        protected sealed override double CastToDouble(byte value) => value;

        protected sealed override byte CastToTValue(double value) => value switch
        {
            >= byte.MaxValue => byte.MaxValue,
            <= byte.MinValue => byte.MinValue,
            _ => (byte)value
        };
    }
}
#endif