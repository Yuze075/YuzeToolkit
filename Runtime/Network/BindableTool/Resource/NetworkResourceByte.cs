#if USE_UNITY_NETCODE
using Unity.Netcode;

namespace YuzeToolkit.Network.BindableTool
{
    /// <summary>
    /// <inheritdoc/>
    /// 使用<see cref="byte"/>的数据类型进行资源的计算
    /// </summary>
    public abstract class NetworkResourceByte : NetworkResource<byte>
    {
        protected NetworkResourceByte(byte valueBase,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) : base(valueBase,
            readPerm, writePerm)
        {
        }

        public override byte Min => byte.MinValue;
        public override byte Max => byte.MaxValue;
        protected sealed override float CastToFloat(byte value) => value;

        protected sealed override byte CastToTValue(float value) => value switch
        {
            >= byte.MaxValue => byte.MaxValue,
            <= byte.MinValue => byte.MinValue,
            _ => (byte)value
        };
    }
}
#endif