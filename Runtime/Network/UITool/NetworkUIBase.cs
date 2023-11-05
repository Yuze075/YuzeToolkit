#if USE_UNITY_NETCODE
using YuzeToolkit.UITool;

namespace YuzeToolkit.Network.UITool
{
    public class NetworkUIBase : NetworkBase, IBelongUICore
    {
        private IUICore? _core;
        IUICore IBelongUICore.Core => IsNotNull(_core, message: $"Type: {GetType()}");
        public void SetCore(IUICore core) => _core = core;
    }
}
#endif