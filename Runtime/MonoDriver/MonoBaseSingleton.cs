using UnityEngine;

namespace YuzeToolkit.Utility
{
    public class MonoBaseSingleton<T> : MonoBase where T : MonoBaseSingleton<T>
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance != null) return _instance;

                var t = FindObjectOfType<T>();
                if (t != null)
                {
                    _instance = t;
                    return _instance;
                }
                
                var obj = new GameObject($"__{typeof(T).Name}__");
                _instance = obj.AddComponent<T>();
                return _instance;
            }
        }

        protected override void DoOnAwake()
        {
            if (_instance == null)
            {
                _instance = (T)this;
            }
            else if(_instance != this)
            {
                Destroy(this);
                Log($"类型为：{typeof(T).Name}的{nameof(Instance)}已经被创建！", LogType.Error);
            }
        }

        protected override void DoOnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}