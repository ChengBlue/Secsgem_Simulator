using System;

namespace SecsGemSimulator.Helpers
{
    public enum LogLevel
    {
        Info,
        Warning,
        Error,
        Tx, // Transmitted
        Rx  // Received
    }

    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public LogLevel Level { get; set; }
        public string Message { get; set; }
        public string Details { get; set; } // Hex dump or detailed info
    }

    public static class LogHelper
    {
        public static event Action<LogEntry> OnLog;

        public static void Log(LogLevel level, string message, string details = null)
        {
            var entry = new LogEntry
            {
                Timestamp = DateTime.Now,
                Level = level,
                Message = message,
                Details = details
            };
            OnLog?.Invoke(entry);
        }

        public static void Info(string message) => Log(LogLevel.Info, message);
        public static void Warning(string message) => Log(LogLevel.Warning, message);
        public static void Error(string message, Exception ex = null) => Log(LogLevel.Error, message, ex?.ToString());
    }
}
