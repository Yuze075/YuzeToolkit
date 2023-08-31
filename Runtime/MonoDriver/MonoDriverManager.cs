using System;
using UnityEngine;

namespace YuzeToolkit.Utility
{
    /// <summary>
    /// <see cref="MonoDriverBase"/>的管理器, 用于处理不同的更新逻辑的默认初始化
    /// </summary>
    [RequireComponent(typeof(AfterMonoDriver), typeof(BeforeMonoDriver), typeof(EndMonoDriver))]
    public class MonoDriverManager : MonoBehaviour
    {
        #region static

        private static bool _sIsInitialize;
        private static MonoDriverManager _sMonoDriverManager;

        public static ILifeCycle RunLifeCycle(IMonoBase monoBase, bool enable = true)
        {
            if (!_sIsInitialize) Load();
            return _sMonoDriverManager.RunLifeCyclePrivate(monoBase, enable);
        }

        public static IDisposable Run(IMonoBase monoBase, bool enable = true)
        {
            return RunLifeCycle(monoBase, enable);
        }

        private static void Load()
        {
            var prefab = Resources.Load<GameObject>("DriverManager");
            Instantiate(prefab).GetComponent<MonoDriverManager>().Init();
        }

        #endregion

        private void Awake()
        {
            Init();
// #if UNITY_EDITOR
//             UnityEditor.EditorApplication.playModeStateChanged += PlayModeStateChanged;
// #endif
        }

        private void OnDestroy()
        {
// #if UNITY_EDITOR
//             UnityEditor.EditorApplication.playModeStateChanged -= PlayModeStateChanged;
// #endif
            Temp();
        }

// #if UNITY_EDITOR
//         private void PlayModeStateChanged(UnityEditor.PlayModeStateChange obj) => Temp();
// #endif


        private FirstMonoDriver _first;
        private BeforeMonoDriver _before;
        private AfterMonoDriver _after;
        private EndMonoDriver _end;
        private bool _initSelf;

        private void Init()
        {
            if (_initSelf) return;
            _initSelf = true;

            if (_sIsInitialize)
            {
                Destroy(gameObject);
                LogSystem.Error("驱动器已经存在！", new[] { "Utility", "MonoDriverManager" });
                return;
            }

            if (!TryGetComponent(out _first))
            {
                _first = gameObject.AddComponent<FirstMonoDriver>();
            }

            if (!TryGetComponent(out _before))
            {
                _before = gameObject.AddComponent<BeforeMonoDriver>();
            }

            if (!TryGetComponent(out _after))
            {
                _after = gameObject.AddComponent<AfterMonoDriver>();
            }

            if (!TryGetComponent(out _end))
            {
                _end = gameObject.AddComponent<EndMonoDriver>();
            }

            DontDestroyOnLoad(gameObject);
            _sIsInitialize = true;
            _sMonoDriverManager = this;

            _first.Initialize();
            _before.Initialize();
            _after.Initialize();
            _end.Initialize();
        }

        private void Temp()
        {
            if (!_initSelf) return;
            if (_sMonoDriverManager != this) return;
            _initSelf = false;
            _sIsInitialize = false;
            _sMonoDriverManager = null;
            Destroy(_first);
            Destroy(_before);
            Destroy(_after);
            Destroy(_end);
            _first = null;
            _before = null;
            _after = null;
            _end = null;
        }

        private ILifeCycle RunLifeCyclePrivate(IMonoBase monoBase, bool enable)
        {
            var lifeCycle = LifeCycleBase.Build(monoBase, enable);
            if (monoBase is null) return null;

            switch (monoBase.Type)
            {
                case OrderType.First:
                    _first.AddMonoBase(lifeCycle);
                    break;
                case OrderType.Before:
                    _before.AddMonoBase(lifeCycle);
                    break;
                case OrderType.After:
                    _after.AddMonoBase(lifeCycle);
                    break;
                case OrderType.End:
                    _end.AddMonoBase(lifeCycle);
                    break;
                default:
                    throw LogSystem.ThrowException(new ArgumentOutOfRangeException());
            }

            return lifeCycle;
        }
    }
}