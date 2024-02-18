#nullable enable
#if YUZE_USE_UNITY_NETCODE && YUZE_USE_EDITOR_TOOLBOX
using Toolbox.Editor;
using Unity.Netcode;
using UnityEditor;

namespace YuzeToolkit.Network.Editor
{
    [CustomEditor(typeof(NetworkBehaviour), true, isFallback = false)]
    [CanEditMultipleObjects]
    public class NetBaseEditor : ToolboxEditor
    {
    }
}
#endif