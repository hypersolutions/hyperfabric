using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HyperFabric.Commands;
using HyperFabric.Core;
using HyperFabric.Logging;

namespace HyperFabric.Tasks
{
    public class DeploymentItemConcurrentTask : ConcurrentTasks<DeploymentItem>
    {
        private readonly CommandContext _context;

        public DeploymentItemConcurrentTask(CommandContext context, IEnumerable<DeploymentItem> items, int taskCount) 
            : base(items, taskCount)
        {
            _context = context;
        }
        
        protected override async Task HandleItemAsync(DeploymentItem item, int taskId)
        {
            var command = CreateCommandChain(item);
            await command.RunAsync();
        }

        protected override Task HandleErrorAsync(DeploymentItem item, Exception error)
        {
            var message = $"Unexpected error occurred: {error.Message}";
            _context.Logger.Log(new LogMessage(StageTypes.Deployment, message, LogLevelTypes.Error));
            return Task.CompletedTask;
        }

        private ICommand CreateCommandChain(DeploymentItem item)
        {
            var itemContext = CreateContext(item);

            ICommand command;
            
            if (item.RemoveApplicationFirst)
            {
                var command6 = new UpgradeApplicationCommand(itemContext);
                var command5 = new CreateApplicationCommand(itemContext);
                var command4 = new ApplicationDeploymentCommand(itemContext, command5, command6);
                var command3 = new CreateApplicationTypeCommand(itemContext, command4);
                var command2 = new CopyImageToStoreCommand(itemContext, command3);
                command = new RemoveApplicationTypeCommand(itemContext, command2);
            }
            else
            {
                var command5 = new UpgradeApplicationCommand(itemContext);
                var command4 = new CreateApplicationCommand(itemContext);
                var command3 = new ApplicationDeploymentCommand(itemContext, command4, command5);
                var command2 = new CreateApplicationTypeCommand(itemContext, command3);
                command = new CopyImageToStoreCommand(itemContext, command2);
            }

            return command;
        }

        private CommandContext CreateContext(DeploymentItem item)
        {
            return new CommandContext
            {
                FabricClient = _context.FabricClient,
                Logger = _context.Logger,
                CurrentDeploymentItem = item
            };
        }
    }
}
