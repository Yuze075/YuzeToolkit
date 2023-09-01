using System;
using UnityEngine;

namespace YuzeToolkit.Utility
{
    /// <summary>
    /// 由<see cref="MonoDriverBase"/>驱动更新的<see cref="MonoBehaviour"/><br/>
    /// 可以设置更新优先级<see cref="IMonoBase.Priority"/>和在unity中的更新优先级<see cref="IMonoBase.Type"/>
    /// </summary>
    public abstract class MonoBase : MonoLogBase, IMonoBase
    {
        public static float SDeltaTime;
        public static float SFixedDeltaTime;
        public float DeltaTime => SDeltaTime;
        public float FixedDeltaTime => SFixedDeltaTime;

        private IDisposable _disposable;

        #region UnityLifeCycle

        protected void OnEnable()
        {
            _disposable = this.Run();
            Enable();
        }

        protected void OnDisable()
        {
            _disposable?.Dispose();
            _disposable = null;
            Disable();
        }

        #endregion

        #region LifeCycle

        /// <summary>
        /// 被封装的OnEnable函数
        /// </summary>
        protected virtual void Enable()
        {
        }

        /// <summary>
        /// 被封装的OnDisable函数
        /// </summary>
        protected virtual void Disable()
        {
        }

        #endregion
    }
}