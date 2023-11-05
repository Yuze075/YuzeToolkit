using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace YuzeToolkit.AttributeTool
{
    public static partial class Helper
    {
        public static Color GetColor(this ColorType colorType)
        {
            return colorType switch
            {
                ColorType.Clear => new Color32(0, 0, 0, 0),
                ColorType.White => new Color32(255, 255, 255, 255),
                ColorType.Black => new Color32(0, 0, 0, 255),
                ColorType.Gray => new Color32(128, 128, 128, 255),
                ColorType.Red => new Color32(255, 0, 63, 255),
                ColorType.Pink => new Color32(255, 152, 203, 255),
                ColorType.Orange => new Color32(255, 128, 0, 255),
                ColorType.Yellow => new Color32(255, 211, 0, 255),
                ColorType.Green => new Color32(98, 200, 79, 255),
                ColorType.Blue => new Color32(0, 135, 189, 255),
                ColorType.Indigo => new Color32(75, 0, 130, 255),
                ColorType.Violet => new Color32(128, 0, 255, 255),
                _ => new Color32(0, 0, 0, 255)
            };
        }

        public static IEnumerable<MethodInfo> GetAllMethods(this Type type, string name)
        {
            return GetAllMethods(type, info => info.Name.Equals(name, StringComparison.Ordinal));
        }

        public static IEnumerable<FieldInfo> GetAllFields(this Type type, string name)
        {
            return GetAllFields(type, info => info.Name.Equals(name, StringComparison.Ordinal));
        }

        public static IEnumerable<PropertyInfo> GetAllProperties(this Type type, string name)
        {
            return GetAllProperties(type, info => info.Name.Equals(name, StringComparison.Ordinal));
        }

        public static IEnumerable<MethodInfo> GetAllMethods(this Type type, Func<MethodInfo, bool> predicate = null)
        {
            if (type == null)
            {
                yield break;
            }

            var methodInfos = type
                .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(info => predicate == null || predicate.Invoke(info));
            foreach (var methodInfo in methodInfos)
            {
                yield return methodInfo;
            }
        }

        public static IEnumerable<FieldInfo> GetAllFields(this Type type, Func<FieldInfo, bool> predicate = null)
        {
            if (type == null)
            {
                yield break;
            }

            var fieldInfos = type
                .GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(info => predicate == null || predicate.Invoke(info));
            foreach (var fieldInfo in fieldInfos)
            {
                yield return fieldInfo;
            }
        }

        public static IEnumerable<PropertyInfo> GetAllProperties(this Type type,
            Func<PropertyInfo, bool> predicate = null)
        {
            if (type == null)
            {
                yield break;
            }

            var propertyInfos = type
                .GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic |
                               BindingFlags.Public)
                .Where(info => predicate == null || predicate.Invoke(info));
            foreach (var propertyInfo in propertyInfos)
            {
                yield return propertyInfo;
            }
        }

        /// <summary>
        /// 尝试获取source中对应valueName的值, 只查询source这一层, 不会嵌套查询
        /// </summary>
        public static bool TryGetValue<T>(this object source, string valueName, out T value)
        {
            var o = GetValue(source, valueName);
            if (o is T t)
            {
                value = t;
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// 获取source中对应valueName的值, 只查询source这一层, 不会嵌套查询
        /// </summary>
        public static object? GetValue(this object? source, string valueName)
        {
            if (source == null) return null;

            var type = source.GetType();
            var fieldInfo = GetAllFields(type, valueName)
                .FirstOrDefault();
            if (fieldInfo != null)
            {
                return fieldInfo.GetValue(source);
            }

            var propertyInfo = GetAllProperties(type, valueName)
                .FirstOrDefault();
            if (propertyInfo != null)
            {
                return propertyInfo.GetValue(source);
            }

            var methodInfos = GetAllMethods(type, valueName);
            return (from methodInfo in methodInfos
                where methodInfo != null && methodInfo.ReturnType != typeof(void) &&
                      methodInfo.GetParameters().Length == 0
                select methodInfo.Invoke(source, null)).FirstOrDefault();
        }
        public static T? GetValue<T>(this object source, string valueName) => source.GetValue(valueName) is T t ? t : default;

        /// <summary>
        /// 尝试去设置source中对应的valueName的值. 可以通过子类去设置父类的值
        /// </summary>
        public static bool SetValue(this object source, string valueName, object value)
        {
            if (source == null) return false;
            var type = source.GetType();
            var fieldInfo = GetAllFields(type, info => info.Name.Equals(valueName, StringComparison.Ordinal))
                .FirstOrDefault();
            if (fieldInfo != null && fieldInfo.FieldType.IsInstanceOfType(value))
            {
                fieldInfo.SetValue(source, value);
                return true;
            }

            var propertyInfo = GetAllProperties(type, info => info.Name.Equals(valueName, StringComparison.Ordinal))
                .FirstOrDefault();
            if (propertyInfo != null && propertyInfo.CanWrite && propertyInfo.PropertyType.IsInstanceOfType(value))
            {
                propertyInfo.SetValue(source, value);
                return true;
            }

            var methodInfos = GetAllMethods(type, info => info.Name.Equals(valueName, StringComparison.Ordinal));
            foreach (var methodInfo in methodInfos)
            {
                if (methodInfo == null || methodInfo.GetParameters().Length != 1 ||
                    !methodInfo.GetParameters()[0].ParameterType.IsInstanceOfType(value)) continue;
                methodInfo.Invoke(source, new[] { value });
                return true;
            }

            return false;
        }
    }

    public enum ColorType
    {
        Clear,
        White,
        Black,
        Gray,
        Red,
        Pink,
        Orange,
        Yellow,
        Green,
        Blue,
        Indigo,
        Violet
    }

    public enum InfoType
    {
        Normal,
        Warning,
        Error
    }
}