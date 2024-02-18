#nullable enable
#if YUZE_USE_UNITY_NETCODE
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.Network.LagCompensation
{
    /// <summary>
    /// The main class for controlling lag compensation
    /// </summary>
    public class LagCompensationManager : MonoBehaviour
    {
        #region Static

        private static LagCompensationManager? _s_instance;

        public static LagCompensationManager S_Instance
        {
            get
            {
                LogSys.IsNotNull(_s_instance != null, nameof(_s_instance));
                return _s_instance;
            }
        }

        #endregion

        private NetworkManager? _networkManager;
        [SerializeField] private float secondsHistory;

        [SerializeField]
        [Tooltip(
            "If true this will sync transform changes after the rollback back to the physics engine so that queries like rayCasts use the compensated positions")]
        private bool syncTransforms = true;

        /// <summary>
        /// Simulation objects
        /// </summary>
        public readonly List<TrackedObject> SimulationObjects = new();

        private void Awake()
        {
            if (_s_instance != null && _s_instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _s_instance = this;
            this.Log($"{nameof(LagCompensationManager)}已经创建!");
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            if (_s_instance != this) return;
            _s_instance = null;
            this.Log($"{nameof(LagCompensationManager)}已经销毁!");
        }

        public void Init(NetworkManager networkManager)
        {
            if (networkManager is { IsServer: false, IsClient: false })
                throw new InvalidOperationException(
                    $"{nameof(NetworkManager)}还未启动, 无法初始化{nameof(LagCompensationManager)}!");
            _networkManager = networkManager;
            _networkManager.NetworkTickSystem.Tick += AddFrames;
        }

        public void Temp()
        {
            if (_networkManager != null) _networkManager.NetworkTickSystem.Tick -= AddFrames;
            _networkManager = null;
        }

        /// <summary>
        /// Turns time back a given amount of seconds, invokes an action and turns it back
        /// </summary>
        /// <param name="secondsAgo">The amount of seconds</param>
        /// <param name="action">The action to invoke when time is turned back</param>
        public void Simulate(float secondsAgo, Action action)
        {
            Simulate(secondsAgo, SimulationObjects, action);
        }

        /// <summary>
        /// Turns time back a given amount of second on the given objects, invokes an action and turns it back
        /// </summary>
        /// <param name="secondsAgo">The amount of seconds</param>
        /// <param name="simulatedObjects">The object to simulate back in time</param>
        /// <param name="action">The action to invoke when time is turned back</param>
        public void Simulate(float secondsAgo, IList<TrackedObject> simulatedObjects, Action action)
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                throw new NotServerException("Only the server can perform lag compensation");
            }

            foreach (var trackedObject in simulatedObjects)
            {
                trackedObject.ReverseTransform(secondsAgo);
            }

            if (!Physics.autoSyncTransforms && syncTransforms)
            {
                Physics.SyncTransforms();
            }

            action.Invoke();

            foreach (var trackedObject in simulatedObjects)
            {
                trackedObject.ResetStateTransform();
            }

            if (!Physics.autoSyncTransforms && syncTransforms)
            {
                Physics.SyncTransforms();
            }
        }

        /// <summary>
        /// Turns time back a given amount of seconds, invokes an action and turns it back. The time is based on the estimated RTT of a clientId
        /// </summary>
        /// <param name="clientId">The clientId's RTT to use</param>
        /// <param name="action">The action to invoke when time is turned back</param>
        public void Simulate(ulong clientId, Action action)
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                throw new NotServerException("Only the server can perform lag compensation");
            }

            float millisecondsDelay =
                NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetCurrentRtt(clientId) / 2f;
            Simulate(millisecondsDelay * 1000f, action);
        }

        internal void AddFrames()
        {
            foreach (var trackedObject in SimulationObjects)
            {
                trackedObject.AddFrame();
            }
        }

        internal int MaxQueuePoints()
        {
            return (int)(secondsHistory / (1f / NetworkManager.Singleton.NetworkConfig.TickRate));
        }
    }
}
#endif