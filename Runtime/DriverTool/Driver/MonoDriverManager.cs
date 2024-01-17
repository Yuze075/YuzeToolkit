#nullable enable
using System;
using UnityEngine;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.DriverTool
{
    /// <summary>
    /// <see cref="MonoDriverBase"/>的管理器, 用于处理不同的更新逻辑的默认初始化
    /// </summary>
    [DefaultExecutionOrder(-10000)]
    public class MonoDriverManager : MonoBehaviour
    {
        #region static

        private static bool s_isInitialize;
        private static MonoDriverManager? s_monoDriverManager;

        public static UpdateToken Run(IMonoBase monoBase)
        {
            if (!s_isInitialize)
                new GameObject("__MonoDriverManager(AutoCreate)__").AddComponent<MonoDriverManager>();
            LogSys.IsNotNull(s_monoDriverManager != null, nameof(s_monoDriverManager));
            return s_monoDriverManager.RunPrivate(monoBase);
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
                LogSys.IsNotNull(_first != null, nameof(_first));
                return _first;
            }
        }

        private BeforeMonoDriver Before
        {
            get
            {
                LogSys.IsNotNull(_before != null, nameof(_before));
                return _before;
            }
        }

        private AfterMonoDriver After
        {
            get
            {
                LogSys.IsNotNull(_after != null, nameof(_after));
                return _after;
            }
        }

        private EndMonoDriver End
        {
            get
            {
                LogSys.IsNotNull(_end != null, nameof(_end));
                return _end;
            }
        }

        private void Init()
        {
            if (s_isInitialize)
            {
                Destroy(gameObject);
                LogSys.LogWarning("驱动器已经存在！");
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
            s_isInitialize = true;
            s_monoDriverManager = this;
        }

        private void Temp()
        {
            if (s_monoDriverManager != this) return;
            s_isInitialize = false;
            s_monoDriverManager = null;
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
            monoBase.Type switch
            {
                EOrderType.First => First.Add(monoBase),
                EOrderType.Before => Before.Add(monoBase),
                EOrderType.After => After.Add(monoBase),
                EOrderType.End => End.Add(monoBase),
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}