#nullable enable
#if YUZE_TOOLKIT_USE_UNITY_NETCODE
using System;
using Unity.Netcode;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.BindableTool.Network
{
    /// <summary>
    /// <inheritdoc/>
    /// 使用<see cref="int"/>的数据类型进行属性的计算
    /// </summary>
    [Serializable]
    public abstract class NetPropertyInt : NetProperty<int>
    {
        protected NetPropertyInt(int valueBase = default, bool isReadOnly = true,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server,
            IModifiableOwner? modifiableOwner = null, ILogging? loggingParent = null) :
            base(valueBase, isReadOnly, readPerm, writePerm, modifiableOwner, loggingParent)
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