using System;

namespace HyperFabric.Logging
{
    public interface ILogProvider : IDisposable
    {
        void Log(string message);
    }
}
