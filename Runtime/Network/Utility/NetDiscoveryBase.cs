#nullable enable
#if YUZE_USE_UNITY_NETCODE
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.Network
{
    public abstract class NetDiscoveryBase<TClientBroadCastInfo, TServerResponseInfo> : MonoBehaviour
        where TClientBroadCastInfo : INetworkSerializable, new()
        where TServerResponseInfo : INetworkSerializable, new()
    {
        #region Static

        private enum MessageType : byte
        {
            BroadCast = 0,
            Response = 1,
        }

        private static void S_WriteHeader(FastBufferWriter writer, MessageType messageType, ulong uniqueApplicationId)
        {
            writer.WriteValueSafe(uniqueApplicationId);
            writer.WriteByteSafe((byte)messageType);
        }

        private static bool S_ReadAndCheckHeader(FastBufferReader reader, MessageType expectedMessageType,
            ulong uniqueApplicationId)
        {
            reader.ReadValueSafe(out ulong receivedUniqueApplicationId);
            if (receivedUniqueApplicationId != uniqueApplicationId) return false;

            reader.ReadByteSafe(out var receivedMessageType);
            return (MessageType)receivedMessageType == expectedMessageType;
        }

        #endregion

        [SerializeField] private ushort port = 9978;
        [SerializeField] private ulong uniqueApplicationId = 1640380034;
#if YUZE_USE_EDITOR_TOOLBOX
        [Disable] [SerializeField]
        private bool isRunning;

        [Disable] [SerializeField]
        private bool isServer;

        [Disable] [SerializeField]
        private bool isClient;
#else
        private bool isRunning;
        private bool isServer;
        private bool isClient;
#endif

        private UdpClient? _udpClient;
        public bool IsRunning => isRunning;
        public bool IsServer => isServer;
        public bool IsClient => isClient;

        /// <summary>
        /// 开始客户端(服务端)网络发现进程, 开始接收对应数据
        /// </summary>
        /// <param name="server">是否以服务端开始网络发行进程, True代表服务端, False代表客户端</param>
        public void StartDiscovery(bool server)
        {
            if (isRunning)
            {
                this.LogWarning(
                    $"{nameof(NetDiscoveryBase<TClientBroadCastInfo, TServerResponseInfo>)}已经处于运行状态，无法再次启动!");
                return;
            }

            isRunning = true;
            isServer = server;
            isClient = !server;
            _udpClient = new UdpClient(server ? port : 0) { EnableBroadcast = true, MulticastLoopback = false };

            _udpClient.BeginReceive(server ? ServerReceiveCallback : ClientReceiveCallback, null);
        }

        /// <summary>
        /// 发送客户端的广播数据
        /// </summary>
        /// <param name="clientBroadCastInfo">发送的广播数据内容</param>
        public void SendClientBroadcastInfo(TClientBroadCastInfo clientBroadCastInfo)
        {
            // 当前没有在运行直接返回
            if (!isRunning)
            {
                this.LogWarning(
                    $"{nameof(NetDiscoveryBase<TClientBroadCastInfo, TServerResponseInfo>)}未处于运行状态，无法发送数据!");
                return;
            }

            // 如果_udpClient为空, 说明出现了错误, 结束发行进程, 直接返回
            if (_udpClient == null)
            {
                this.LogWarning(
                    $"{nameof(NetDiscoveryBase<TClientBroadCastInfo, TServerResponseInfo>)}的{nameof(_udpClient)}为空, 无法发送数据!");
                StopDiscovery();
                return;
            }

            // 不是客户端直接返回
            if (!isClient)
            {
                this.LogWarning("无法在作为服务端的时候触发客户端广播消息!");
                return;
            }

            // 设置广播地址
            var endPoint = new IPEndPoint(IPAddress.Broadcast, port);

            // 写入广播数据
            using var writer = new FastBufferWriter(1024, Allocator.Temp, 1024 * 64);
            S_WriteHeader(writer, MessageType.BroadCast, uniqueApplicationId);
            writer.WriteNetworkSerializable(clientBroadCastInfo);
            var data = writer.ToArray();

            // 开始发送广播数据
            _udpClient.BeginSend(data, data.Length, endPoint, ar =>
            {
                if (!ar.IsCompleted || _udpClient == null)
                {
                    this.LogWarning($"客户端发送数据失败!");
                    StopDiscovery();
                    return;
                }

                this.Log($"客户端发送长度为{_udpClient.EndSend(ar)}的数据成功!");
            }, null);
        }

        /// <summary>
        /// 暂停客户端(服务端)的网络发现进程
        /// </summary>
        public void StopDiscovery()
        {
            if (!isRunning)
            {
                this.LogWarning($"{nameof(NetDiscoveryBase<TClientBroadCastInfo, TServerResponseInfo>)}未处于运行状态，无法停止!");
                return;
            }

            isRunning = false;
            isServer = false;
            isClient = false;
            if (_udpClient == null) return;
            try
            {
                _udpClient.Close();
            }
            catch (Exception e)
            {
                this.LogWarning($"关闭{nameof(_udpClient)}出现异常: [{e.GetType()}]{{{e.Message}}}");
            }

            _udpClient = null;
        }

        private void ServerReceiveCallback(IAsyncResult asyncResult)
        {
            if (!asyncResult.IsCompleted || _udpClient == null)
            {
                this.LogWarning($"服务端接收数据失败!");
                StopDiscovery();
                return;
            }

            // 获取这次收到的数据
            IPEndPoint? ipEndPoint = null;
            var buffer = _udpClient.EndReceive(asyncResult, ref ipEndPoint);

            using var reader = new FastBufferReader(buffer, Allocator.Temp);

            // 读取头数据, 如果头数据不是广播数据, 或者uniqueApplicationId不匹配, 直接返回(继续保持接收数据的状态)
            if (!S_ReadAndCheckHeader(reader, MessageType.BroadCast, uniqueApplicationId))
            {
                _udpClient.BeginReceive(ServerReceiveCallback, null);
                return;
            }

            // 读取客户端发送过来的广播数据
            reader.ReadNetworkSerializable(out TClientBroadCastInfo clientBroadCastInfo);

            // 如果服务端不接受客户端的广播数据, 直接返回(继续保持接收数据的状态)
            if (!ServerProcessClientBroadcastInfo(ipEndPoint, clientBroadCastInfo, out var serverResponseInfo))
            {
                _udpClient.BeginReceive(ServerReceiveCallback, null);
                return;
            }

            // 接受客户端的广播数据, 并且发送返回数据给客户端
            using var writer = new FastBufferWriter(1024, Allocator.Temp, 1024 * 64);
            S_WriteHeader(writer, MessageType.Response, uniqueApplicationId);
            writer.WriteNetworkSerializable(serverResponseInfo);

            var data = writer.ToArray();
            _udpClient.BeginSend(data, data.Length, ipEndPoint, ar =>
            {
                if (!ar.IsCompleted || _udpClient == null)
                {
                    this.LogWarning($"服务端发送数据失败!");
                    StopDiscovery();
                    return;
                }

                this.Log($"服务端发送长度为{_udpClient.EndSend(ar)}的数据成功!");
            }, null);

            _udpClient.BeginReceive(ServerReceiveCallback, null);
        }

        private void ClientReceiveCallback(IAsyncResult asyncResult)
        {
            if (!asyncResult.IsCompleted || _udpClient == null)
            {
                this.LogWarning($"客户端接收数据失败!");
                return;
            }

            // 获取这次收到的数据
            IPEndPoint? ipEndPoint = null;
            var buffer = _udpClient.EndReceive(asyncResult, ref ipEndPoint);

            using var reader = new FastBufferReader(buffer, Allocator.Temp);

            // 读取头数据, 如果头数据不是响应数据, 或者uniqueApplicationId不匹配, 直接返回
            if (S_ReadAndCheckHeader(reader, MessageType.Response, uniqueApplicationId))
            {
                // 读取服务端发送过来的响应数据
                reader.ReadNetworkSerializable(out TServerResponseInfo serverResponseInfo);
                ClientProcessServerResponseInfo(ipEndPoint, serverResponseInfo);
            }

            _udpClient.BeginReceive(ClientReceiveCallback, null);
        }

        /// <summary>
        /// 服务端除了接收到来自客户端的广播信息, 判断这个<see cref="TClientBroadCastInfo"/>信息是否能够加入服务端
        /// </summary>
        /// <param name="senderIPEndPoint">客户端发送广播信息的地址</param>
        /// <param name="clientBroadCastInfo">客户端尝试获取服务端信息的广播数据</param>
        /// <param name="serverResponseInfo">当可以加入服务端时, 返回给客户端的响应数据</param>
        /// <returns>返回客户端能否加入服务端, Ture可以加入, False不能加入</returns>
        protected abstract bool ServerProcessClientBroadcastInfo(IPEndPoint senderIPEndPoint,
            TClientBroadCastInfo clientBroadCastInfo, [NotNullWhen(true)] out TServerResponseInfo? serverResponseInfo);

        /// <summary>
        /// 客户端处理接收到服务端返回的响应数据
        /// </summary>
        /// <param name="senderIPEndPoint">服务端返回数据的地址</param>
        /// <param name="serverResponseInfo">服务端返回的响应数据</param>
        protected abstract void ClientProcessServerResponseInfo(IPEndPoint senderIPEndPoint,
            TServerResponseInfo? serverResponseInfo);
    }
}
#endif