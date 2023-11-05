using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using YuzeToolkit.AttributeTool;

namespace YuzeToolkit.Editor.AttributeTool
{
    #region SubclassPropertyDrawer

    [CustomPropertyDrawer(typeof(SubclassAttribute))]
    public class SubclassDrawer : PropertyDrawer
    {
        private struct TypePopupCache
        {
            public AdvancedTypePopup TypePopup { get; }

            public TypePopupCache(AdvancedTypePopup typePopup)
            {
                TypePopup = typePopup;
            }
        }

        private const int MaxTypePopupLineCount = 13;
        private static readonly Type UnityObjectType = typeof(UnityEngine.Object);
        private static readonly GUIContent NullDisplayName = new(TypeMenuUtility.KNullDisplayName);

        private readonly Dictionary<string, TypePopupCache> _typePopups = new();
        private readonly Dictionary<string, GUIContent> _typeNameCaches = new();

        private SerializedProperty _targetProperty;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);

            if (property.propertyType == SerializedPropertyType.ManagedReference)
            {
                // Draw the subclass selector popup.
                var popupPosition = rect;
                if (!string.IsNullOrWhiteSpace(label.text))
                {
                    popupPosition.width -= EditorGUIUtility.labelWidth;
                    popupPosition.x += EditorGUIUtility.labelWidth;
                    popupPosition.height = EditorGUIUtility.singleLineHeight;
                }

                if (EditorGUI.DropdownButton(popupPosition, GetTypeName(property), FocusType.Keyboard))
                {
                    var popup = GetTypePopup(property);
                    _targetProperty = property;
                    popup.TypePopup.Show(popupPosition);
                }

                EditorGUI.PropertyField(rect, property, label, true);
            }
            else
            {
                var warningContent = new GUIContent(property.displayName + "(Incorrect Attribute Used)")
                {
                    image = EditorGUIUtility.IconContent("console.warnicon").image
                };
                EditorGUI.LabelField(rect, warningContent);
            }

            EditorGUI.EndProperty();
        }

        private TypePopupCache GetTypePopup(SerializedProperty property)
        {
            // Cache this string. This property internally call Assembly.GetName, which result in a large allocation.
            var managedReferenceFieldTypename = property.managedReferenceFieldTypename;
            if (_typePopups.TryGetValue(managedReferenceFieldTypename, out var result)) return result;

            var state = new AdvancedDropdownState();

            var baseType = ManagedReferenceUtility.GetType(managedReferenceFieldTypename);
            var popup = new AdvancedTypePopup(
                TypeCache.GetTypesDerivedFrom(baseType).Append(baseType).Where(p =>
                    (p.IsPublic || p.IsNestedPublic) &&
                    !p.IsAbstract &&
                    !p.IsGenericType &&
                    !UnityObjectType.IsAssignableFrom(p) &&
                    Attribute.IsDefined(p, typeof(SerializableAttribute))
                ),
                MaxTypePopupLineCount,
                state
            );
            popup.OnItemSelected += item =>
            {
                var type = item.Type;
                var obj = _targetProperty.SetManagedReference(type);
                _targetProperty.isExpanded = (obj != null);
                _targetProperty.serializedObject.ApplyModifiedProperties();
                _targetProperty.serializedObject.Update();
            };

            result = new TypePopupCache(popup);
            _typePopups.Add(managedReferenceFieldTypename, result);

            return result;
        }

        private GUIContent GetTypeName(SerializedProperty property)
        {
            // Cache this string.
            var managedReferenceFullTypename = property.managedReferenceFullTypename;

            if (string.IsNullOrEmpty(managedReferenceFullTypename))
            {
                return NullDisplayName;
            }

            if (_typeNameCaches.TryGetValue(managedReferenceFullTypename, out GUIContent cachedTypeName))
            {
                return cachedTypeName;
            }

            var type = ManagedReferenceUtility.GetType(managedReferenceFullTypename);
            string typeName = null;

            var typeMenu = TypeMenuUtility.GetAttribute(type);
            if (typeMenu != null)
            {
                typeName = typeMenu.TypeNameWithoutPath;
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
            _typeNameCaches.Add(managedReferenceFullTypename, result);
            return result;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, true);
        }

        #region AdvancedDropdown

        public class AdvancedTypePopupItem : AdvancedDropdownItem
        {
            public Type Type { get; }

            public AdvancedTypePopupItem(Type type, string name) : base(name)
            {
                Type = type;
            }
        }

        public class AdvancedTypePopup : AdvancedDropdown
        {
            private const int KMaxNamespaceNestCount = 16;

            private static void AddTo(AdvancedDropdownItem root, IEnumerable<Type> types)
            {
                var itemCount = 0;

                // Add null item.
                var nullItem = new AdvancedTypePopupItem(null, TypeMenuUtility.KNullDisplayName)
                {
                    id = itemCount++
                };
                root.AddChild(nullItem);

                var typeArray = types.OrderByType().ToArray();
                var isSingleNamespace = true;
                var namespaces = new string[KMaxNamespaceNestCount];
                foreach (var type in typeArray)
                {
                    var splitTypePath = TypeMenuUtility.GetSplitTypePath(type);
                    if (splitTypePath.Length <= 1)
                    {
                        continue;
                    }

                    // If they explicitly want sub category, let them do.
                    if (TypeMenuUtility.GetAttribute(type) != null)
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
                    var splitTypePath = TypeMenuUtility.GetSplitTypePath(type);
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
                    var item = new AdvancedTypePopupItem(type,
                        ObjectNames.NicifyVariableName(splitTypePath[^1]))
                    {
                        id = itemCount++
                    };
                    parent.AddChild(item);
                }
            }

            private static AdvancedDropdownItem GetItem(AdvancedDropdownItem parent, string name)
            {
                return parent.children.FirstOrDefault(item => item.name == name);
            }

            private static readonly float KHeaderHeight = EditorGUIUtility.singleLineHeight * 2f;

            private Type[] _mTypes;

            public event Action<AdvancedTypePopupItem> OnItemSelected;

            public AdvancedTypePopup(IEnumerable<Type> types, int maxLineCount, AdvancedDropdownState state) :
                base(state)
            {
                SetTypes(types);
                minimumSize = new Vector2(minimumSize.x,
                    EditorGUIUtility.singleLineHeight * maxLineCount + KHeaderHeight);
            }

            private void SetTypes(IEnumerable<Type> types)
            {
                _mTypes = types.ToArray();
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
                if (item is AdvancedTypePopupItem typePopupItem)
                {
                    OnItemSelected?.Invoke(typePopupItem);
                }
            }
        }

        #endregion
    }

    #endregion

    #region Static

    public static class ManagedReferenceUtility
    {
        public static object SetManagedReference(this SerializedProperty property, Type type)
        {
            try
            {
                var obj = type != null ? Activator.CreateInstance(type) : null;
                property.managedReferenceValue = obj;
                return obj;
            }
            catch (MissingMethodException)
            {
                var obj = type != null ? FormatterServices.GetUninitializedObject(type) : null;
                property.managedReferenceValue = obj;
                return obj;
            }
        }

        public static Type GetType(string typeName)
        {
            var splitIndex = typeName.IndexOf(' ');
            var assembly = Assembly.Load(typeName[..splitIndex]);
            return assembly.GetType(typeName[(splitIndex + 1)..]);
        }
    }

    public static class TypeMenuUtility
    {
        public const string KNullDisplayName = "<null>";

        public static AddSubclassMenuAttribute GetAttribute(Type type)
        {
            return Attribute.GetCustomAttribute(type, typeof(AddSubclassMenuAttribute)) as AddSubclassMenuAttribute;
        }

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

        public static IEnumerable<Type> OrderByType(this IEnumerable<Type> source)
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
    }

    #endregion
}