using System.Threading.Tasks;
using HyperFabric.Tasks;

namespace HyperFabric.Commands
{
    public sealed class DeploymentItemFabricCommand : ICommand
    {
        private readonly CommandContext _manifestContext;
        private readonly ICommand _innerCommand;

        public DeploymentItemFabricCommand(CommandContext manifestContext, ICommand innerCommand)
        {
            _manifestContext = manifestContext;
            _innerCommand = innerCommand;
        }
        
        public async Task RunAsync()
        {
            var numParallelTasks = _manifestContext.Manifest.Options.NumberOfParallelDeployments;
            var context = new CommandContext
            {
                FabricClient = _manifestContext.FabricClient, 
                Logger = _manifestContext.Logger
            };
            
            foreach (var group in _manifestContext.Manifest.Groups)
            {
                var task = new DeploymentItemConcurrentTask(context, group.Items, numParallelTasks);

                await task.StartAsync();
            }

            await _innerCommand.RunAsync();
        }
    }
}
