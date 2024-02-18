#nullable enable
using System;
using UnityEngine;

namespace YuzeToolkit
{
    public static class FGUI
    {
        public static Rect ByWidth(this ref Rect rect, float width)
        {
            var returnRect = new Rect(rect.x, rect.y, width, rect.height);
            rect = new Rect(rect.x + width, rect.y, rect.width - width, rect.height);
            return returnRect;
        }

        public static Rect ByHeight(this ref Rect rect, float height)
        {
            var returnRect = new Rect(rect.x, rect.y, rect.width, height);
            rect = new Rect(rect.x, rect.y + height, rect.width, rect.height - height);
            return returnRect;
        }

        public static Rect ByWidth(this ref Rect rect, float width, float height)
        {
            var returnRect = new Rect(rect.x, rect.y, width, height);
            rect = new Rect(rect.x + width, rect.y, rect.width - width, rect.height);
            return returnRect;
        }

        public static Rect ByHeight(this ref Rect rect, float width, float height)
        {
            var returnRect = new Rect(rect.x, rect.y, width, height);
            rect = new Rect(rect.x, rect.y + height, rect.width, rect.height - height);
            return returnRect;
        }
    }

    public static class FGUILayout
    {
        public readonly struct End : IDisposable
        {
            private readonly Action _end;
            public End(Action end) => _end = end;
            public void Dispose() => _end.Invoke();
        }

        #region Horizontal & Vertical

        private static readonly Action EndHorizontal = GUILayout.EndHorizontal;
        private static readonly Action EndVertical = GUILayout.EndVertical;

        public static End Horizontal
        {
            get
            {
                GUILayout.BeginHorizontal();
                return new End(EndHorizontal);
            }
        }


        public static End Vertical
        {
            get
            {
                GUILayout.BeginVertical();
                return new End(EndVertical);
            }
        }

        #endregion

        #region Button

        public static void Button(Action? action, string? text = null) => Button(action, new GUIContent(text));

        public static void Button(Action? action, GUIContent? content)
        {
            if (GUILayout.Button(content)) action?.Invoke();
        }

        public static T? Button<T>(Func<T>? func, string? text = null) =>
            Button(func, new GUIContent(text));

        public static T? Button<T>(Func<T>? func, GUIContent? content) =>
            GUILayout.Button(content) && func != null ? func.Invoke() : default;

        #endregion

        #region Field

        public static void Field<T>(Func<T?> get, Action<T?> set, string? text = null) =>
            set(Field(get(), new GUIContent(text)));

        public static void Field<T>(Func<T?> get, Action<T?> set, GUIContent? content) => set(Field(get(), content));
        public static void Field<T>(ref T? value, string? text = null) => value = Field(value, new GUIContent(text));
        public static void Field<T>(ref T? value, GUIContent? content) => value = Field(value, content);
        public static T? Field<T>(T? value, string? text = null) => Field(value, new GUIContent(text));

        public static T? Field<T>(T? value, GUIContent? content)
        {
            return value switch
            {
                bool newValue => BoolField(newValue, content) is T t ? t : value,
                string newValue => StringField(newValue, content) is T t ? t : value,
                sbyte newValue => TypeField(newValue, content, str =>
                    sbyte.TryParse(str, out var i) ? (true, i) : (false, default)) is T t
                    ? t
                    : value,
                byte newValue => TypeField(newValue, content, str =>
                    byte.TryParse(str, out var i) ? (true, i) : (false, default)) is T t
                    ? t
                    : value,
                short newValue => TypeField(newValue, content, str =>
                    short.TryParse(str, out var i) ? (true, i) : (false, default)) is T t
                    ? t
                    : value,
                int newValue => TypeField(newValue, content, str =>
                    int.TryParse(str, out var i) ? (true, i) : (false, default)) is T t
                    ? t
                    : value,
                long newValue => TypeField(newValue, content, str =>
                    long.TryParse(str, out var i) ? (true, i) : (false, default)) is T t
                    ? t
                    : value,
                float newValue => TypeField(newValue, content, str =>
                    float.TryParse(str, out var i) ? (true, i) : (false, default)) is T t
                    ? t
                    : value,
                double newValue => TypeField(newValue, content, str =>
                    double.TryParse(str, out var i) ? (true, i) : (false, default)) is T t
                    ? t
                    : value,
                ushort newValue => TypeField(newValue, content, str =>
                    ushort.TryParse(str, out var i) ? (true, i) : (false, default)) is T t
                    ? t
                    : value,
                uint newValue => TypeField(newValue, content, str =>
                    uint.TryParse(str, out var i) ? (true, i) : (false, default)) is T t
                    ? t
                    : value,
                ulong newValue => TypeField(newValue, content, str =>
                    ulong.TryParse(str, out var i) ? (true, i) : (false, default)) is T t
                    ? t
                    : value,
                Vector2 newValue => Vector2Field(newValue, content) is T t ? t : value,
                Vector3 newValue => Vector3Field(newValue, content) is T t ? t : value,
                Vector4 newValue => Vector4Field(newValue, content) is T t ? t : value,
                Vector2Int newValue => Vector2IntField(newValue, content) is T t ? t : value,
                Vector3Int newValue => Vector3IntField(newValue, content) is T t ? t : value,
                Quaternion newValue => QuaternionField(newValue, content) is T t ? t : value,
                _ => CantDrawField(value, content)
            };
        }

        #endregion

        #region StringField

        public enum EStringFieldType
        {
            Field,
            Password,
            TextArea
        }

        public static string? StringField(string? value, string? text = null,
            EStringFieldType type = EStringFieldType.Field) =>
            StringField(value, new GUIContent(text));

        public static string? StringField(string? value, GUIContent? content,
            EStringFieldType type = EStringFieldType.Field)
        {
            switch (type)
            {
                case EStringFieldType.Field:
                    using (Horizontal)
                    {
                        content.DrawLabel();
                        return GUILayout.TextField(value);
                    }
                case EStringFieldType.Password:
                    using (Horizontal)
                    {
                        content.DrawLabel();
                        return GUILayout.PasswordField(value, '*');
                    }
                case EStringFieldType.TextArea:
                    using (Horizontal)
                    {
                        content.DrawLabel();
                        return GUILayout.TextArea(value);
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        #endregion

        #region TypeField

        private static T? TypeField<T>(T? value, GUIContent? content, Func<string, (bool, T)> tryParse)
        {
            using (Horizontal)
            {
                content.DrawLabel();
                var str = GUILayout.TextField($"{value}");
                var (canParse, i) = tryParse.Invoke(str);
                if (canParse) return i;
                return string.IsNullOrWhiteSpace(str) ? default : value;
            }
        }

        private static T? CantDrawField<T>(T? value, GUIContent? content)
        {
            using (Horizontal)
            {
                content.DrawLabel();
                GUILayout.Label($"无法显示{typeof(T).Name}类型!");
            }

            return value;
        }

        private static bool BoolField(bool value, GUIContent? content)
        {
            using (Horizontal)
            {
                if (content.DrawLabel()) GUILayout.Space(20);
                return GUILayout.Toggle(value, new GUIContent());
            }
        }

        private static Vector2 Vector2Field(Vector2 value, GUIContent? content)
        {
            using (Horizontal)
            {
                if (content.DrawLabel()) GUILayout.FlexibleSpace();
                var x = Field(value.x, "x:");
                var y = Field(value.y, "y:");
                return new Vector2(x, y);
            }
        }

        private static Vector3 Vector3Field(Vector3 value, GUIContent? content)
        {
            using (Horizontal)
            {
                if (content.DrawLabel()) GUILayout.FlexibleSpace();
                var x = Field(value.x, "x:");
                var y = Field(value.y, "y:");
                var z = Field(value.z, "z:");
                return new Vector3(x, y, z);
            }
        }

        private static Vector4 Vector4Field(Vector4 value, GUIContent? content)
        {
            using (Horizontal)
            {
                if (content.DrawLabel()) GUILayout.FlexibleSpace();
                var x = Field(value.x, "x:");
                var y = Field(value.y, "y:");
                var z = Field(value.z, "z:");
                var w = Field(value.w, "w:");
                return new Vector4(x, y, z, w);
            }
        }

        private static Vector2Int Vector2IntField(Vector2Int value, GUIContent? content)
        {
            using (Horizontal)
            {
                if (content.DrawLabel()) GUILayout.FlexibleSpace();
                var x = Field(value.x, "x:");
                var y = Field(value.y, "y:");
                return new Vector2Int(x, y);
            }
        }

        private static Vector3Int Vector3IntField(Vector3Int value, GUIContent? content)
        {
            using (Horizontal)
            {
                if (content.DrawLabel()) GUILayout.FlexibleSpace();
                var x = Field(value.x, "x:");
                var y = Field(value.y, "y:");
                var z = Field(value.z, "z:");
                return new Vector3Int(x, y, z);
            }
        }

        private static Quaternion QuaternionField(Quaternion value, GUIContent? content)
        {
            using (Horizontal)
            {
                if (content.DrawLabel()) GUILayout.FlexibleSpace();
                var x = Field(value.x, "x:");
                var y = Field(value.y, "y:");
                var z = Field(value.z, "z:");
                var w = Field(value.w, "w:");
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