using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using HyperFabric.Logging;
using HyperTask;

namespace HyperFabric.Tasks
{
    public class LogMessageSingleConsumerTask : SingleConsumerTask<LogMessage>
    {
        private readonly IEnumerable<ILogProvider> _providers;

        public LogMessageSingleConsumerTask(IEnumerable<ILogProvider> providers)
        {
            _providers = providers;
        }

        protected override Task HandleItemAsync(LogMessage item)
        {
            var value = JsonSerializer.Serialize(item);

            foreach (var provider in _providers)
            {
                provider.Log(value);
            }
            
            return Task.CompletedTask;
        }
    }
}
