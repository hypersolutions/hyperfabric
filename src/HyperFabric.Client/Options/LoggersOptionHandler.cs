using System;
using System.Linq;

namespace HyperFabric.Client.Options
{
    public sealed class LoggersOptionHandler : IOptionHandler
    {
        public void Handle(DeploymentOptions deploymentOption)
        {
            deploymentOption.LoggerList =
                deploymentOption.Loggers?
                    .Trim()
                    .Split(",", StringSplitOptions.RemoveEmptyEntries)
                    .Select(l => l.Trim())
                    .ToArray() ?? new string[0];
        }
    }
}
