using System;
using UnityEngine;

namespace YuzeToolkit.GUITool
{
    public static class FGUILayout
    {
        private class BeginAndEnd : IDisposable
        {
            private readonly Action _end;

            public BeginAndEnd(Action begin, Action end)
            {
                begin?.Invoke();
                _end = end;
            }

            public void Dispose()
            {
                _end?.Invoke();
            }
        }

        #region Horizontal

        public static IDisposable Horizontal(params GUILayoutOption[] options) =>
            Horizontal(GUIContent.none, GUIStyle.none, options);

        public static IDisposable Horizontal(string text, params GUILayoutOption[] options) =>
            Horizontal(new GUIContent(text), GUIStyle.none, options);

        public static IDisposable Horizontal(Texture image, params GUILayoutOption[] options) =>
            Horizontal(new GUIContent(image), GUIStyle.none, options);

        public static IDisposable Horizontal(GUIContent content, params GUILayoutOption[] options) =>
            Horizontal(content, GUIStyle.none, options);

        public static IDisposable Horizontal(GUIStyle style, params GUILayoutOption[] options) =>
            Horizontal(GUIContent.none, style, options);

        public static IDisposable Horizontal(string text, GUIStyle style, params GUILayoutOption[] options) =>
            Horizontal(new GUIContent(text), style, options);

        public static IDisposable Horizontal(Texture image, GUIStyle style, params GUILayoutOption[] options) =>
            Horizontal(new GUIContent(image), style, options);

        public static IDisposable Horizontal(GUIContent content, GUIStyle style, params GUILayoutOption[] options) =>
            new BeginAndEnd(() => GUILayout.BeginHorizontal(content, style, options), GUILayout.EndHorizontal);

        #endregion

        #region Vertical

        public static IDisposable Vertical(params GUILayoutOption[] options) =>
            Vertical(GUIContent.none, GUIStyle.none, options);

        public static IDisposable Vertical(string text, params GUILayoutOption[] options) =>
            Vertical(new GUIContent(text), GUIStyle.none, options);

        public static IDisposable Vertical(Texture image, params GUILayoutOption[] options) =>
            Vertical(new GUIContent(image), GUIStyle.none, options);

        public static IDisposable Vertical(GUIContent content, params GUILayoutOption[] options) =>
            Vertical(content, GUIStyle.none, options);

        public static IDisposable Vertical(GUIStyle style, params GUILayoutOption[] options) =>
            Vertical(GUIContent.none, style, options);

        public static IDisposable Vertical(string text, GUIStyle style, params GUILayoutOption[] options) =>
            Vertical(new GUIContent(text), style, options);

        public static IDisposable Vertical(Texture image, GUIStyle style, params GUILayoutOption[] options) =>
            Vertical(new GUIContent(image), style, options);

        public static IDisposable Vertical(GUIContent content, GUIStyle style, params GUILayoutOption[] options) =>
            new BeginAndEnd(() => GUILayout.BeginVertical(content, style, options), GUILayout.EndVertical);

        #endregion

        #region Area

        public static IDisposable Area(Rect screenRect) =>
            Area(screenRect, GUIContent.none, GUIStyle.none);

        public static IDisposable Area(Rect screenRect, string text) =>
            Area(screenRect, new GUIContent(text), GUIStyle.none);

        public static IDisposable Area(Rect screenRect, Texture image) =>
            Area(screenRect, new GUIContent(image), GUIStyle.none);

        public static IDisposable Area(Rect screenRect, GUIContent content) =>
            Area(screenRect, content, GUIStyle.none);

        public static IDisposable Area(Rect screenRect, GUIStyle style) =>
            Area(screenRect, GUIContent.none, style);

        public static IDisposable Area(Rect screenRect, string text, GUIStyle style) =>
            Area(screenRect, new GUIContent(text), style);

        public static IDisposable Area(Rect screenRect, Texture image, GUIStyle style) =>
            Area(screenRect, new GUIContent(image), style);

        public static IDisposable Area(Rect screenRect, GUIContent content, GUIStyle style) =>
            new BeginAndEnd(() => GUILayout.BeginArea(screenRect, content, style), GUILayout.EndArea);

        #endregion

        #region ScrollView

        public static IDisposable ScrollView(Vector2 scrollPosition, out Vector2 newPosition,
            params GUILayoutOption[] options) => ScrollView(scrollPosition, false, false,
            GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar, GUI.skin.scrollView, out newPosition,
            options);

        public static IDisposable ScrollView(Vector2 scrollPosition, bool alwaysShowHorizontal,
            bool alwaysShowVertical, out Vector2 newPosition, params GUILayoutOption[] options) => ScrollView(
            scrollPosition, alwaysShowHorizontal, alwaysShowVertical, GUI.skin.horizontalScrollbar,
            GUI.skin.verticalScrollbar, GUI.skin.scrollView, out newPosition, options);

        public static IDisposable ScrollView(Vector2 scrollPosition, GUIStyle horizontalScrollbar,
            GUIStyle verticalScrollbar, out Vector2 newPosition, params GUILayoutOption[] options) =>
            ScrollView(scrollPosition, false, false, horizontalScrollbar,
                verticalScrollbar, GUI.skin.scrollView, out newPosition, options);

        public static IDisposable ScrollView(Vector2 scrollPosition, GUIStyle background, out Vector2 newPosition,
            params GUILayoutOption[] options) => ScrollView(scrollPosition, false, false,
            GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar, background, out newPosition, options);

        public static IDisposable ScrollView(Vector2 scrollPosition,
            bool alwaysShowHorizontal, bool alwaysShowVertical,
            GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar,
            GUIStyle background, out Vector2 newPosition, params GUILayoutOption[] options)
        {
            newPosition = GUILayout.BeginScrollView(scrollPosition, alwaysShowHorizontal, alwaysShowVertical,
                horizontalScrollbar, verticalScrollbar, background, options);
            return new BeginAndEnd(null, GUILayout.EndScrollView);
        }

        #endregion

        #region Box

        public static void Box(Texture image, params GUILayoutOption[] options) =>
            Box(new GUIContent(image), GUI.skin.box, options);

        public static void Box(string text, params GUILayoutOption[] options) =>
            Box(new GUIContent(text), GUI.skin.box, options);

        public static void Box(GUIContent content, params GUILayoutOption[] options) =>
            Box(content, GUI.skin.box, options);

        public static void Box(Texture image, GUIStyle style, params GUILayoutOption[] options) =>
            Box(new GUIContent(image), style, options);

        public static void Box(string text, GUIStyle style, params GUILayoutOption[] options) =>
            Box(new GUIContent(text), style, options);

        public static void Box(GUIContent content, GUIStyle style, params GUILayoutOption[] options) =>
            GUILayout.Box(content, style, options);

        #endregion

        #region Space

        public static void Space(float pixels) => GUILayout.Space(pixels);

        public static void FlexibleSpace() => GUILayout.FlexibleSpace();

        #endregion

        #region Button

        public static void Button(string name, Action action)
        {
            if (GUILayout.Button(name)) action?.Invoke();
        }

        public static void Button(Action action)
        {
            if (GUILayout.Button("Button")) action?.Invoke();
        }

        public static void Button<T>(string name, Action<T> action, T value)
        {
            if (GUILayout.Button(name)) action?.Invoke(value);
        }

        public static void Button<T>(Action<T> action, T value)
        {
            if (GUILayout.Button("Button")) action?.Invoke(value);
        }

        public static void Button<T1, T2>(string name, Action<T1, T2> action, T1 value1, T2 value2)
        {
            if (GUILayout.Button(name)) action?.Invoke(value1, value2);
        }

        public static void Button<T1, T2>(Action<T1, T2> action, T1 value1, T2 value2)
        {
            if (GUILayout.Button("Button")) action?.Invoke(value1, value2);
        }

        public static void Button<T1, T2, T3>(string name, Action<T1, T2, T3> action, T1 value1, T2 value2,
            T3 value3)
        {
            if (GUILayout.Button(name)) action?.Invoke(value1, value2, value3);
        }

        public static void Button<T1, T2, T3>(Action<T1, T2, T3> action, T1 value1, T2 value2, T3 value3)
        {
            if (GUILayout.Button("Button")) action?.Invoke(value1, value2, value3);
        }

        public static void Button<T1, T2, T3, T4>(string name, Action<T1, T2, T3, T4> action, T1 value1, T2 value2,
            T3 value3, T4 value4)
        {
            if (GUILayout.Button(name)) action?.Invoke(value1, value2, value3, value4);
        }

        public static void Button<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 value1, T2 value2, T3 value3,
            T4 value4)
        {
            if (GUILayout.Button("Button")) action?.Invoke(value1, value2, value3, value4);
        }

        public static T Button<T>(string name, Func<T> func) =>
            GUILayout.Button(name) && func != null ? func.Invoke() : default;

        public static T Button<T>(Func<T> func) =>
            GUILayout.Button("Button") && func != null ? func.Invoke() : default;

        public static TResult Button<T, TResult>(string name, Func<T, TResult> func, T value) =>
            GUILayout.Button(name) && func != null ? func.Invoke(value) : default;

        public static TResult Button<T, TResult>(Func<T, TResult> func, T value) =>
            GUILayout.Button("Button") && func != null ? func.Invoke(value) : default;

        public static TResult Button<T1, T2, TResult>(string name, Func<T1, T2, TResult> func,
            T1 value1, T2 value2) => GUILayout.Button(name) && func != null
            ? func.Invoke(value1, value2)
            : default;

        public static TResult Button<T1, T2, TResult>(Func<T1, T2, TResult> func,
            T1 value1, T2 value2) => GUILayout.Button("Button") && func != null
            ? func.Invoke(value1, value2)
            : default;

        public static TResult Button<T1, T2, T3, TResult>(string name, Func<T1, T2, T3, TResult> func,
            T1 value1, T2 value2, T3 value3) => GUILayout.Button(name) && func != null
            ? func.Invoke(value1, value2, value3)
            : default;

        public static TResult Button<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func,
            T1 value1, T2 value2, T3 value3) => GUILayout.Button("Button") && func != null
            ? func.Invoke(value1, value2, value3)
            : default;

        public static TResult Button<T1, T2, T3, T4, TResult>(string name, Func<T1, T2, T3, T4, TResult> func,
            T1 value1, T2 value2, T3 value3, T4 value4) => GUILayout.Button(name) && func != null
            ? func.Invoke(value1, value2, value3, value4)
            : default;

        public static TResult Button<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func,
            T1 value1, T2 value2, T3 value3, T4 value4) => GUILayout.Button("Button") && func != null
            ? func.Invoke(value1, value2, value3, value4)
            : default;

        #endregion

        #region Field

        public static T? Field<T>(T? value, string? name = null)
        {
            return value switch
            {
                bool newValue => BoolField(newValue, name) is T t ? t : value,
                string newValue => StringField(newValue, name) is T t ? t : value,
                sbyte newValue => TypeField(newValue, name, str =>
                    sbyte.TryParse(str, out var i) ? (true, i) : (false, default)) is T t
                    ? t
                    : value,
                byte newValue => TypeField(newValue, name, str =>
                    byte.TryParse(str, out var i) ? (true, i) : (false, default)) is T t
                    ? t
                    : value,
                short newValue => TypeField(newValue, name, str =>
                    short.TryParse(str, out var i) ? (true, i) : (false, default)) is T t
                    ? t
                    : value,
                int newValue => TypeField(newValue, name, str =>
                    int.TryParse(str, out var i) ? (true, i) : (false, default)) is T t
                    ? t
                    : value,
                long newValue => TypeField(newValue, name, str =>
                    long.TryParse(str, out var i) ? (true, i) : (false, default)) is T t
                    ? t
                    : value,
                float newValue => TypeField(newValue, name, str =>
                    float.TryParse(str, out var i) ? (true, i) : (false, default)) is T t
                    ? t
                    : value,
                double newValue => TypeField(newValue, name, str =>
                    double.TryParse(str, out var i) ? (true, i) : (false, default)) is T t
                    ? t
                    : value,
                ushort newValue => TypeField(newValue, name, str =>
                    ushort.TryParse(str, out var i) ? (true, i) : (false, default)) is T t
                    ? t
                    : value,
                uint newValue => TypeField(newValue, name, str =>
                    uint.TryParse(str, out var i) ? (true, i) : (false, default)) is T t
                    ? t
                    : value,
                ulong newValue => TypeField(newValue, name, str =>
                    ulong.TryParse(str, out var i) ? (true, i) : (false, default)) is T t
                    ? t
                    : value,
                Vector2 newValue => Vector2Field(newValue, name) is T t ? t : value,
                Vector3 newValue => Vector3Field(newValue, name) is T t ? t : value,
                Vector4 newValue => Vector4Field(newValue, name) is T t ? t : value,
                Vector2Int newValue => Vector2IntField(newValue, name) is T t ? t : value,
                Vector3Int newValue => Vector3IntField(newValue, name) is T t ? t : value,
                Quaternion newValue => QuaternionField(newValue, name) is T t ? t : value,
                IFGUILayoutField<T> fastGUIField => fastGUIField.DrawField(value, name),
                _ => CantDrawField(value, name)
            };
        }

        public static void Field<T>(T? value, out T? newValue, string? name = null) => newValue = Field(value, name);
        public static void Field<T>(T? value, Action<T?> set, string? name = null) => set(Field(value, name));
        public static void Field<T>(ref T? value, string? name = null) => value = Field(value, name);
        public static T? Field<T>(Func<T?> get, string? name = null) => Field(get(), name);
        public static void Field<T>(Func<T?> get, out T? newValue, string? name = null) => newValue = Field(get(), name);
        public static void Field<T>(Func<T?> get, Action<T?> set, string? name = null) => set(Field(get(), name));


        private static T? TypeField<T>(T? value, string? name, Func<string, (bool, T)> tryParse)
        {
            using (Horizontal())
            {
                if (!string.IsNullOrWhiteSpace(name)) GUILayout.Label(name);
                var str = GUILayout.TextField($"{value}");
                var (canParse, i) = tryParse.Invoke(str);
                if (canParse) return i;
                return string.IsNullOrWhiteSpace(str) ? default : value;
            }
        }

        private static T? CantDrawField<T>(T? value, string? name)
        {
            using (Horizontal())
            {
                if (!string.IsNullOrWhiteSpace(name)) GUILayout.Label(name);
                GUILayout.Label($"无法显示{typeof(T).Name}类型!", (string)null!);
            }

            return value;
        }

        private static bool BoolField(bool value, string? name)
        {
            using (Horizontal())
            {
                if (!string.IsNullOrWhiteSpace(name)) GUILayout.Label(name);
                return GUILayout.Toggle(value, (string)null!);
            }
        }

        private static string StringField(string value, string? name)
        {
            using (Horizontal())
            {
                if (!string.IsNullOrWhiteSpace(name)) GUILayout.Label(name);
                return GUILayout.TextField(value);
            }
        }

        private static Vector2 Vector2Field(Vector2 value, string? name)
        {
            using (Horizontal())
            {
                if (!string.IsNullOrWhiteSpace(name))
                {
                    GUILayout.Label(name);
                    GUILayout.Space(20);
                }

                var x = Field(value.x, "x:");
                var y = Field(value.y, "y:");
                return new Vector2(x, y);
            }
        }

        private static Vector3 Vector3Field(Vector3 value, string? name)
        {
            using (Horizontal())
            {
                if (!string.IsNullOrWhiteSpace(name))
                {
                    GUILayout.Label(name);
                    GUILayout.FlexibleSpace();
                }

                var x = Field(value.x, "x:");
                var y = Field(value.y, "y:");
                var z = Field(value.z, "z:");
                return new Vector3(x, y, z);
            }
        }

        private static Vector4 Vector4Field(Vector4 value, string? name)
        {
            using (Horizontal())
            {
                if (!string.IsNullOrWhiteSpace(name))
                {
                    GUILayout.Label(name);
                    GUILayout.FlexibleSpace();
                }

                var x = Field(value.x, "x:");
                var y = Field(value.y, "y:");
                var z = Field(value.z, "z:");
                var w = Field(value.w, "w:");
                return new Vector4(x, y, z, w);
            }
        }

        private static Vector2Int Vector2IntField(Vector2Int value, string? name)
        {
            using (Horizontal())
            {
                if (!string.IsNullOrWhiteSpace(name))
                {
                    GUILayout.Label(name);
                    GUILayout.Space(20);
                }

                var x = Field(value.x, "x:");
                var y = Field(value.y, "y:");
                return new Vector2Int(x, y);
            }
        }

        private static Vector3Int Vector3IntField(Vector3Int value, string? name)
        {
            using (Horizontal())
            {
                if (!string.IsNullOrWhiteSpace(name))
                {
                    GUILayout.Label(name);
                    GUILayout.FlexibleSpace();
                }

                var x = Field(value.x, "x:");
                var y = Field(value.y, "y:");
                var z = Field(value.z, "z:");
                return new Vector3Int(x, y, z);
            }
        }

        private static Quaternion QuaternionField(Quaternion value, string? name)
        {
            using (Horizontal())
            {
                if (!string.IsNullOrWhiteSpace(name))
                {
                    GUILayout.Label(name);
                    GUILayout.FlexibleSpace();
                }

                var x = Field(value.x, "x:");
                var y = Field(value.y, "y:");
                var z = Field(value.z, "z:");
                var w = Field(value.w, "w:");
                return new Quaternion(x, y, z, w);
            }
        }

        #endregion
    }


}