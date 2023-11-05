#if USE_UNITY_NETCODE
using System;
using Unity.Netcode;

namespace YuzeToolkit.Network.BindableTool
{
    /// <summary>
    /// <inheritdoc/>
    /// 使用<see cref="int"/>的数据类型进行属性的计算
    /// </summary>
    [Serializable]
    public abstract class NetworkPropertyInt : NetworkProperty<int>
    {
        protected NetworkPropertyInt(int valueBase,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) : base(valueBase,
            readPerm, writePerm)
        {
        }

        public override int Min => int.MinValue;
        public override int Max => int.MaxValue;
        protected sealed override double CastToDouble(int value) => value;

        protected sealed override int CastToTValue(double value) => value switch
        {
            >= int.MaxValue => int.MaxValue,
            <= int.MinValue => int.MinValue,
            _ => (int)value
        };
    }
}
#endif