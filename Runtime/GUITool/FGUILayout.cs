#nullable enable
using System;
using UnityEngine;

namespace YuzeToolkit.GUITool
{
    public static class FGUILayout
    {
        public readonly struct End : IDisposable
        {
            private readonly Action _end;
            public End(Action end) => _end = end;
            public void Dispose() => _end.Invoke();
        }

        #region Horizontal

        public static End Horizontal(params GUILayoutOption[]? options) =>
            Horizontal(GUIContent.none, GUIStyle.none, options);

        public static End Horizontal(string? text, params GUILayoutOption[]? options) =>
            Horizontal(new GUIContent(text), GUIStyle.none, options);

        public static End Horizontal(Texture? image, params GUILayoutOption[]? options) =>
            Horizontal(new GUIContent(image), GUIStyle.none, options);

        public static End Horizontal(GUIContent? content, params GUILayoutOption[]? options) =>
            Horizontal(content, GUIStyle.none, options);

        public static End Horizontal(GUIStyle? style, params GUILayoutOption[]? options) =>
            Horizontal(GUIContent.none, style, options);

        public static End Horizontal(string? text, GUIStyle? style, params GUILayoutOption[]? options) =>
            Horizontal(new GUIContent(text), style, options);

        public static End Horizontal(Texture? image, GUIStyle? style, params GUILayoutOption[]? options) =>
            Horizontal(new GUIContent(image), style, options);

        public static End Horizontal(GUIContent? content, GUIStyle? style, params GUILayoutOption[]? options)
        {
            GUILayout.BeginHorizontal(content, style, options);
            return new End(EndHorizontal);
        }

        private static readonly Action EndHorizontal = GUILayout.EndHorizontal;

        #endregion

        #region Vertical
        
        public static End Vertical(params GUILayoutOption[]? options) =>
            Vertical(GUIContent.none, GUIStyle.none, options);

        public static End Vertical(string? text, params GUILayoutOption[]? options) =>
            Vertical(new GUIContent(text), GUIStyle.none, options);

        public static End Vertical(Texture? image, params GUILayoutOption[]? options) =>
            Vertical(new GUIContent(image), GUIStyle.none, options);

        public static End Vertical(GUIContent? content, params GUILayoutOption[]? options) =>
            Vertical(content, GUIStyle.none, options);

        public static End Vertical(GUIStyle? style, params GUILayoutOption[]? options) =>
            Vertical(GUIContent.none, style, options);

        public static End Vertical(string? text, GUIStyle? style, params GUILayoutOption[]? options) =>
            Vertical(new GUIContent(text), style, options);

        public static End Vertical(Texture? image, GUIStyle? style, params GUILayoutOption[]? options) =>
            Vertical(new GUIContent(image), style, options);

        public static End Vertical(GUIContent? content, GUIStyle? style, params GUILayoutOption[]? options)
        {
            GUILayout.BeginVertical(content, style, options);
            return new End(EndVertical);
        }

        private static readonly Action EndVertical = GUILayout.EndVertical;

        #endregion

        #region Area

        public static End Area(Rect screenRect) =>
            Area(screenRect, GUIContent.none, GUIStyle.none);

        public static End Area(Rect screenRect, string? text) =>
            Area(screenRect, new GUIContent(text), GUIStyle.none);

        public static End Area(Rect screenRect, Texture? image) =>
            Area(screenRect, new GUIContent(image), GUIStyle.none);

        public static End Area(Rect screenRect, GUIContent? content) =>
            Area(screenRect, content, GUIStyle.none);

        public static End Area(Rect screenRect, GUIStyle? style) =>
            Area(screenRect, GUIContent.none, style);

        public static End Area(Rect screenRect, string? text, GUIStyle? style) =>
            Area(screenRect, new GUIContent(text), style);

        public static End Area(Rect screenRect, Texture? image, GUIStyle? style) =>
            Area(screenRect, new GUIContent(image), style);

        public static End Area(Rect screenRect, GUIContent? content, GUIStyle? style)
        {
            GUILayout.BeginArea(screenRect, content, style);
            return new End(EndArea);
        }

        private static readonly Action EndArea = GUILayout.EndArea;

        #endregion

        #region ScrollView

        public static End ScrollView(Vector2 scrollPosition, out Vector2 newPosition,
            params GUILayoutOption[] options) => ScrollView(scrollPosition, false, false,
            GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar, GUI.skin.scrollView, out newPosition,
            options);

        public static End ScrollView(Vector2 scrollPosition, bool alwaysShowHorizontal,
            bool alwaysShowVertical, out Vector2 newPosition, params GUILayoutOption[]? options) => ScrollView(
            scrollPosition, alwaysShowHorizontal, alwaysShowVertical, GUI.skin.horizontalScrollbar,
            GUI.skin.verticalScrollbar, GUI.skin.scrollView, out newPosition, options);

        public static End ScrollView(Vector2 scrollPosition, GUIStyle? horizontalScrollbar,
            GUIStyle? verticalScrollbar, out Vector2 newPosition, params GUILayoutOption[]? options) =>
            ScrollView(scrollPosition, false, false, horizontalScrollbar,
                verticalScrollbar, GUI.skin.scrollView, out newPosition, options);

        public static End ScrollView(Vector2 scrollPosition, GUIStyle? background, out Vector2 newPosition,
            params GUILayoutOption[]? options) => ScrollView(scrollPosition, false, false,
            GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar, background, out newPosition, options);

        public static End ScrollView(Vector2 scrollPosition,
            bool alwaysShowHorizontal, bool alwaysShowVertical,
            GUIStyle? horizontalScrollbar, GUIStyle? verticalScrollbar,
            GUIStyle? background, out Vector2 newPosition, params GUILayoutOption[]? options)
        {
            newPosition = GUILayout.BeginScrollView(scrollPosition, alwaysShowHorizontal, alwaysShowVertical,
                horizontalScrollbar, verticalScrollbar, background, options);
            return new End(EndScrollView);
        }

        private static readonly Action EndScrollView = GUILayout.EndScrollView;

        #endregion

        #region Button

        public static void Button(Action? action, params GUILayoutOption[]? options) =>
            Button(action, new GUIContent("Button"), GUI.skin.button, options);

        public static void Button(Action? action, Texture? image, params GUILayoutOption[]? options) =>
            Button(action, new GUIContent(image), GUI.skin.button, options);

        public static void Button(Action? action, string? text, params GUILayoutOption[]? options) =>
            Button(action, new GUIContent(text), GUI.skin.button, options);

        public static void Button(Action? action, GUIContent? content, params GUILayoutOption[]? options) =>
            Button(action, content, GUI.skin.button, options);

        public static void Button(Action? action, Texture? image, GUIStyle? style, params GUILayoutOption[]? options) =>
            Button(action, new GUIContent(image), style, options);

        public static void Button(Action? action, string? text, GUIStyle? style, params GUILayoutOption[]? options) =>
            Button(action, new GUIContent(text), style, options);

        public static void Button(Action? action, GUIContent? content, GUIStyle? style,
            params GUILayoutOption[]? options)
        {
            if (GUILayout.Button(content, style, options)) action?.Invoke();
        }

        public static T? Button<T>(Func<T>? func, params GUILayoutOption[]? options) =>
            Button(func, new GUIContent("Button"), GUI.skin.button, options);

        public static T? Button<T>(Func<T>? func, Texture? image, params GUILayoutOption[]? options) =>
            Button(func, new GUIContent(image), GUI.skin.button, options);

        public static T? Button<T>(Func<T>? func, string? text, params GUILayoutOption[]? options) =>
            Button(func, new GUIContent(text), GUI.skin.button, options);

        public static T? Button<T>(Func<T>? func, GUIContent? content, params GUILayoutOption[]? options) =>
            Button(func, content, GUI.skin.button, options);

        public static T? Button<T>(Func<T>? func, Texture? image, GUIStyle? style, params GUILayoutOption[]? options) =>
            Button(func, new GUIContent(image), style, options);

        public static T? Button<T>(Func<T>? func, string? text, GUIStyle? style, params GUILayoutOption[]? options) =>
            Button(func, new GUIContent(text), style, options);

        public static T? Button<T>(Func<T>? func, GUIContent? content, GUIStyle? style,
            params GUILayoutOption[]? options) =>
            GUILayout.Button(content, style, options) && func != null ? func.Invoke() : default;

        #endregion

        #region RepeatButton

        public static void RepeatButton(Action? action, params GUILayoutOption[]? options) =>
            RepeatButton(action, new GUIContent("Button"), GUI.skin.button, options);

        public static void RepeatButton(Action? action, Texture? image, params GUILayoutOption[]? options) =>
            RepeatButton(action, new GUIContent(image), GUI.skin.button, options);

        public static void RepeatButton(Action? action, string? text, params GUILayoutOption[]? options) =>
            RepeatButton(action, new GUIContent(text), GUI.skin.button, options);

        public static void RepeatButton(Action? action, GUIContent? content, params GUILayoutOption[]? options) =>
            RepeatButton(action, content, GUI.skin.button, options);

        public static void RepeatButton(Action? action, Texture? image, GUIStyle? style,
            params GUILayoutOption[]? options) =>
            RepeatButton(action, new GUIContent(image), style, options);

        public static void RepeatButton(Action? action, string? text, GUIStyle? style,
            params GUILayoutOption[]? options) =>
            RepeatButton(action, new GUIContent(text), style, options);

        public static void RepeatButton(Action? action, GUIContent? content, GUIStyle? style,
            params GUILayoutOption[]? options)
        {
            if (GUILayout.RepeatButton(content, style, options)) action?.Invoke();
        }

        public static T? RepeatButton<T>(Func<T>? func, params GUILayoutOption[]? options) =>
            RepeatButton(func, new GUIContent("Button"), GUI.skin.button, options);

        public static T? RepeatButton<T>(Func<T>? func, Texture? image, params GUILayoutOption[]? options) =>
            RepeatButton(func, new GUIContent(image), GUI.skin.button, options);

        public static T? RepeatButton<T>(Func<T>? func, string? text, params GUILayoutOption[]? options) =>
            RepeatButton(func, new GUIContent(text), GUI.skin.button, options);

        public static T? RepeatButton<T>(Func<T>? func, GUIContent? content, params GUILayoutOption[]? options) =>
            RepeatButton(func, content, GUI.skin.button, options);

        public static T? RepeatButton<T>(Func<T>? func, Texture? image, GUIStyle? style,
            params GUILayoutOption[]? options) =>
            RepeatButton(func, new GUIContent(image), style, options);

        public static T? RepeatButton<T>(Func<T>? func, string? text, GUIStyle? style,
            params GUILayoutOption[]? options) =>
            RepeatButton(func, new GUIContent(text), style, options);

        public static T? RepeatButton<T>(Func<T>? func, GUIContent? content, GUIStyle? style,
            params GUILayoutOption[]? options) =>
            GUILayout.RepeatButton(content, style, options) && func != null ? func.Invoke() : default;

        #endregion

        #region Field

        public enum EStringFieldType
        {
            Field,
            Password,
            TextArea
        }

        public static string? StringField(string? value,
            EStringFieldType type = EStringFieldType.Field, params GUILayoutOption[]? options) =>
            StringField(value, new GUIContent(), GUI.skin.textField, type, options);

        public static string? StringField(string? value, Texture? image,
            EStringFieldType type = EStringFieldType.Field, params GUILayoutOption[]? options) =>
            StringField(value, new GUIContent(image), GUI.skin.textField, type, options);

        public static string? StringField(string? value, string? text,
            EStringFieldType type = EStringFieldType.Field, params GUILayoutOption[]? options) =>
            StringField(value, new GUIContent(text), GUI.skin.textField, type, options);

        public static string? StringField(string? value, GUIContent? content,
            EStringFieldType type = EStringFieldType.Field, params GUILayoutOption[]? options) =>
            StringField(value, content, GUI.skin.textField, type, options);

        public static string? StringField(string? value, Texture? image, GUIStyle? style,
            EStringFieldType type = EStringFieldType.Field, params GUILayoutOption[]? options) =>
            StringField(value, new GUIContent(image), style, type, options);

        public static string? StringField(string? value, string? text, GUIStyle? style,
            EStringFieldType type = EStringFieldType.Field, params GUILayoutOption[]? options) =>
            StringField(value, new GUIContent(text), style, type, options);

        public static string? StringField(string? value, GUIContent? content, GUIStyle? style,
            EStringFieldType type = EStringFieldType.Field, params GUILayoutOption[]? options)
        {
            switch (type)
            {
                case EStringFieldType.Field:
                    using (Horizontal())
                    {
                        content.DrawLabel();
                        return GUILayout.TextField(value, style, options);
                    }
                case EStringFieldType.Password:
                    using (Horizontal())
                    {
                        content.DrawLabel();
                        return GUILayout.PasswordField(value, '*', style, options);
                    }
                case EStringFieldType.TextArea:
                    using (Horizontal())
                    {
                        content.DrawLabel();
                        return GUILayout.TextArea(value, style, options);
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public static void Field<T>(Func<T?> get, Action<T?> set, params GUILayoutOption[]? options) =>
            set(Field(get(), new GUIContent(), GUI.skin.textField, options));

        public static void Field<T>(Func<T?> get, Action<T?> set, string? text, params GUILayoutOption[]? options) =>
            set(Field(get(), new GUIContent(text), GUI.skin.textField, options));

        public static void Field<T>(Func<T?> get, Action<T?> set, Texture? image, params GUILayoutOption[]? options) =>
            set(Field(get(), new GUIContent(image), GUI.skin.textField, options));

        public static void Field<T>(Func<T?> get, Action<T?> set, GUIContent? content,
            params GUILayoutOption[]? options) =>
            set(Field(get(), content, GUI.skin.textField, options));

        public static void Field<T>(Func<T?> get, Action<T?> set, string? text, GUIStyle? style,
            params GUILayoutOption[]? options) =>
            set(Field(get(), new GUIContent(text), style, options));

        public static void Field<T>(Func<T?> get, Action<T?> set, Texture? image, GUIStyle? style,
            params GUILayoutOption[]? options) =>
            set(Field(get(), new GUIContent(image), style, options));

        public static void Field<T>(Func<T?> get, Action<T?> set, GUIContent? content, GUIStyle? style,
            params GUILayoutOption[]? options) => set(Field(get(), content, style, options));

        public static void Field<T>(ref T? value, params GUILayoutOption[]? options) =>
            value = Field(value, new GUIContent(), GUI.skin.textField, options);

        public static void Field<T>(ref T? value, string? text, params GUILayoutOption[]? options) =>
            value = Field(value, new GUIContent(text), GUI.skin.textField, options);

        public static void Field<T>(ref T? value, Texture? image, params GUILayoutOption[]? options) =>
            value = Field(value, new GUIContent(image), GUI.skin.textField, options);

        public static void Field<T>(ref T? value, GUIContent? content, params GUILayoutOption[]? options) =>
            value = Field(value, content, GUI.skin.textField, options);

        public static void Field<T>(ref T? value, string? text, GUIStyle? style, params GUILayoutOption[]? options) =>
            value = Field(value, new GUIContent(text), style, options);

        public static void Field<T>(ref T? value, Texture? image, GUIStyle? style, params GUILayoutOption[]? options) =>
            value = Field(value, new GUIContent(image), style, options);

        public static void Field<T>(ref T? value, GUIContent? content, GUIStyle? style,
            params GUILayoutOption[]? options) => value = Field(value, content, style, options);

        public static T? Field<T>(T? value, params GUILayoutOption[]? options) =>
            Field(value, new GUIContent(), GUI.skin.textField, options);

        public static T? Field<T>(T? value, string? text, params GUILayoutOption[]? options) =>
            Field(value, new GUIContent(text), GUI.skin.textField, options);

        public static T? Field<T>(T? value, Texture? image, params GUILayoutOption[]? options) =>
            Field(value, new GUIContent(image), GUI.skin.textField, options);

        public static T? Field<T>(T? value, GUIContent? content, params GUILayoutOption[]? options) =>
            Field(value, content, GUI.skin.textField, options);

        public static T? Field<T>(T? value, string? text, GUIStyle? style, params GUILayoutOption[]? options) =>
            Field(value, new GUIContent(text), style, options);

        public static T? Field<T>(T? value, Texture? image, GUIStyle? style, params GUILayoutOption[]? options) =>
            Field(value, new GUIContent(image), style, options);

        public static T? Field<T>(T? value, GUIContent? content, GUIStyle? style, params GUILayoutOption[]? options)
        {
            return value switch
            {
                bool newValue => BoolField(newValue, content, style, options) is T t ? t : value,
                string newValue => StringField(newValue, content, style, options: options) is T t ? t : value,
                sbyte newValue => TypeField(newValue, content, style, str =>
                    sbyte.TryParse(str, out var i) ? (true, i) : (false, default), options) is T t
                    ? t
                    : value,
                byte newValue => TypeField(newValue, content, style, str =>
                    byte.TryParse(str, out var i) ? (true, i) : (false, default), options) is T t
                    ? t
                    : value,
                short newValue => TypeField(newValue, content, style, str =>
                    short.TryParse(str, out var i) ? (true, i) : (false, default), options) is T t
                    ? t
                    : value,
                int newValue => TypeField(newValue, content, style, str =>
                    int.TryParse(str, out var i) ? (true, i) : (false, default), options) is T t
                    ? t
                    : value,
                long newValue => TypeField(newValue, content, style, str =>
                    long.TryParse(str, out var i) ? (true, i) : (false, default), options) is T t
                    ? t
                    : value,
                float newValue => TypeField(newValue, content, style, str =>
                    float.TryParse(str, out var i) ? (true, i) : (false, default), options) is T t
                    ? t
                    : value,
                double newValue => TypeField(newValue, content, style, str =>
                    double.TryParse(str, out var i) ? (true, i) : (false, default), options) is T t
                    ? t
                    : value,
                ushort newValue => TypeField(newValue, content, style, str =>
                    ushort.TryParse(str, out var i) ? (true, i) : (false, default), options) is T t
                    ? t
                    : value,
                uint newValue => TypeField(newValue, content, style, str =>
                    uint.TryParse(str, out var i) ? (true, i) : (false, default), options) is T t
                    ? t
                    : value,
                ulong newValue => TypeField(newValue, content, style, str =>
                    ulong.TryParse(str, out var i) ? (true, i) : (false, default), options) is T t
                    ? t
                    : value,
                Vector2 newValue => Vector2Field(newValue, content, style, options) is T t ? t : value,
                Vector3 newValue => Vector3Field(newValue, content, style, options) is T t ? t : value,
                Vector4 newValue => Vector4Field(newValue, content, style, options) is T t ? t : value,
                Vector2Int newValue => Vector2IntField(newValue, content, style, options) is T t ? t : value,
                Vector3Int newValue => Vector3IntField(newValue, content, style, options) is T t ? t : value,
                Quaternion newValue => QuaternionField(newValue, content, style, options) is T t ? t : value,
                _ => CantDrawField(value, content, style, options)
            };
        }

        private static T? TypeField<T>(T? value, GUIContent? content, GUIStyle? style, Func<string, (bool, T)> tryParse,
            params GUILayoutOption[]? options)
        {
            using (Horizontal())
            {
                content.DrawLabel();
                var str = GUILayout.TextField($"{value}", style, options);
                var (canParse, i) = tryParse.Invoke(str);
                if (canParse) return i;
                return string.IsNullOrWhiteSpace(str) ? default : value;
            }
        }

        private static T? CantDrawField<T>(T? value, GUIContent? content, GUIStyle? style,
            params GUILayoutOption[]? options)
        {
            using (Horizontal())
            {
                content.DrawLabel();
                GUILayout.Label($"无法显示{typeof(T).Name}类型!", style, options);
            }

            return value;
        }

        private static bool BoolField(bool value, GUIContent? content, GUIStyle? style,
            params GUILayoutOption[]? options)
        {
            using (Horizontal())
            {
                if (content.DrawLabel()) GUILayout.Space(20);
                return GUILayout.Toggle(value, new GUIContent(), style == GUI.skin.textField ? GUI.skin.toggle : style,
                    options);
            }
        }

        private static Vector2 Vector2Field(Vector2 value, GUIContent? content, GUIStyle? style,
            params GUILayoutOption[]? options)
        {
            using (Horizontal())
            {
                if (content.DrawLabel()) GUILayout.FlexibleSpace();
                var x = Field(value.x, "x:", style, options);
                var y = Field(value.y, "y:", style, options);
                return new Vector2(x, y);
            }
        }

        private static Vector3 Vector3Field(Vector3 value, GUIContent? content, GUIStyle? style,
            params GUILayoutOption[]? options)
        {
            using (Horizontal())
            {
                if (content.DrawLabel()) GUILayout.FlexibleSpace();
                var x = Field(value.x, "x:", style, options);
                var y = Field(value.y, "y:", style, options);
                var z = Field(value.z, "z:", style, options);
                return new Vector3(x, y, z);
            }
        }

        private static Vector4 Vector4Field(Vector4 value, GUIContent? content, GUIStyle? style,
            params GUILayoutOption[]? options)
        {
            using (Horizontal())
            {
                if (content.DrawLabel()) GUILayout.FlexibleSpace();
                var x = Field(value.x, "x:", style, options);
                var y = Field(value.y, "y:", style, options);
                var z = Field(value.z, "z:", style, options);
                var w = Field(value.w, "w:", style, options);
                return new Vector4(x, y, z, w);
            }
        }

        private static Vector2Int Vector2IntField(Vector2Int value, GUIContent? content, GUIStyle? style,
            params GUILayoutOption[]? options)
        {
            using (Horizontal())
            {
                if (content.DrawLabel()) GUILayout.FlexibleSpace();
                var x = Field(value.x, "x:", style, options);
                var y = Field(value.y, "y:", style, options);
                return new Vector2Int(x, y);
            }
        }

        private static Vector3Int Vector3IntField(Vector3Int value, GUIContent? content, GUIStyle? style,
            params GUILayoutOption[]? options)
        {
            using (Horizontal())
            {
                if (content.DrawLabel()) GUILayout.FlexibleSpace();
                var x = Field(value.x, "x:", style, options);
                var y = Field(value.y, "y:", style, options);
                var z = Field(value.z, "z:", style, options);
                return new Vector3Int(x, y, z);
            }
        }

        private static Quaternion QuaternionField(Quaternion value, GUIContent? content, GUIStyle? style,
            params GUILayoutOption[]? options)
        {
            using (Horizontal())
            {
                if (content.DrawLabel()) GUILayout.FlexibleSpace();
                var x = Field(value.x, "x:", style, options);
                var y = Field(value.y, "y:", style, options);
                var z = Field(value.z, "z:", style, options);
                var w = Field(value.w, "w:", style, options);
                return new Quaternion(x, y, z, w);
            }
        }

        #endregion

        private static bool DrawLabel(this GUIContent? content)
        {
            if (content == null || (content.image == null && string.IsNullOrWhiteSpace(content.text))) return false;
            GUILayout.Label(content);
            return true;
        }
    }
}