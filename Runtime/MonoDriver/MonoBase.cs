using System;
using System.Collections.Generic;
using UnityEngine;

namespace YuzeToolkit.Utility
{
    /// <summary>
    /// 由<see cref="MonoDriverBase"/>驱动更新的<see cref="MonoBehaviour"/><br/>
    /// 可以设置更新优先级<see cref="Priority"/>和在unity中的更新优先级<see cref="Type"/>
    /// </summary>
    public abstract class MonoBase : MonoLogBase, IMonoBase
    {
        public virtual int Priority => 0;

        public virtual OrderType Type => OrderType.After;

        public float DeltaTime => MonoDriverBase.DeltaTime;
        public float FixedDeltaTime => MonoDriverBase.FixedDeltaTime;
        
        #region Internal

        private bool _isEnable;
        private IList<MonoBaseWrapperList<IMonoOnUpdate>.MonoBaseWrapper> _updateWrapperList;
        private int _updateIndex = -1;
        private IList<MonoBaseWrapperList<IMonoOnFixedUpdate>.MonoBaseWrapper> _fixedUpdateWrapperList;
        private int _fixedUpdateIndex = -1;
        private IList<MonoBaseWrapperList<IMonoOnLateUpdate>.MonoBaseWrapper> _lateUpdateWrapperList;
        private int _lateUpdateIndex = -1;

        bool IMonoBase.IsEnable => _isEnable;

        IList<MonoBaseWrapperList<IMonoOnUpdate>.MonoBaseWrapper> IMonoBase.UpdateWrappers
        {
            set => _updateWrapperList = value;
        }

        int IMonoBase.UpdateIndex
        {
            set => _updateIndex = value;
        }

        IList<MonoBaseWrapperList<IMonoOnFixedUpdate>.MonoBaseWrapper> IMonoBase.FixedUpdateWrappers
        {
            set => _fixedUpdateWrapperList = value;
        }

        int IMonoBase.FixedUpdateIndex
        {
            set => _fixedUpdateIndex = value;
        }

        IList<MonoBaseWrapperList<IMonoOnLateUpdate>.MonoBaseWrapper> IMonoBase.LateUpdateWrappers
        {
            set => _lateUpdateWrapperList = value;
        }

        int IMonoBase.LateUpdateIndex
        {
            set => _lateUpdateIndex = value;
        }

        #endregion

        #region UnityLifeCycle

        protected sealed override void Awake()
        {
            base.Awake();
            MonoDriverManager.AddMonoBaseInternal(this);
            DoOnAwake();
        }

        protected void OnEnable()
        {
            _isEnable = true;
            if (_updateIndex != -1)
            {
                var wrapper = _updateWrapperList[_updateIndex];
                wrapper.Enable();
                _updateWrapperList[_updateIndex] = wrapper;
            }

            if (_fixedUpdateIndex != -1)
            {
                var wrapper = _fixedUpdateWrapperList[_fixedUpdateIndex];
                wrapper.Enable();
                _fixedUpdateWrapperList[_fixedUpdateIndex] = wrapper;
            }

            if (_lateUpdateIndex != -1)
            {
                var wrapper = _lateUpdateWrapperList[_lateUpdateIndex];
                wrapper.Enable();
                _lateUpdateWrapperList[_lateUpdateIndex] = wrapper;
            }

            DoOnEnable();
        }

        protected void OnDisable()
        {
            _isEnable = false;
            if (_updateIndex != -1)
            {
                var wrapper = _updateWrapperList[_updateIndex];
                wrapper.Disable();
                _updateWrapperList[_updateIndex] = wrapper;
            }

            if (_fixedUpdateIndex != -1)
            {
                var wrapper = _fixedUpdateWrapperList[_fixedUpdateIndex];
                wrapper.Disable();
                _fixedUpdateWrapperList[_fixedUpdateIndex] = wrapper;
            }

            if (_lateUpdateIndex != -1)
            {
                var wrapper = _lateUpdateWrapperList[_lateUpdateIndex];
                wrapper.Disable();
                _lateUpdateWrapperList[_lateUpdateIndex] = wrapper;
            }

            DoOnDisable();
        }

        protected void OnDestroy()
        {
            if (_updateIndex != -1)
            {
                var wrapper = _updateWrapperList[_updateIndex];
                wrapper.Destroy(_updateIndex);
                _updateWrapperList[_updateIndex] = wrapper;
            }

            if (_fixedUpdateIndex != -1)
            {
                var wrapper = _fixedUpdateWrapperList[_fixedUpdateIndex];
                wrapper.Destroy(_fixedUpdateIndex);
                _fixedUpdateWrapperList[_fixedUpdateIndex] = wrapper;
            }

            if (_lateUpdateIndex != -1)
            {
                var wrapper = _lateUpdateWrapperList[_lateUpdateIndex];
                wrapper.Destroy(_lateUpdateIndex);
                _lateUpdateWrapperList[_lateUpdateIndex] = wrapper;
            }

            // _updateWrapperList = null;
            // _fixedUpdateWrapperList = null;
            // _lateUpdateWrapperList = null;
            // _updateIndex = -1;
            // _fixedUpdateIndex = -1;
            // _lateUpdateIndex = -1;
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