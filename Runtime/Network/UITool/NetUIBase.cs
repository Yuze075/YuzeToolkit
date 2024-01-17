#nullable enable
#if YUZE_TOOLKIT_USE_UNITY_NETCODE
using YuzeToolkit.Network;

namespace YuzeToolkit.UITool.Network
{
    public class NetUIBase : NetBase, IBelongUICore
    {
        private IUICore? _core;
        IUICore IBelongUICore.Core
        {
            get
            {
                IsNotNull(_core != null, nameof(_core));
                return _core;
            }
            set => _core = value;
        }
    }
}
#endif