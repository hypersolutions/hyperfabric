using System;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace HyperFabric.Logging
{
    public sealed class LogMessage
    {
        public LogMessage(StageTypes stage, string message, LogLevelTypes logLevel)
        {
            Timestamp = DateTime.UtcNow.ToString("O");
            Stage = stage.ToString();
            Message = message;
            LogLevel = logLevel.ToString();
        }
        
        public string Timestamp { get; }
        
        public string Stage { get; }

        public string LogLevel { get; }
        
        public string Message { get; }
    }
}
