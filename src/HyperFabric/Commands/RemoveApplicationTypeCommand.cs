using System;
using System.Threading.Tasks;
using HyperFabric.Logging;
using Microsoft.ServiceFabric.Common;

namespace HyperFabric.Commands
{
    public sealed class RemoveApplicationTypeCommand : Command, ICommand
    {
        private readonly CommandContext _context;
        private readonly ICommand _innerCommand;

        public RemoveApplicationTypeCommand(CommandContext context, ICommand innerCommand) : base(context.Logger)
        {
            _context = context;
            _innerCommand = innerCommand;
        }
        
        public async Task RunAsync()
        {
            var item = _context.CurrentDeploymentItem;
            LogInfo(StageTypes.Deployment, $"Removing application type {item.ApplicationTypeName}...");
            
            try
            {
                await DeleteExistingApplicationAsync();
                await RemoveApplicationTypeAsync();
                LogInfo(StageTypes.Deployment, $"Removed application type {item.ApplicationTypeName} successfully");
                
                await _innerCommand.RunAsync();
            }
            catch (Exception error)
            {
                LogInfo(
                    StageTypes.Deployment,
                    $"Failed to remove application type {item.ApplicationTypeName}: {error.Message}");
            }
        }
        
        private async Task DeleteExistingApplicationAsync()
        {
            var item = _context.CurrentDeploymentItem;
            var existingApplicationInfo = await _context.AppClient.GetApplicationInfoAsync(item.ApplicationId);

            if (existingApplicationInfo != null)
            {
                await _context.AppClient.DeleteApplicationAsync(existingApplicationInfo.Id);
            }
        }

        private async Task RemoveApplicationTypeAsync()
        {
            var item = _context.CurrentDeploymentItem;
            var applicationTypes = await _context.AppTypeClient
                .GetApplicationTypeInfoListByNameAsync(item.ApplicationTypeName);
            
            foreach (var applicationType in applicationTypes.Data)
            {
                var descriptionInfo = new UnprovisionApplicationTypeDescriptionInfo(applicationType.Version, false);
                await _context.AppTypeClient.UnprovisionApplicationTypeAsync(applicationType.Name, descriptionInfo);
            }
        }
    }
}
