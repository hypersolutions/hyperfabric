using System.Collections.Generic;
using System.Text.Json;
using HyperFabric.Logging;

namespace HyperFabric.Tasks
{
    public class LogMessageSingleConsumerTask : SingleConsumerTask<LogMessage>
    {
        private readonly IEnumerable<ILogProvider> _providers;

        public LogMessageSingleConsumerTask(IEnumerable<ILogProvider> providers)
        {
            _providers = providers;
        }

        protected override void HandleItem(LogMessage item)
        {
            var value = JsonSerializer.Serialize(item);

            foreach (var provider in _providers)
            {
                provider.Log(value);
            }
        }
    }
}
