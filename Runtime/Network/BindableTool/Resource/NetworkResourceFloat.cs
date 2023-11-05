#if USE_UNITY_NETCODE
using Unity.Netcode;

namespace YuzeToolkit.Network.BindableTool
{
    /// <summary>
    /// <inheritdoc/>
    /// 使用<see cref="float"/>的数据类型进行资源的计算
    /// </summary>
    public abstract class NetworkResourceFloat : NetworkResource<float>
    {
        protected NetworkResourceFloat(float valueBase,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) : base(valueBase,
            readPerm, writePerm)
        {
        }

        public override float Min => float.MinValue;
        public override float Max => float.MaxValue;
        protected sealed override float CastToFloat(float value) => value;

        protected sealed override float CastToTValue(float value) => value switch
        {
            >= float.MaxValue => float.MaxValue,
            <= float.MinValue => float.MinValue,
            _ => value
        };
    }
}
#endif