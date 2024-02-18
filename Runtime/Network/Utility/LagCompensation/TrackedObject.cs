#nullable enable
#if YUZE_USE_UNITY_NETCODE
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.Network.LagCompensation
{
    /// <summary>
    /// A component used for lag compensation. Each object with this component will get tracked
    /// </summary>
    public class TrackedObject : MonoBehaviour
    {
        private struct TrackedPoint
        {
            public Vector3 Position;
            public Quaternion Rotation;
        }

        private readonly Dictionary<float, TrackedPoint> _frameData = new();
        private FixedQueue<float>? _frameKeys;
        private int _maxPoints;

        private Vector3 _savedPosition;
        private Quaternion _savedRotation;

        private LagCompensationManager? _lagCompensationManager;

        private FixedQueue<float> FrameKeys
        {
            get
            {
                this.IsNotNull(_frameKeys != null, nameof(_frameKeys));
                return _frameKeys;
            }
        }

        private void Awake()
        {
            _lagCompensationManager = LagCompensationManager.S_Instance;

            _maxPoints = _lagCompensationManager.MaxQueuePoints();

            _frameKeys = new FixedQueue<float>(_maxPoints);
            _frameKeys.Enqueue(0);
            _lagCompensationManager.SimulationObjects.Add(this);
        }

        /// <summary>
        /// Gets the total amount of points stored in the component
        /// </summary>
        public int TotalPoints => FrameKeys.Count;

        /// <summary>
        /// Gets the average amount of time between the points in miliseconds
        /// </summary>
        public float AvgTimeBetweenPointsMs
        {
            get
            {
                if (FrameKeys.Count == 0) return 0;
                return (FrameKeys[^1] - FrameKeys[0]) / FrameKeys.Count * 1000f;
            }
        }

        /// <summary>
        /// Gets the total time history we have for this object
        /// </summary>
        public float TotalTimeHistory => FrameKeys[^1] - FrameKeys[0];

        internal void ReverseTransform(float secondsAgo)
        {
            _savedPosition = transform.position;
            _savedRotation = transform.rotation;

            var currentTime = NetworkManager.Singleton.NetworkTickSystem.LocalTime.TimeAsFloat;
            var targetTime = currentTime - secondsAgo;

            var previousTime = 0f;
            var nextTime = 0f;
            foreach (var time in FrameKeys)
            {
                if (previousTime <= targetTime && time >= targetTime)
                {
                    nextTime = time;
                    break;
                }

                previousTime = time;
            }

            var timeBetweenFrames = nextTime - previousTime;
            var timeAwayFromPrevious = currentTime - previousTime;
            var lerpProgress = timeAwayFromPrevious / timeBetweenFrames;
            transform.position = Vector3.Lerp(_frameData[previousTime].Position, _frameData[nextTime].Position,
                lerpProgress);
            transform.rotation = Quaternion.Slerp(_frameData[previousTime].Rotation, _frameData[nextTime].Rotation,
                lerpProgress);
        }

        internal void ResetStateTransform()
        {
            transform.position = _savedPosition;
            transform.rotation = _savedRotation;
        }

        private void OnDestroy()
        {
            if (_lagCompensationManager != null)
                _lagCompensationManager.SimulationObjects.Remove(this);
        }

        internal void AddFrame()
        {
            if (FrameKeys.Count == _maxPoints)
                _frameData.Remove(FrameKeys.Dequeue());

            var time = (float)NetworkManager.Singleton.NetworkTickSystem.LocalTime.FixedTime;
            _frameData.Add(time, new TrackedPoint()
            {
                Position = transform.position,
                Rotation = transform.rotation
            });
            FrameKeys.Enqueue(time);
        }
    }
}
#endif