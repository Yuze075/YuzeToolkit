#if USE_UNITY_NETCODE
using System;
using Unity.Netcode;

namespace YuzeToolkit.Network.BindableTool
{
    /// <summary>
    /// <inheritdoc/>
    /// 使用<see cref="ushort"/>的数据类型进行属性的计算
    /// </summary>
    [Serializable]
    public abstract class NetworkPropertyUShort : NetworkProperty<ushort>
    {
        protected NetworkPropertyUShort(ushort valueBase,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) : base(valueBase,
            readPerm, writePerm)
        {
        }

        public override ushort Min => ushort.MinValue;
        public override ushort Max => ushort.MaxValue;
        protected sealed override double CastToDouble(ushort value) => value;

        protected sealed override ushort CastToTValue(double value) => value switch
        {
            >= ushort.MaxValue => ushort.MaxValue,
            <= ushort.MinValue => ushort.MinValue,
            _ => (ushort)value
        };
    }
}
#endif