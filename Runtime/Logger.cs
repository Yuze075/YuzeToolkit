namespace YuzeToolkit.Framework.Utility
{
    public static class Logger
    {
        private static string[] Tags { get; } = new[] { "Utility" };

        public static void Log(string massage)
        {
#if LOGGER_SYSTEM
            LoggerSystem.LoggerSystem.Log(massage, Tags);
#else
            UnityEngine.Debug.Log(massage);
#endif
        }

        public static void Warning(string massage)
        {
#if LOGGER_SYSTEM
            LoggerSystem.LoggerSystem.Warning(massage, Tags);
#else
            UnityEngine.Debug.LogWarning(massage);
#endif
        }

        public static void Error(string massage)
        {
#if LOGGER_SYSTEM
            LoggerSystem.LoggerSystem.Error(massage, Tags);
#else
            UnityEngine.Debug.LogError(massage);
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