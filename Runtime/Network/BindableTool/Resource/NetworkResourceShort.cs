#if USE_UNITY_NETCODE
using Unity.Netcode;

namespace YuzeToolkit.Network.BindableTool
{
    /// <summary>
    /// <inheritdoc/>
    /// 使用<see cref="short"/>的数据类型进行资源的计算
    /// </summary>
    public abstract class NetworkResourceShort : NetworkResource<short>
    {
        protected NetworkResourceShort(short valueBase,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) : base(valueBase,
            readPerm, writePerm)
        {
        }

        public override short Min => short.MinValue;
        public override short Max => short.MaxValue;
        protected sealed override float CastToFloat(short value) => value;

        protected sealed override short CastToTValue(float value) => value switch
        {
            >= short.MaxValue => short.MaxValue,
            <= short.MinValue => short.MinValue,
            _ => (short)value
        };
    }
}
#endif