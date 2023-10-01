using System;
using System.Collections.Generic;
using UnityEngine;
using YuzeToolkit.Log;

namespace YuzeToolkit.MonoDriver
{
    /// <summary>
    /// <see cref="MonoDriverBase"/>的管理器, 用于处理不同的更新逻辑的默认初始化
    /// </summary>
    [DefaultExecutionOrder(-10000)]
    public class MonoDriverManager : MonoBehaviour
    {
        #region static

        private static bool _s_isInitialize;
        private static MonoDriverManager _s_monoDriverManager;
        private static readonly List<(IMonoBase, DisposableWaiter)> S_MonoBases = new();

        public static IDisposable Run(IMonoBase monoBase)
        {
            if (!_s_isInitialize)
            {
                LogSys.Warning($"没有创建对应的{nameof(MonoDriverManager)}！");
                var waiter = new DisposableWaiter();
                S_MonoBases.Add((monoBase, waiter));
                return waiter;
            }

            if (S_MonoBases.Count > 0)
            {
                foreach (var (sMonoBase, disposableWaiter) in S_MonoBases)
                {
                    disposableWaiter.Disposable = _s_monoDriverManager.RunPrivate(sMonoBase);
                }

                S_MonoBases.Clear();
            }

            return _s_monoDriverManager.RunPrivate(monoBase);
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

        private FirstMonoDriver _first;
        private BeforeMonoDriver _before;
        private AfterMonoDriver _after;
        private EndMonoDriver _end;
        private bool _initSelf;

        private void Init()
        {
            if (_initSelf) return;
            _initSelf = true;

            if (_s_isInitialize)
            {
                Destroy(gameObject);
                LogSys.Error("驱动器已经存在！", new[] { "Utility", "MonoDriverManager" });
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

            _first.Initialize();
            _before.Initialize();
            _after.Initialize();
            _end.Initialize();
        }

        private void Temp()
        {
            if (!_initSelf) return;
            if (_s_monoDriverManager != this) return;
            _initSelf = false;
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

        private IDisposable RunPrivate(IMonoBase monoBase)
        {
            if (monoBase == null) return new NullDisposable();
            var disposable = LifeCycleBase.Build(monoBase);
            switch (monoBase.Type)
            {
                case OrderType.First:
                    _first.AddMonoBase(disposable);
                    break;
                case OrderType.Before:
                    _before.AddMonoBase(disposable);
                    break;
                case OrderType.After:
                    _after.AddMonoBase(disposable);
                    break;
                case OrderType.End:
                    _end.AddMonoBase(disposable);
                    break;
                default:
                    throw LogSys.ThrowException(new ArgumentOutOfRangeException());
            }

            return disposable;
        }

        private class DisposableWaiter : IDisposable
        {
            public IDisposable Disposable;

            public void Dispose()
            {
                if (Disposable == null)
                {
                    LogSys.Warning($"还未获得驱动器的{nameof(IDisposable)}对象！");
                    return;
                }

                Disposable.Dispose();
            }
        }
    }
}