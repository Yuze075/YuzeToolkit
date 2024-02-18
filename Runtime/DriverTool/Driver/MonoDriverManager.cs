#nullable enable
using System;
using UnityEngine;

namespace YuzeToolkit.DriverTool
{
    /// <summary>
    /// <see cref="MonoDriverBase"/>的管理器, 用于处理不同的更新逻辑的默认初始化
    /// </summary>
    [DefaultExecutionOrder(-10000)]
    public class MonoDriverManager : MonoBehaviour
    {
        #region static

        private static bool _s_isInitialize;
        private static MonoDriverManager? _s_monoDriverManager;

        public static UpdateToken Run(IMonoBase monoBase)
        {
            if (!_s_isInitialize)
                new GameObject("__MonoDriverManager(AutoCreate)__").AddComponent<MonoDriverManager>();
#if UNITY_EDITOR
            MonoBaseExtensions.IsNotNull(_s_monoDriverManager != null, nameof(_s_monoDriverManager));
#endif
            return _s_monoDriverManager.RunPrivate(monoBase);
        }

        #endregion

        private void Awake() => Init();
        private void OnDestroy() => Temp();

        private FirstMonoDriver? _first;
        private BeforeMonoDriver? _before;
        private AfterMonoDriver? _after;
        private EndMonoDriver? _end;

        private FirstMonoDriver First
        {
            get
            {
                MonoBaseExtensions.IsNotNull(_first != null, nameof(_first));
                return _first;
            }
        }

        private BeforeMonoDriver Before
        {
            get
            {
                MonoBaseExtensions.IsNotNull(_before != null, nameof(_before));
                return _before;
            }
        }

        private AfterMonoDriver After
        {
            get
            {
                MonoBaseExtensions.IsNotNull(_after != null, nameof(_after));
                return _after;
            }
        }

        private EndMonoDriver End
        {
            get
            {
                MonoBaseExtensions.IsNotNull(_end != null, nameof(_end));
                return _end;
            }
        }

        private void Init()
        {
            if (_s_isInitialize)
            {
                Destroy(gameObject);
                MonoBaseExtensions.LogError("驱动器已经存在！");
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
            _s_isInitialize = true;
            _s_monoDriverManager = this;
        }

        private void Temp()
        {
            if (_s_monoDriverManager != this) return;
            _s_isInitialize = false;
            _s_monoDriverManager = null;
            Destroy(_first);
            Destroy(_before);
            Destroy(_after);
            Destroy(_end);
            _first = null;
            _before = null;
            _after = null;
            _end = null;
        }

        private UpdateToken RunPrivate(IMonoBase monoBase) =>
            monoBase.UpdateOrderType switch
            {
                EOrderType.First => First.Add(monoBase),
                EOrderType.Before => Before.Add(monoBase),
                EOrderType.After => After.Add(monoBase),
                EOrderType.End => End.Add(monoBase),
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}