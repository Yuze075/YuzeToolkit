using Toolbox.Editor;
using UnityEditor;
using YuzeToolkit.Network;

namespace YuzeToolkit.Editor.Network
{
    [CustomEditor(typeof(NetworkBase), true, isFallback = false)]
    [CanEditMultipleObjects]
    public class NetworkBaseEditor : ToolboxEditor
    {
    }
}