using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace YuzeToolkit.AttributeTool
{
    public static partial class Helper
    {
        public static IEnumerable<MethodInfo> GetAllMethods(this Type? type, string name) =>
            GetAllMethods(type, info => info.Name.Equals(name, StringComparison.Ordinal));

        public static IEnumerable<FieldInfo> GetAllFields(this Type? type, string name) =>
            GetAllFields(type, info => info.Name.Equals(name, StringComparison.Ordinal));

        public static IEnumerable<PropertyInfo> GetAllProperties(this Type? type, string name) =>
            GetAllProperties(type, info => info.Name.Equals(name, StringComparison.Ordinal));

        public static IEnumerable<MethodInfo> GetAllMethods(this Type? type, Func<MethodInfo, bool>? predicate = null)
        {
            if (type == null) yield break;
            var methodNames = new List<string>();
            var methodInfos = type
                .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(info => predicate == null || predicate.Invoke(info));
            foreach (var methodInfo in methodInfos)
            {
                methodNames.Add(methodInfo.Name);
                yield return methodInfo;
            }

            while (type.BaseType != null)
            {
                type = type.BaseType;
                methodInfos = type
                    .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                    .Where(info => predicate == null || predicate.Invoke(info));
                foreach (var methodInfo in methodInfos)
                {
                    if (methodNames.Contains(methodInfo.Name)) continue;
                    yield return methodInfo;
                }
            }
        }

        public static IEnumerable<FieldInfo> GetAllFields(this Type? type, Func<FieldInfo, bool>? predicate = null)
        {
            if (type == null) yield break;
            var fieldNames = new List<string>();
            var fieldInfos = type
                .GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(info => predicate == null || predicate.Invoke(info));
            foreach (var fieldInfo in fieldInfos)
            {
                fieldNames.Add(fieldInfo.Name);
                yield return fieldInfo;
            }

            while (type.BaseType != null)
            {
                type = type.BaseType;
                fieldInfos = type
                    .GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                    .Where(info => predicate == null || predicate.Invoke(info));
                foreach (var fieldInfo in fieldInfos)
                {
                    if (fieldNames.Contains(fieldInfo.Name)) continue;
                    yield return fieldInfo;
                }
            }
        }

        public static IEnumerable<PropertyInfo> GetAllProperties(this Type? type,
            Func<PropertyInfo, bool>? predicate = null)
        {
            if (type == null) yield break;
            var propertyNames = new List<string>();
            var propertyInfos = type
                .GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic |
                               BindingFlags.Public)
                .Where(info => predicate == null || predicate.Invoke(info));
            foreach (var propertyInfo in propertyInfos)
            {
                propertyNames.Add(propertyInfo.Name);
                yield return propertyInfo;
            }

            while (type.BaseType != null)
            {
                type = type.BaseType;
                propertyInfos = type
                    .GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                    .Where(info => predicate == null || predicate.Invoke(info));
                foreach (var propertyInfo in propertyInfos)
                {
                    if (propertyNames.Contains(propertyInfo.Name)) continue;
                    yield return propertyInfo;
                }
            }
        }

        public static bool TryGetValue<T>(this object source, string valueName, out T value)
        {
            if (GetValue(source, valueName) is T t)
            {
                value = t;
                return true;
            }

            value = default!;
            return false;
        }

        public static T? GetValue<T>(this object? source, string? valueName) =>
            source.GetValue(valueName) is T t ? t : default;

        public static object? GetValue(this object? source, string? valueName)
        {
            if (source == null) return null;
            if (string.IsNullOrWhiteSpace(valueName)) return null;
            
            var type = source.GetType();

            var fieldInfo = GetAllFields(type, valueName).FirstOrDefault();
            if (fieldInfo != null) return fieldInfo.GetValue(source);

            var propertyInfo = GetAllProperties(type, valueName).FirstOrDefault();
            if (propertyInfo != null && propertyInfo.CanRead) return propertyInfo.GetValue(source);

            var methodInfos = GetAllMethods(type, valueName);
            return (from methodInfo in methodInfos
                where methodInfo != null && methodInfo.ReturnType != typeof(void) &&
                      methodInfo.GetParameters().Length == 0
                select methodInfo.Invoke(source, null)).FirstOrDefault();
        }
        
        public static bool SetValue(this object? source, string? valueName, object? value)
        {
            if (source == null) return false;
            if (string.IsNullOrWhiteSpace(valueName)) return false;
            
            var type = source.GetType();
            
            var fieldInfo = GetAllFields(type, valueName).FirstOrDefault();
            if (fieldInfo != null && fieldInfo.FieldType.IsInstanceOfType(value))
            {
                fieldInfo.SetValue(source, value);
                return true;
            }

            var propertyInfo = GetAllProperties(type, valueName).FirstOrDefault();
            if (propertyInfo != null && propertyInfo.CanWrite && propertyInfo.PropertyType.IsInstanceOfType(value))
            {
                propertyInfo.SetValue(source, value);
                return true;
            }

            var methodInfos = GetAllMethods(type, valueName);
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

    public enum InfoType
    {
        Normal,
        Warning,
        Error
    }
}