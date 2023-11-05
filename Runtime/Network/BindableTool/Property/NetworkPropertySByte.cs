#if USE_UNITY_NETCODE
using System;
using Unity.Netcode;

namespace YuzeToolkit.Network.BindableTool
{
    /// <summary>
    /// <inheritdoc/>
    /// 使用<see cref="sbyte"/>的数据类型进行属性的计算
    /// </summary>
    [Serializable]
    public abstract class NetworkPropertySByte : NetworkProperty<sbyte>
    {
        protected NetworkPropertySByte(sbyte valueBase,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) : base(valueBase,
            readPerm, writePerm)
        {
        }

        public override sbyte Min => sbyte.MinValue;
        public override sbyte Max => sbyte.MaxValue;
        protected sealed override double CastToDouble(sbyte value) => value;

        protected sealed override sbyte CastToTValue(double value) => value switch
        {
            >= sbyte.MaxValue => sbyte.MaxValue,
            <= sbyte.MinValue => sbyte.MinValue,
            _ => (sbyte)value
        };
    }
}
#endif