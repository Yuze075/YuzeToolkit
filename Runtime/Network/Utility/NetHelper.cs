#nullable enable
#if YUZE_TOOLKIT_USE_UNITY_NETCODE
using System;
using System.Diagnostics.CodeAnalysis;
using Unity.Netcode;
using UnityEngine;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.Network
{
    public static class NetHelper
    {
        public static bool TryGetNetworkObjectId(object? o, out ulong objectId)
        {
            if (o is NetworkObject networkObject)
            {
                objectId = networkObject.NetworkObjectId;
                return true;
            }

            if (o is NetworkBehaviour networkBehaviour)
            {
                objectId = networkBehaviour.NetworkObjectId;
                return true;
            }
            objectId = 0;
            return false;
        }

        public static ulong GetNetworkObjectId(object? obj) => obj switch
        {
            NetworkObject networkObject => networkObject.NetworkObjectId,
            NetworkBehaviour networkBehaviour => networkBehaviour.NetworkObjectId,
            _ => throw new ArgumentException($"{obj}类型是{obj?.GetType()}, 不是需要的{typeof(NetworkBehaviour)}!")
        };

        public static NetworkObject? GetNetworkObject(ulong objectId)
        {
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out var networkObject))
                return networkObject;

            LogSys.LogWarning($"无法从{nameof(NetworkSpawnManager)}中获取到Id为{objectId}的{nameof(NetworkObject)}!");
            return null;
        }

        public static bool TryGetNetworkObject(ulong objectId, [MaybeNullWhen(false)] out NetworkObject networkObject)
        {
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out networkObject))
                return true;

            LogSys.LogWarning($"无法从{nameof(NetworkSpawnManager)}中获取到Id为{objectId}的{nameof(NetworkObject)}!");
            return false;
        }

        public static TComponent? GetNetworkComponent<TComponent>(ulong objectId)
        {
            if (!TryGetNetworkObject(objectId, out var networkObject)) return default;

            if (networkObject.TryGetComponent<TComponent>(out var component)) return component;

            LogSys.LogWarning($"无法从Id为{objectId}的{nameof(NetworkObject)}中获取到{typeof(TComponent).Name}的组件!");
            return default;
        }

        public static bool TryGetNetworkComponent<TComponent>(ulong objectId,
            [MaybeNullWhen(false)] out TComponent component)
        {
            if (!TryGetNetworkObject(objectId, out var networkObject))
            {
                component = default;
                return false;
            }

            if (networkObject.TryGetComponent(out component)) return true;

            LogSys.LogWarning($"无法从Id为{objectId}的{nameof(NetworkObject)}中获取到{typeof(TComponent).Name}的组件!");
            return false;
        }
    }
}
#endif