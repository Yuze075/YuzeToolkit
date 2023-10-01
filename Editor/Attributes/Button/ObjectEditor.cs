using UnityEditor;

namespace YuzeToolkit.Editor.Attributes.Button
{
    using Object = UnityEngine.Object;

    /// <summary>
    /// Custom inspector for <see cref="UnityEngine.Object"/> including derived classes.
    /// </summary>
    [CustomEditor(typeof(Object), true)]
    [CanEditMultipleObjects]
    internal class ObjectEditor : UnityEditor.Editor
    {
        private ButtonsDrawer _buttonsDrawer;

        private void OnEnable()
        {
            _buttonsDrawer = new ButtonsDrawer(target);
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            _buttonsDrawer.DrawButtons(targets);
        }
    }
}
