using System;
using System.Collections.Generic;

namespace HyperFabric.Logging
{
    public interface ILogger : IDisposable
    {
        bool HasErrors { get; }
        IEnumerable<ILogProvider> Providers { get; }
        
        void Log(params LogMessage[] messages);
    }
}
