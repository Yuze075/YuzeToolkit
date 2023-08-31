using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace YuzeToolkit.Utility
{
    public class NetworkBase : NetworkLogBase, IMonoBase
    {
        public float DeltaTime => MonoDriverBase.DeltaTime;
        public float FixedDeltaTime => MonoDriverBase.FixedDeltaTime;

        private ILifeCycle _lifeCycle;

        #region UnityLifeCycle

        protected override void Awake()
        {
            base.Awake();
            _lifeCycle = MonoDriverManager.RunLifeCycle(this);
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

        public override void OnDestroy()
        {
            _lifeCycle.Dispose();
            DoOnDestroy();
            base.OnDestroy();
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