#nullable enable
#if YUZE_USE_UNITY_NETCODE
using Unity.Netcode;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.UITool.Network
{
    public class NetUIBase : NetworkBehaviour, IBelongUICore
    {
        private IUICore? _core;

        IUICore IBelongUICore.Core
        {
            get
            {
                this.IsNotNull(_core != null, nameof(_core));
                return _core;
            }
            set => _core = value;
        }
    }
}
#endif