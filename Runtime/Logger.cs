namespace YuzeToolkit.Framework.Utility
{
    public static class Logger
    {
        private static string[] Tags { get; } = new[] { "Utility" };

        public static void Log(string message)
        {
#if LOGGER_SYSTEM
            LoggerSystem.LoggerSystem.Log(message, Tags);
#else
            UnityEngine.Debug.Log(message);
#endif
        }

        public static void Warning(string message)
        {
#if LOGGER_SYSTEM
            LoggerSystem.LoggerSystem.Warning(message, Tags);
#else
            UnityEngine.Debug.LogWarning(message);
#endif
        }

        public static void Error(string message)
        {
#if LOGGER_SYSTEM
            LoggerSystem.LoggerSystem.Error(message, Tags);
#else
            UnityEngine.Debug.LogError(message);
#endif
        }

        public static void Exception(System.Exception exception)
        {
#if LOGGER_SYSTEM
            LoggerSystem.LoggerSystem.Exception(exception, Tags);
#else
            UnityEngine.Debug.LogException(exception);
#endif
        }
    }
}