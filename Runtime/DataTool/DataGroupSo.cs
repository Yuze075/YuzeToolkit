#nullable enable
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityComponent = UnityEngine.Component;

namespace YuzeToolkit
{
    public abstract class DataGroupSo : SoBase
    {
#if UNITY_EDITOR
        [UnityEditor.MenuItem(nameof(YuzeToolkit) + "/" + nameof(RecheckData))]
        public static void RecheckData()
        {
            var dataGroupSos = Resources.LoadAll<DataGroupSo>(string.Empty);
            foreach (var dataGroupSo in dataGroupSos)
            {
                if (!dataGroupSo.DoRecheckData()) continue;
                UnityEditor.EditorUtility.SetDirty(dataGroupSo);
                UnityEditor.AssetDatabase.SaveAssetIfDirty(dataGroupSo);
            }

            UnityEditor.AssetDatabase.Refresh();
        }

        private static IEnumerable<TTarget> GetUnityObjectInPath<TTarget>(params string[]? searchInFolders)
            where TTarget : Object => UnityEditor.AssetDatabase.FindAssets($"t:{typeof(TTarget).Name}", searchInFolders)
            .Select(guid => UnityEditor.AssetDatabase.LoadAssetAtPath<TTarget>
                (UnityEditor.AssetDatabase.GUIDToAssetPath(guid)));

        // ReSharper disable Unity.PerformanceAnalysis
        private static IEnumerable<TTarget> GetUnityComponentInPath<TTarget>(params string[]? searchInFolders)
            where TTarget : Component => UnityEditor.AssetDatabase
            .FindAssets($"t:{nameof(GameObject)}", searchInFolders)
            .Select(guid => UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>
                (UnityEditor.AssetDatabase.GUIDToAssetPath(guid)).GetComponent<TTarget>())
            .Where(target => target != null);

        private static string AddAssetsPath(string? path) => string.IsNullOrWhiteSpace(path) ? "Assets/" :
            Regex.IsMatch(path, "^Assets") ? path : "Assets/" + path;

        private static string[] AddAssetsPath(IReadOnlyList<string>? paths)
        {
            var array = new string[paths?.Count ?? 0];
            for (var i = 0; i < array.Length; i++) array[i] = AddAssetsPath(paths![i]);
            return array;
        }

        private protected static bool ReloadUnityObject<TTarget>(ref List<TTarget> list,
            params string[]? searchInFolders) where TTarget : Object
        {
            var isDirty = false;
            var newList = new List<TTarget>();
            foreach (var target in GetUnityObjectInPath<TTarget>(AddAssetsPath(searchInFolders)))
            {
                if (!newList.Contains(target)) newList.Add(target);
                if (isDirty || list.Contains(target)) continue;
                isDirty = true;
            }

            list = newList;
            return isDirty;
        }

        private protected static bool ReloadUnityComponent<TTarget>(ref List<TTarget> list,
            params string[]? searchInFolders) where TTarget : Component
        {
            var isDirty = false;
            var newList = new List<TTarget>();
            foreach (var target in GetUnityComponentInPath<TTarget>(AddAssetsPath(searchInFolders)))
            {
                if (newList.Contains(target)) continue;
                newList.Add(target);
                if (isDirty || list.Contains(target)) continue;
                isDirty = true;
            }

            list = newList;
            return isDirty;
        }

        private protected abstract bool DoRecheckData();
#endif
    }

    public abstract class DataGroupSo<TSo, TInterface> : DataGroupSo, IEnumerable<TInterface>
        where TSo : ScriptableObject, TInterface
    {
        [SerializeField] [ReorderableList] [InLineEditor]
        private List<TSo> sos = new();

        public IReadOnlyList<TInterface> Sos => sos;

        public IEnumerator<TInterface> GetEnumerator() =>
            sos.Where(data1 => data1 != null).Cast<TInterface>().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
#if UNITY_EDITOR
        [EditorButton(nameof(DoRecheckData))] [Directory] [SerializeField] [ReorderableList]
        private string[]? paths;

        private protected override bool DoRecheckData() => ReloadUnityObject(ref sos, paths);
#endif
    }

    public abstract class DataGroupSo<TSo, TComponent, TInterface> : DataGroupSo, IEnumerable<TInterface>
        where TSo : ScriptableObject, TInterface where TComponent : UnityComponent, TInterface
    {
        [SerializeField] [ReorderableList] [InLineEditor]
        private List<TSo> sos = new();

        public IReadOnlyList<TInterface> Sos => sos;

        [SerializeField] [ReorderableList] [InLineEditor]
        private List<TComponent> components = new();

        public IReadOnlyList<TInterface> Components => components;

        public IEnumerator<TInterface> GetEnumerator()
        {
            foreach (var so in sos.Where(so => so != null)) yield return so;
            foreach (var component in components.Where(component => component != null)) yield return component;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
#if UNITY_EDITOR
        [EditorButton(nameof(DoRecheckData))] [Directory] [SerializeField] [ReorderableList]
        private string[]? paths;

        private protected override bool DoRecheckData() =>
            ReloadUnityObject(ref sos, paths) && ReloadUnityComponent(ref components, paths);
#endif
    }
}