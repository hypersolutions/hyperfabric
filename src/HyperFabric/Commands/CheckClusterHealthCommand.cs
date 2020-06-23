using System;
using System.Threading.Tasks;
using HyperFabric.Logging;
using Microsoft.ServiceFabric.Common;

namespace HyperFabric.Commands
{
    public sealed class CheckClusterHealthCommand : Command, ICommand
    {
        private readonly CommandContext _context;
        private readonly ICommand _innerCommand;
        private readonly StageTypes _stage;
        private const int HealthCheckDelayMilliseconds = 5000;
        
        public CheckClusterHealthCommand(CommandContext context, ICommand innerCommand, StageTypes stage) 
            : base(context.Logger)
        {
            _context = context;
            _innerCommand = innerCommand;
            _stage = stage;
        }
        
        internal int HealthCheckDelay { get; set; } = HealthCheckDelayMilliseconds;
        
        public async Task RunAsync()
        {
            try
            {
                var canContinue = !_context.Manifest.Options.CheckClusterHealthWaitTime.HasValue;
                
                if (_context.Manifest.Options.CheckClusterHealthWaitTime.HasValue)
                {
                    LogInfo(_stage, "Checking the cluster is healthy...");
                    
                    var waitTime = _context.Manifest.Options.CheckClusterHealthWaitTime.Value;

                    canContinue = await CheckClusterHealthAsync(waitTime);

                    LogStatus(canContinue, waitTime);
                }
                
                if (canContinue) 
                    await _innerCommand.RunAsync();
            }
            catch (Exception error)
            {
                LogError(_stage, error.Message);
            }
        }

        private async Task<bool> CheckClusterHealthAsync(int waitTime)
        {
            var result = await _context.FabricClient.Cluster.GetClusterHealthChunkAsync();
            var startTime = DateTime.Now;
                    
            while (result.HealthState != HealthState.Ok)
            {
                var currentTime = DateTime.Now;

                if (currentTime > startTime.AddSeconds(waitTime))
                {
                    return false;
                }

                await Task.Delay(HealthCheckDelay);
                result = await _context.FabricClient.Cluster.GetClusterHealthChunkAsync();
            }
                    
            return result.HealthState == HealthState.Ok;
        }
        
        private void LogStatus(bool success, int waitTime)
        {
            if (success)
            {
                LogInfo(_stage, $"Cluster is healthy");
            }
            else
            {
                LogError(_stage, $"The cluster failed to become healthy within {waitTime} seconds");
            }
        }
    }
}
