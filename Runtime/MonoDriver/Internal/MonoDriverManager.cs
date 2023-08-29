using System;
using UnityEngine;

namespace YuzeToolkit.Utility
{
    /// <summary>
    /// <see cref="MonoDriverBase"/>的管理器, 用于处理不同的更新逻辑的默认初始化
    /// </summary>
    [RequireComponent(typeof(AfterMonoDriver), typeof(BeforeMonoDriver), typeof(EndMonoDriver))]
    internal class MonoDriverManager : MonoBehaviour
    {
        #region static

        private static FirstMonoDriver _first;
        private static BeforeMonoDriver _before;
        private static AfterMonoDriver _after;
        private static EndMonoDriver _end;
        private static bool _isInitialize;

        /// <summary>
        /// 用于<see cref="MonoBase"/>在<see cref="MonoBase.Awake"/>添加逻辑
        /// </summary>
        internal static void AddMonoBaseInternal(IMonoBase monoBase)
        {
            if (!_isInitialize)
                throw LogSystem.ThrowException(new Exception("驱动器尚未初始化！"), new[] { "Utility", "MonoDriverManager" });
            // todo 进行一个懒汉式的初始化

            switch (monoBase.Type)
            {
                case OrderType.First:
                    _first.AddMonoBase(monoBase);
                    break;
                case OrderType.Before:
                    _before.AddMonoBase(monoBase);
                    break;
                case OrderType.After:
                    _after.AddMonoBase(monoBase);
                    break;
                case OrderType.End:
                    _end.AddMonoBase(monoBase);
                    break;
                default:
                    throw LogSystem.ThrowException(new ArgumentOutOfRangeException());
            }
        }

        #endregion

        /// <summary>
        /// 获取到对应顺序更新的<see cref="MonoDriverBase"/>
        /// </summary>
        private void Awake()
        {
            if (_isInitialize)
                throw LogSystem.ThrowException(new Exception("驱动器已经存在！"), new[] { "Utility", "MonoDriverManager" });

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
            _isInitialize = true;

            _first.Initialize();
            _before.Initialize();
            _after.Initialize();
            _end.Initialize();
        }
    }
}