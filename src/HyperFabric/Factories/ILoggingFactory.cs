using HyperFabric.Logging;

namespace HyperFabric.Factories
{
    internal interface ILoggingFactory
    {
        ILogger Create(string[] names);
    }
}
