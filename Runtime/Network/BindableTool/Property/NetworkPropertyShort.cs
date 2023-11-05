#if USE_UNITY_NETCODE
using System;
using Unity.Netcode;

namespace YuzeToolkit.Network.BindableTool
{
    /// <summary>
    /// <inheritdoc/>
    /// 使用<see cref="short"/>的数据类型进行属性的计算
    /// </summary>
    [Serializable]
    public abstract class NetworkPropertyShort : NetworkProperty<short>
    {
        protected NetworkPropertyShort(short valueBase,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) : base(valueBase,
            readPerm, writePerm)
        {
        }

        public override short Min => short.MinValue;
        public override short Max => short.MaxValue;
        protected sealed override double CastToDouble(short value) => value;

        protected sealed override short CastToTValue(double value) => value switch
        {
            >= short.MaxValue => short.MaxValue,
            <= short.MinValue => short.MinValue,
            _ => (short)value
        };
    }
}
#endif