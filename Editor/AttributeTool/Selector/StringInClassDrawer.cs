#nullable enable
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace YuzeToolkit.AttributeTool.Editor
{
    [CustomPropertyDrawer(typeof(StringInClassAttribute))]
    public class StringInClassDrawer : PropertyDrawer
    {
        private bool _stringIsInClass = true;
        private bool _meetMatchRule = true;
        private bool _openMatchRule;
        private string? _matchRule;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = EditorGUI.GetPropertyHeight(property);
            var ret = height;
            if (property.propertyType != SerializedPropertyType.String)
            {
                return ret;
            }

            if (!_meetMatchRule)
            {
                ret += height;
            }

            if (_openMatchRule)
            {
                ret += height;
            }

            return ret;
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);
            // 判断是否为string类型, 不是直接报错
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorHelper.DrawWarningMessage(rect, property.displayName + "(错误的特性使用!)");
                return;
            }

            var stringInClassAttribute = (StringInClassAttribute)attribute;
            var height = EditorGUI.GetPropertyHeight(property);

            DrawMatchRule(ref rect, stringInClassAttribute, height);

            DrawStringField(ref rect, property, stringInClassAttribute, height);

            DrawStringWarnBox(ref rect, height);
            EditorGUI.EndProperty();
        }

        /// <summary>
        /// 绘制提示文本, 提示字符串可能存在的错误
        /// </summary>
        private void DrawStringWarnBox(ref Rect rect, float height)
        {
            if (_meetMatchRule) return;

            // 绘制提示框, 更加不同情况展示不同文本
            EditorHelper.DrawWarningMessage(new Rect(rect.position, new Vector2(rect.width, height)),
                _stringIsInClass ? "↑ String is not meeting match rule!" : "↑ String is not in class!");

            // 重新设置矩形大小
            rect = new Rect(new Vector2(rect.x, rect.y + height),
                new Vector2(rect.width, rect.height - height));
        }

        /// <summary>
        /// 绘制MatchRule字符串框体, 可以输入和选择MatchRule
        /// </summary>
        private void DrawMatchRule(ref Rect rect, StringInClassAttribute stringInClassAttribute, float height)
        {
            // 没有开启MatchRule, 直接返回
            if (!_openMatchRule)
            {
                _matchRule = "";
                return;
            }

            // 绘制字MatchRule符串显示框体
            stringInClassAttribute.MatchRule = EditorGUI.TextField(
                new Rect(rect.position, new Vector2(rect.width - height * 3, height)), new GUIContent("MatchRule: "),
                stringInClassAttribute.MatchRule);
            // 获取MatchRuleType中的所有字符串
            var (matchListName, matchListValue) = GetStringInClass(stringInClassAttribute.MatchRuleType);
            var matchIndex = matchListValue.IndexOf(stringInClassAttribute.MatchRule);

            // 绘制下拉菜单框体
            matchIndex = EditorGUI.Popup(new Rect(
                    new Vector2(rect.x + rect.width - height * 3, rect.y), new Vector2(height * 3, height)),
                matchIndex, matchListName.ToArray());

            // 设置MatchRule字符串数据
            stringInClassAttribute.MatchRule = matchIndex switch
            {
                0 => "",
                > 0 when matchIndex < matchListValue.Count => matchListValue[matchIndex],
                _ => stringInClassAttribute.MatchRule
            };

            _matchRule = stringInClassAttribute.MatchRule;

            // 重新设置矩形大小
            rect = new Rect(new Vector2(rect.x, rect.y + height), new Vector2(rect.width, rect.height - height));
        }


        /// <summary>
        /// 绘制字符串输入框体
        /// </summary>
        private void DrawStringField(ref Rect rect, SerializedProperty property,
            StringInClassAttribute stringInClassAttribute, float height)
        {
            // 绘制MatchRule开关
            if (GUI.Button(new Rect(rect.position, new Vector2(height, height)), "✓"))
            {
                _openMatchRule = !_openMatchRule;
            }

            // 绘制不同的PropertyField
            if (stringInClassAttribute.HasLabel)
            {
                property.stringValue = EditorGUI.TextField(new Rect(
                        new Vector2(rect.x + height, rect.y),
                        new Vector2(rect.width - 4 * height, height)),
                    new GUIContent(property.displayName), property.stringValue);
            }
            else
            {
                property.stringValue = EditorGUI.TextField(new Rect(
                        new Vector2(rect.x + height, rect.y),
                        new Vector2(rect.width - 4 * height, height)),
                    property.stringValue);
            }

            List<string> listName;
            List<string> listValue;

            // 判断是否在整个Class中
            (_, listValue) = GetStringInClass(stringInClassAttribute.TargetType);
            var index = listValue.IndexOf(property.stringValue);
            _stringIsInClass = index >= 0;

            // 判断是否为MatchRule所需要的
            (listName, listValue) = GetStringInClass(stringInClassAttribute.TargetType, _matchRule ?? "");
            index = listValue.IndexOf(property.stringValue);
            _meetMatchRule = index >= 0;

            // 绘制下拉菜单
            index = EditorGUI.Popup(
                new Rect(new Vector2(rect.x + rect.width - height * 3, rect.y), new Vector2(height * 3, height)),
                index, stringInClassAttribute.UseValueToName ? listValue.ToArray() : listName.ToArray());

            // 绑定数据
            property.stringValue = index switch
            {
                0 => "",
                > 0 when index < listValue.Count => listValue[index],
                _ => property.stringValue
            };

            // 重新设置矩形大小
            rect = new Rect(new Vector2(rect.x, rect.y + height),
                new Vector2(rect.width, rect.height - height));
        }

        /// <summary>
        /// 获取对于的<see cref="string"/>列表, 默认返回列表包含一个空值
        /// </summary>
        private static (List<string> listName, List<string> listValue) GetStringInClass(Type? targetType,
            string matchRule = "")
        {
            var listName = new List<string> { "<Empty>" };
            var listValue = new List<string> { "<Empty>" };

            if (targetType == null) return (listName, listValue);

            var fields = targetType.GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (var field in fields)
            {
                // 获取name和value
                var name = field.Name;
                var value = GetStingValue(field);

                // 如果值为空, 或者不满足筛选条件则不添加进列表中
                if (string.IsNullOrWhiteSpace(value) || !Regex.IsMatch(value, matchRule)) continue;
                listName.Add(name);
                listValue.Add(value);
            }

            return (listName, listValue);

            static string GetStingValue(FieldInfo fieldInfo)
            {
                var value = fieldInfo.GetValue(null);
                if (value is string s)
                {
                    return s;
                }

                return "";
            }
        }
    }
}