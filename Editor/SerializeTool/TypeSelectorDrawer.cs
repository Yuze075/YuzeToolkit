#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using YuzeToolkit.AttributeTool;
using YuzeToolkit.AttributeTool.Editor;

namespace YuzeToolkit.SerializeTool.Editor
{
    [CustomPropertyDrawer(typeof(TypeSelectorAttribute))]
    public class TypeSelectorDrawer : PropertyDrawer
    {
        private const int MaxTypePopupLineCount = 13;
        private static readonly GUIContent NullDisplayName = new("<null>");


        private readonly Dictionary<string, TypePopupCache> _typePopupCaches = new();
        private readonly Dictionary<string, GUIContent> _typeNameCaches = new();
        private TypeSelectorAttribute TypeAttribute => (TypeSelectorAttribute)attribute;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);
            SerializedProperty strProperty;

            if (property.propertyType == SerializedPropertyType.String)
            {
                strProperty = property;
            }
            else
            {
                strProperty = property.FindPropertyRelative("assemblyQualifiedTypeName");
                if (strProperty == null)
                {
                    EditorHelper.DrawWarningMessage(rect, $"错误的特性使用, 对象不是{typeof(string)}或者{typeof(SerializeType)}!");
                    EditorGUI.EndProperty();
                    return;
                }
            }

            var popupPosition = rect;

            if (!string.IsNullOrWhiteSpace(label.text))
            {
                var labelPosition = rect;
                labelPosition.width = EditorGUIUtility.labelWidth;
                labelPosition.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(labelPosition, label);

                popupPosition.width -= EditorGUIUtility.labelWidth;
                popupPosition.x += EditorGUIUtility.labelWidth;
                popupPosition.height = EditorGUIUtility.singleLineHeight;
            }

            if (EditorGUI.DropdownButton(popupPosition, GetTypeName(strProperty), FocusType.Keyboard))
            {
                var popup = GetTypePopup(strProperty);
                popup.TypePopup.Show(popupPosition);
            }

            EditorGUI.EndProperty();
        }

        private TypePopupCache GetTypePopup(SerializedProperty strProperty)
        {
            var baseTypeName = TypeAttribute.AssemblyQualifiedBaseTypeName;
            if (_typePopupCaches.TryGetValue(baseTypeName, out var result)) return result;

            var state = new AdvancedDropdownState();
            var baseType = TypeAttribute.BaseType;
            var popup = new TypePopup(
                TypeCache.GetTypesDerivedFrom(baseType).Append(baseType).Where(TypeAttribute.CheckType),
                MaxTypePopupLineCount, state);

            popup.OnItemSelected += item =>
            {
                var type = item.Type;
                strProperty.stringValue = type != null ? type.FullName + "," + type.Assembly.GetName().Name : null;
                strProperty.serializedObject.ApplyModifiedProperties();
                strProperty.serializedObject.Update();
            };

            result = new TypePopupCache(popup);
            _typePopupCaches.Add(baseTypeName, result);

            return result;
        }

        private GUIContent GetTypeName(SerializedProperty strProperty)
        {
            var baseTypeName = strProperty.stringValue;
            if (string.IsNullOrEmpty(baseTypeName)) return NullDisplayName;
            if (_typeNameCaches.TryGetValue(baseTypeName, out var cachedTypeName)) return cachedTypeName;

            var type = Type.GetType(baseTypeName);
            if (type == null) return NullDisplayName;
            string? typeName = null;

            var subclassMenu = TypePopup.GetAttribute(type);
            if (subclassMenu != null)
            {
                typeName = subclassMenu.TypeNameWithoutPath;
                if (!string.IsNullOrWhiteSpace(typeName))
                {
                    typeName = ObjectNames.NicifyVariableName(typeName);
                }
            }

            if (string.IsNullOrWhiteSpace(typeName))
            {
                typeName = ObjectNames.NicifyVariableName(type.Name);
            }

            var result = new GUIContent(typeName);
            _typeNameCaches.Add(baseTypeName, result);
            return result;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
            EditorGUIUtility.singleLineHeight;
    }

    public struct TypePopupCache
    {
        public TypePopup TypePopup { get; }

        public TypePopupCache(TypePopup typePopup) => TypePopup = typePopup;
    }

    public class TypePopup : AdvancedDropdown
    {
        public static string[] GetSplitTypePath(Type type)
        {
            var typeMenu = GetAttribute(type);
            if (typeMenu != null)
            {
                return typeMenu.SplitMenuName;
            }

            if (type.FullName == null) return new[] { type.Name };
            var splitIndex = type.FullName.LastIndexOf('.');
            if (splitIndex >= 0)
            {
                return new[]
                    { type.FullName[..splitIndex], type.FullName[(splitIndex + 1)..] };
            }

            return new[] { type.Name };
        }

        public static AddSubclassMenuAttribute? GetAttribute(Type type)
        {
            return Attribute.GetCustomAttribute(type, typeof(AddSubclassMenuAttribute)) as AddSubclassMenuAttribute;
        }

        public static IEnumerable<Type> OrderByType(IEnumerable<Type> source)
        {
            return source.OrderBy(type =>
            {
                if (type == null)
                {
                    return -999;
                }

                return GetAttribute(type)?.Order ?? 0;
            }).ThenBy(type =>
            {
                if (type == null)
                {
                    return null;
                }

                return GetAttribute(type)?.MenuName ?? type.Name;
            });
        }

        private const int KMaxNamespaceNestCount = 16;
        private const string KNullDisplayName = "<null>";

        private static void AddTo(AdvancedDropdownItem root, IEnumerable<Type> types)
        {
            var itemCount = 0;

            // Add null item.
            var nullItem = new TypePopupItem(null, KNullDisplayName)
            {
                id = itemCount++
            };
            root.AddChild(nullItem);

            var typeArray = OrderByType(types).ToArray();
            var isSingleNamespace = true;
            var namespaces = new string?[KMaxNamespaceNestCount];
            foreach (var type in typeArray)
            {
                var splitTypePath = GetSplitTypePath(type);
                if (splitTypePath.Length <= 1)
                {
                    continue;
                }

                // If they explicitly want sub category, let them do.
                if (GetAttribute(type) != null)
                {
                    isSingleNamespace = false;
                    break;
                }

                for (var k = 0; (splitTypePath.Length - 1) > k; k++)
                {
                    var ns = namespaces[k];
                    if (ns == null)
                    {
                        namespaces[k] = splitTypePath[k];
                    }
                    else if (ns != splitTypePath[k])
                    {
                        isSingleNamespace = false;
                        break;
                    }
                }

                if (!isSingleNamespace)
                {
                    break;
                }
            }

            // Add type items.
            foreach (var type in typeArray)
            {
                var splitTypePath = GetSplitTypePath(type);
                if (splitTypePath.Length == 0)
                {
                    continue;
                }

                var parent = root;

                // Add namespace items.
                if (!isSingleNamespace)
                {
                    for (var k = 0; (splitTypePath.Length - 1) > k; k++)
                    {
                        var foundItem = GetItem(parent, splitTypePath[k]);
                        if (foundItem != null)
                        {
                            parent = foundItem;
                        }
                        else
                        {
                            var newItem = new AdvancedDropdownItem(splitTypePath[k])
                            {
                                id = itemCount++,
                            };
                            parent.AddChild(newItem);
                            parent = newItem;
                        }
                    }
                }

                // Add type item.
                var item = new TypePopupItem(type,
                    ObjectNames.NicifyVariableName(splitTypePath[^1]))
                {
                    id = itemCount++
                };
                parent.AddChild(item);
            }
        }

        private static AdvancedDropdownItem? GetItem(AdvancedDropdownItem parent, string name)
        {
            return parent.children.FirstOrDefault(item => item.name == name);
        }

        private static readonly float KHeaderHeight = EditorGUIUtility.singleLineHeight * 2f;

        private readonly Type[] _mTypes;

        public event Action<TypePopupItem>? OnItemSelected;

        public TypePopup(IEnumerable<Type> types, int maxLineCount, AdvancedDropdownState state) :
            base(state)
        {
            _mTypes = types.ToArray();
            minimumSize = new Vector2(minimumSize.x,
                EditorGUIUtility.singleLineHeight * maxLineCount + KHeaderHeight);
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem("Select Type");
            AddTo(root, _mTypes);
            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            base.ItemSelected(item);
            if (item is TypePopupItem typePopupItem)
            {
                OnItemSelected?.Invoke(typePopupItem);
            }
        }
    }

    public class TypePopupItem : AdvancedDropdownItem
    {
        public Type? Type { get; }
        public TypePopupItem(Type? type, string name) : base(name) => Type = type;
    }
}