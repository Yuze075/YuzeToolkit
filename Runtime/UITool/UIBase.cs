#nullable enable
namespace YuzeToolkit.UITool
{
    /// <summary>
    /// <inheritdoc cref="IBelongUICore" />
    /// </summary>
    public abstract class UIBase : MonoBase, IBelongUICore
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