#nullable enable
#if YUZE_USE_UNITY_NETCODE
using System;
using Unity.Collections;
using Unity.Netcode;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.EventTool.Network
{
    public class NetEventSenderAndRegister<T> : IEventSender<T>, IEventRegister<T>, IDisposable
        where T : unmanaged, INetworkSerializeByMemcpy
    {
        public NetEventSenderAndRegister(NetworkManager networkManager, IEventNode eventNode,
            ulong? netObjectId = null)
        {
            _networkManager = networkManager;
            _eventNode = eventNode;
            _eventName = netObjectId == null
                ? $"[NetEventSenderAndRegister]: {typeof(T).FullName}"
                : $"[NetEventSenderAndRegister]: {typeof(T).FullName}({netObjectId})";

            _networkManager.OnClientConnectedCallback += OnClientConnectedCallback;
        }

        [NonSerialized] private bool _disposed;
        private readonly string _eventName;
        private readonly NetworkManager _networkManager;
        private readonly IEventNode _eventNode;

        public bool SendEvent(T eventValue)
        {
            if (!_networkManager.IsServer)
            {
                LogSys.LogError($"[{nameof(NetEventSenderAndRegister<T>)}.{nameof(SendEvent)}]: 不是服务端不能调用!");
                return false;
            }

            SendEventToAllClients(false, eventValue);
            return _eventNode.SendEvent(eventValue);
        }

        public bool SendEventAll(T eventValue)
        {
            if (!_networkManager.IsServer)
            {
                LogSys.LogError($"[{nameof(NetEventSenderAndRegister<T>)}.{nameof(SendEventAll)}]: 不是服务端不能调用!");
                return false;
            }

            SendEventToAllClients(true, eventValue);
            return _eventNode.SendEventAll(eventValue);
        }

        private void SendEventToAllClients(bool isSendAll, T message)
        {
            if (_networkManager == null || _networkManager.CustomMessagingManager == null) return;
            var writer = new FastBufferWriter(FastBufferWriter.GetWriteSize<T>(), Allocator.Temp);
            writer.WriteValueSafe(isSendAll);
            writer.WriteValueSafe(message);
            _networkManager.CustomMessagingManager.SendNamedMessageToAll(_eventName, writer);
        }

        public void AddEvent(Action<T> onEvent) => _eventNode.AddEvent(onEvent);
        public void RemoveEvent(Action<T> onEvent) => _eventNode.RemoveEvent(onEvent);

        public void Dispose()
        {
            if (_disposed) return;
            if (_networkManager != null && _networkManager.CustomMessagingManager != null)
            {
                _networkManager.CustomMessagingManager.UnregisterNamedMessageHandler(_eventName);
                _networkManager.OnClientConnectedCallback -= OnClientConnectedCallback;
            }

            _disposed = true;
        }

        private void OnClientConnectedCallback(ulong obj)
        {
            if (_networkManager.IsServer || _networkManager.CustomMessagingManager == null) return;
            _networkManager.CustomMessagingManager.RegisterNamedMessageHandler(_eventName, ReceiveMessage);
        }

        private void ReceiveMessage(ulong senderClientId, FastBufferReader messagePayload)
        {
            messagePayload.ReadValue(out bool isSendAll);
            messagePayload.ReadValue(out T value);
            if (isSendAll) SendEventAll(value);
            else SendEvent(value);
        }
    }
}
#endif