﻿using UnityEngine;

namespace YuzeToolkit.Utility
{
    /// <summary>
    /// 由<see cref="MonoDriverBase"/>驱动更新的<see cref="MonoBehaviour"/><br/>
    /// 可以设置更新优先级<see cref="IMonoBase.Priority"/>和在unity中的更新优先级<see cref="IMonoBase.Type"/>
    /// </summary>
    public abstract class MonoBase : MonoLogBase, IMonoBase
    {
        public float DeltaTime => MonoDriverBase.DeltaTime;
        public float FixedDeltaTime => MonoDriverBase.FixedDeltaTime;

        private ILifeCycle _lifeCycle;

        #region UnityLifeCycle

        protected sealed override void Awake()
        {
            base.Awake();
            _lifeCycle = MonoDriverManager.RunLifeCycle(this, false);
            DoOnAwake();
        }

        protected void OnEnable()
        {
            _lifeCycle.Enable = true;
            DoOnEnable();
        }

        protected void OnDisable()
        {
            _lifeCycle.Enable = false;
            DoOnDisable();
        }

        protected void OnDestroy()
        {
            _lifeCycle.Dispose();
            DoOnDestroy();
        }

        #endregion

        #region LifeCycle

        /// <summary>
        /// 被封装的Awake函数
        /// </summary>
        protected virtual void DoOnAwake()
        {
        }

        /// <summary>
        /// 被封装的OnEnable函数
        /// </summary>
        protected virtual void DoOnEnable()
        {
        }

        /// <summary>
        /// 被封装的OnDisable函数
        /// </summary>
        protected virtual void DoOnDisable()
        {
        }

        /// <summary>
        /// 被封装的OnDestroy函数
        /// </summary>
        protected virtual void DoOnDestroy()
        {
        }

        #endregion
    }
}