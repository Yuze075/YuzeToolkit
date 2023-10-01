using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace YuzeToolkit.IoC
{
    [Serializable]
    public struct ParentReference
    {
#if UNITY_EDITOR
        static ParentReference()
        {
            EditorApplication.playModeStateChanged -= OnSceneChange;
            EditorApplication.playModeStateChanged += OnSceneChange;
        }

        private static void OnSceneChange(PlayModeStateChange _) => _containers.Clear();
#endif


        private static Dictionary<string, Container> _containers = new();

        [SerializeField] private bool isRoot;
        [SerializeField] private string parentKey;
        [SerializeField] private string selfKey;
        [SerializeField] private Container parent;

        public bool IsRoot => isRoot;
        public Container Parent => parent;

        public void Init(Container selfContainer)
        {
            if (!isRoot)
            {
                if (!_containers.TryGetValue(parentKey, out var container))
                {
                    throw new IoCException($"{selfContainer.GetType()}无法获取到key为{parentKey}的Parent！");
                }

                parent = container;
            }

            if (!string.IsNullOrWhiteSpace(selfKey))
            {
                _containers.TryAdd(selfKey, selfContainer);
            }
        }

        public void Init(Container selfContainer, Container parentContainer)
        {
            if (parentContainer == null)
            {
                throw new IoCException($"传入的parentContainer：{parentContainer.GetType()}为空！");
            }

            parent = parentContainer;
            if (!string.IsNullOrWhiteSpace(selfKey))
            {
                _containers.TryAdd(selfKey, selfContainer);
            }
        }
    }
}