using HyperFabric.Commands;
using HyperFabric.Core;
using HyperFabric.Logging;
using Microsoft.ServiceFabric.Client;

namespace HyperFabric.Factories
{
    public sealed class DeploymentCommandFactory : IDeploymentCommandFactory
    {
        private readonly IServiceFabricClient _fabricClient;
        private readonly ILogger _logger;

        public DeploymentCommandFactory(IServiceFabricClient fabricClient, ILogger logger)
        {
            _fabricClient = fabricClient;
            _logger = logger;
        }
        
        public ICommand Create(Manifest manifest)
        {
            var manifestContext = new CommandContext
            {
                FabricClient = _fabricClient,
                Logger = _logger,
                Manifest = manifest,
            };

            return new CheckClusterConnectionCommand(
                manifestContext, new CheckClusterHealthCommand(
                    manifestContext, new CopyPackagesLocallyCommand(
                        manifestContext, new DeploymentItemFabricCommand(
                            manifestContext, new CheckClusterHealthCommand(
                                manifestContext, new CleanLocalPackagesCommand
                                    (manifestContext), 
                                StageTypes.Cleanup))), 
                    StageTypes.Preparation));
        }
    }
}
