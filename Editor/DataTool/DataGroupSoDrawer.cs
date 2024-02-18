#nullable enable
#if YUZE_USE_EDITOR_TOOLBOX
using System;
using Toolbox.Editor;
using UnityEditor;
using UnityEngine;

namespace YuzeToolkit.DataTool.Editor
{
    [InitializeOnLoad]
    public static class DataGroupSoDrawer
    {
        static DataGroupSoDrawer()
        {
            ToolboxEditorToolbar.OnToolbarGui -= OnToolbarGui;
            ToolboxEditorToolbar.OnToolbarGui += OnToolbarGui;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static bool _s_saveTimeScale;
        private static float _s_timeScale = 1f;
        private static bool _s_isPlayMode;

        private static void OnPlayModeStateChanged(PlayModeStateChange playModeStateChange)
        {
            switch (playModeStateChange)
            {
                case PlayModeStateChange.EnteredPlayMode:
                    _s_isPlayMode = true;
                    if (!_s_saveTimeScale) _s_timeScale = 1f;
                    Time.timeScale = _s_timeScale;
                    break;
                case PlayModeStateChange.EnteredEditMode:
                case PlayModeStateChange.ExitingEditMode:
                case PlayModeStateChange.ExitingPlayMode:
                    _s_isPlayMode = false;
                    if (!_s_saveTimeScale) _s_timeScale = 1f;
                    Time.timeScale = 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(playModeStateChange), playModeStateChange, null);
            }
        }

        private static void OnToolbarGui()
        {
            GUILayout.FlexibleSpace();
            if (!_s_isPlayMode) FGUILayout.Button(DataGroupSo.RecheckData, "ReData");
            FGUILayout.Button(() =>
            {
                _s_timeScale = _s_timeScale switch
                {
                    0f => 0f,
                    0.25f => 0f,
                    0.5f => 0.25f,
                    1f => 0.5f,
                    2f => 1f,
                    4f => 2f,
                    8f => 4f,
                    _ => 1f
                };
            }, "<");
            FGUILayout.Field(ref _s_saveTimeScale, string.Empty);
            GUILayout.Label($"TimeScale:{_s_timeScale:F2}X");
            FGUILayout.Button(() =>
            {
                _s_timeScale = _s_timeScale switch
                {
                    0f => 0.25f,
                    0.25f => 0.5f,
                    0.5f => 1f,
                    1f => 2f,
                    2f => 4f,
                    4f => 8f,
                    8f => 8f,
                    _ => 1f
                };
            }, ">");
        }
    }
}
#endif