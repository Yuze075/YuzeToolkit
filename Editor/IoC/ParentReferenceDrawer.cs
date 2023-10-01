using System;
using UnityEditor;
using UnityEngine;
using YuzeToolkit.IoC;

namespace YuzeToolkit.Editor.IoC
{
    [CustomPropertyDrawer(typeof(ParentReference))]
    public class ParentReferenceDrawer : PropertyDrawer
    {
        static ParentReferenceDrawer()
        {
            EditorApplication.playModeStateChanged += PlayModeStateChanged;
        }

        private static void PlayModeStateChanged(PlayModeStateChange obj)
        {
            switch (obj)
            {
                case PlayModeStateChange.EnteredEditMode:
                    _isPlayMode = false;
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    _isPlayMode = true;
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(obj), obj, null);
            }
        }

        private static bool _isPlayMode;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            if (_isPlayMode)
            {
                GUI.enabled = false;
                var parent = property.FindPropertyRelative("parent");
                EditorGUI.PropertyField(rect, parent, new GUIContent("Parent"));
                GUI.enabled = true;
            }
            else
            {
                var isRoot = property.FindPropertyRelative("isRoot");
                var parentKey = property.FindPropertyRelative("parentKey");
                var selfKey = property.FindPropertyRelative("selfKey");

                var isRootValue = isRoot.boolValue;

                EditorGUI.PropertyField(
                    new Rect(rect.position, new Vector2(rect.width, rect.height / (isRootValue ? 2 : 3))), isRoot,
                    new GUIContent("IsRoot"));
                if (!isRootValue)
                {
                    EditorGUI.PropertyField(
                        new Rect(new Vector2(rect.x, rect.y + rect.height / 3),
                            new Vector2(rect.width, rect.height / 3)),
                        parentKey, new GUIContent("ParentKey"));
                }

                EditorGUI.PropertyField(
                    new Rect(new Vector2(rect.x, rect.y + rect.height / (isRootValue ? 2 : 3f / 2f)),
                        new Vector2(rect.width, rect.height / (isRootValue ? 2 : 3))), selfKey,
                    new GUIContent("SelfKey"));
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (_isPlayMode)
            {
                var parent = property.FindPropertyRelative("parent");
                return EditorGUI.GetPropertyHeight(parent);
            }

            var isRoot = property.FindPropertyRelative("isRoot");
            var isRootValue = isRoot.boolValue;
            return EditorGUI.GetPropertyHeight(isRoot) * (isRootValue ? 2 : 3);
        }
    }
}