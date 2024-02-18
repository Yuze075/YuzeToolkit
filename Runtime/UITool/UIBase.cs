#nullable enable
using UnityEngine;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.UITool
{
    /// <summary>
    /// <inheritdoc cref="IBelongUICore" />
    /// </summary>
    public abstract class UIBase : MonoBehaviour, IBelongUICore
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