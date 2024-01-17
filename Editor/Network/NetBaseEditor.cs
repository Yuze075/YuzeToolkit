#nullable enable
#if YUZE_TOOLKIT_USE_UNITY_NETCODE && YUZE_TOOLKIT_USE_EDITOR_TOOLBOX
using Toolbox.Editor;
using UnityEditor;

namespace YuzeToolkit.Network.Editor
{
    [CustomEditor(typeof(NetBase), true, isFallback = false)]
    [CanEditMultipleObjects]
    public class NetBaseEditor : ToolboxEditor
    {
    }
}
#endif