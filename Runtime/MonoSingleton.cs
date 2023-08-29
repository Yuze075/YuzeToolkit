﻿using UnityEngine;

namespace YuzeToolkit.Utility
{
    public abstract class MonoSingleton<T> : MonoLogBase where T : MonoSingleton<T>
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance != null) return _instance;

                var obj = new GameObject($"__{typeof(T).Name}__");
                _instance = obj.AddComponent<T>();
                return _instance;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            if (_instance == null)
            {
                _instance = (T)this;
            }
            else
            {
                Destroy(this);
                Log($"类型为：{typeof(T).Name}的{nameof(Instance)}已经被创建！", LogType.Error);
            }
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}