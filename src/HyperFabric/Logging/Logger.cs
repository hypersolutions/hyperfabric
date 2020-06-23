using System;
using System.Collections.Generic;
using System.Linq;
using HyperFabric.Tasks;

namespace HyperFabric.Logging
{
    public sealed class Logger : ILogger
    {
        private readonly List<ILogProvider> _providers;
        private readonly LogMessageSingleConsumerTask _loggerTask;
        private volatile bool _hasErrors;
        private bool _isDisposed;
        
        public Logger(IEnumerable<ILogProvider> providers)
        {
            _providers = providers.ToList();
            _loggerTask = new LogMessageSingleConsumerTask(_providers);
        }
        
        ~Logger()
        {
            Dispose(false);
        }

        public bool HasErrors => _hasErrors;

        public IEnumerable<ILogProvider> Providers => _providers;
        
        public void Log(params LogMessage[] messages)
        {
            if (!_hasErrors && messages.Any(m => m.LogLevel == LogLevelTypes.Error.ToString()))
            {
                _hasErrors = true;
            }
            
            messages.ToList().ForEach(_loggerTask.Post);
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        private void Dispose(bool disposing)
        {
            if (!_isDisposed && disposing)
            {
                _loggerTask.Dispose();
                _providers.ForEach(p => p.Dispose());
            }
            
            _isDisposed = true;
        }
    }
}
