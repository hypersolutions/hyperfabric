using System;
using System.Collections.Generic;
using System.Linq;
using HyperFabric.Logging;

namespace HyperFabric.Factories
{
    internal class LoggingFactory : ILoggingFactory
    {
        private static readonly Dictionary<string, Type> _knownLogProviders = new Dictionary<string, Type>
        {
            {"console", typeof(ConsoleLogProvider)}, 
            {"file", typeof(FileLogProvider)}
        };

        public ILogger Create(string[] names)
        {
            var providers = new List<ILogProvider>();

            foreach (var name in names)
            {
                var logProviderName = name.ToLower();

                if (!_knownLogProviders.ContainsKey(logProviderName)) continue;
                
                var logProviderType = _knownLogProviders[logProviderName];
                var ctor = logProviderType.GetConstructors().First();
                var provider = (ILogProvider) ctor.Invoke(new object[0]);
                providers.Add(provider);
            }

            if (!providers.Any())
            {
                providers.Add(new ConsoleLogProvider());
            }
            
            return new Logger(providers);
        }
    }
}
