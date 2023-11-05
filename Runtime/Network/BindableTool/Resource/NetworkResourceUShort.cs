#if USE_UNITY_NETCODE
using Unity.Netcode;

namespace YuzeToolkit.Network.BindableTool
{
    /// <summary>
    /// <inheritdoc/>
    /// 使用<see cref="ushort"/>的数据类型进行资源的计算
    /// </summary>
    public abstract class NetworkResourceUShort : NetworkResource<ushort>
    {
        protected NetworkResourceUShort(ushort valueBase,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) : base(valueBase,
            readPerm, writePerm)
        {
        }

        public override ushort Min => ushort.MinValue;
        public override ushort Max => ushort.MaxValue;
        protected sealed override float CastToFloat(ushort value) => value;

        protected sealed override ushort CastToTValue(float value) => value switch
        {
            >= ushort.MaxValue => ushort.MaxValue,
            <= ushort.MinValue => ushort.MinValue,
            _ => (ushort)value
        };
    }
}
#endif