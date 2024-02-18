#nullable enable
#if YUZE_USE_UNITY_NETCODE
using System.Diagnostics;
using Unity.Netcode;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.Network.LogTool
{
    public static class NetUnityLoggingExtensions
    {
#pragma warning disable CS0618 // 类型或成员已过时
        public const string C_IsNotServer = "不是服务端不能调用!";
        public const string C_IsNotOwner = "不是拥有者不能调用!";

        [UnityEngine.HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_ASSERT_CHECK")]
        public static void CheckServer(this NetworkBehaviour self, string? name) =>
            LogSys.AssertInternal(self.IsServer, name, C_IsNotServer,
                self is IUnityLogging unityLogging ? unityLogging.Tags : null
#if UNITY_EDITOR && YUZE_LOG_TOOL_USE_CONTEXT
                , self
#endif
            );


        [UnityEngine.HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_ASSERT_CHECK")]
        public static void CheckOwner(this NetworkBehaviour self, string? name) =>
            LogSys.AssertInternal(self.IsOwner, name, C_IsNotOwner,
                self is IUnityLogging unityLogging ? unityLogging.Tags : null
#if UNITY_EDITOR && YUZE_LOG_TOOL_USE_CONTEXT
                , self
#endif
            );
#pragma warning restore CS0618 // 类型或成员已过时
    }
}
#endif