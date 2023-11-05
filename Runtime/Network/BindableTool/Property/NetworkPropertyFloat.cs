#if USE_UNITY_NETCODE
using System;
using Unity.Netcode;

namespace YuzeToolkit.Network.BindableTool
{
    /// <summary>
    /// <inheritdoc/>
    /// 使用<see cref="float"/>的数据类型进行属性的计算
    /// </summary>
    [Serializable]
    public abstract class NetworkPropertyFloat : NetworkProperty<float>
    {
        protected NetworkPropertyFloat(float valueBase,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) : base(valueBase,
            readPerm, writePerm)
        {
        }

        public override float Min => float.MinValue;
        public override float Max => float.MaxValue;
        protected sealed override double CastToDouble(float value) => value;

        protected sealed override float CastToTValue(double value) => value switch
        {
            >= float.MaxValue => float.MaxValue,
            <= float.MinValue => float.MinValue,
            _ => (float)value
        };
    }
}
#endif