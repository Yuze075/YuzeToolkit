using System;
using UnityEngine;

namespace YuzeToolkit.Framework.Utility
{
    public class MonoDriver : MonoSingleton<MonoDriver>
    {
        public Action UpdateAction { get; set; }
        public Action FixedUpdateAction { get; set; }
        public Action LateUpdateAction { get; set; }

        private void Update()
        {
            UpdateAction?.Invoke();
        }

        private void FixedUpdate()
        {
            FixedUpdateAction?.Invoke();
        }

        private void LateUpdate()
        {
            LateUpdateAction?.Invoke();
        }
    }
}