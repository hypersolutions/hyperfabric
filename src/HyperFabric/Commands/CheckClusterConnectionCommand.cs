using System;
using System.Threading.Tasks;
using HyperFabric.Logging;

namespace HyperFabric.Commands
{
    public sealed class CheckClusterConnectionCommand : Command, ICommand
    {
        private readonly CommandContext _context;
        private readonly ICommand _innerCommand;

        public CheckClusterConnectionCommand(CommandContext context, ICommand innerCommand) : base(context.Logger)
        {
            _context = context;
            _innerCommand = innerCommand;
        }
        
        public async Task RunAsync()
        {
            try
            {
                LogInfo(StageTypes.Preparation, "Checking the connection to the cluster...");
                
                await _context.FabricClient.Cluster.GetClusterHealthChunkAsync();
                
                LogInfo(StageTypes.Preparation, "Connection to the cluster succeeded");

                await _innerCommand.RunAsync();
            }
            catch (Exception error)
            {
                LogError(StageTypes.Preparation, $"Connection to the cluster failed with error: {error.Message}");
            }
        }
    }
}
