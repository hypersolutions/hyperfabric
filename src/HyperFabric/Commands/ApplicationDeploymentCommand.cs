using System;
using System.Threading.Tasks;
using HyperFabric.Logging;

namespace HyperFabric.Commands
{
    public sealed class ApplicationDeploymentCommand : Command, ICommand
    {
        private readonly CommandContext _context;
        private readonly ICommand _createCommand;
        private readonly ICommand _upgradeCommand;
        
        public ApplicationDeploymentCommand(CommandContext context, ICommand createCommand, ICommand upgradeCommand) 
            : base(context.Logger)
        {
            _context = context;
            _createCommand = createCommand;
            _upgradeCommand = upgradeCommand;
        }

        public async Task RunAsync()
        {
            var item = _context.CurrentDeploymentItem;
            
            try
            {
                LogInfo(
                    StageTypes.Deployment, 
                    $"Checking to see if application {item.ApplicationName} requires creation or upgrade...");
                
                var appInfo = await _context.AppClient.GetApplicationInfoAsync(item.ApplicationId);
                
                if (appInfo != null && appInfo.TypeVersion == item.ApplicationTypeVersion)
                {
                    LogInfo(StageTypes.Deployment, $"Ignoring application {item.ApplicationName} as version matches");
                }
                else if (appInfo != null && appInfo.TypeVersion != item.ApplicationTypeVersion)
                {
                    await _upgradeCommand.RunAsync();
                }
                else
                {
                    await _createCommand.RunAsync();
                }
            }
            catch (Exception error)
            {
                LogError(
                    StageTypes.Deployment, 
                    $"Failed to determine if application {item.ApplicationName} " +
                    $"should be created or upgraded: {error.Message}");
            }
        }
    }
}
