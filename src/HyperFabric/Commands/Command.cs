using HyperFabric.Logging;

namespace HyperFabric.Commands
{
    public abstract class Command
    {
        private readonly ILogger _logger;

        protected Command(ILogger logger)
        {
            _logger = logger;
        }

        protected void LogInfo(StageTypes stage, string message) => 
            _logger.Log(new LogMessage(stage, message, LogLevelTypes.Ok));
        
        protected void LogError(StageTypes stage, string message) => 
            _logger.Log(new LogMessage(stage, message, LogLevelTypes.Error));
    }
}
