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

        private IDisposable _lifeCycle;

        #region UnityLifeCycle

        protected void OnEnable()
        {
            _lifeCycle = this.Run();
            Enable();
        }

        protected void OnDisable()
        {
            _lifeCycle.Dispose();
            _lifeCycle = null;
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