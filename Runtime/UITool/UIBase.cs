using YuzeToolkit.DriverTool;

namespace YuzeToolkit.UITool
{
    /// <summary>
    /// <inheritdoc cref="IBelongUICore" />
    /// </summary>
    public abstract class UIBase : MonoBase, IBelongUICore
    {
        private IUICore? _core;
        IUICore IBelongUICore.Core => IsNotNull(_core, message: $"Type: {GetType()}");
        public void SetCore(IUICore core) => _core = core;
    }
}