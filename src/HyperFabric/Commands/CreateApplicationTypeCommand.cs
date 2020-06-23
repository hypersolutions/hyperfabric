using System;
using System.Linq;
using System.Threading.Tasks;
using HyperFabric.Core;
using HyperFabric.Logging;
using Microsoft.ServiceFabric.Common;

namespace HyperFabric.Commands
{
    public sealed class CreateApplicationTypeCommand : Command, ICommand
    {
        private readonly CommandContext _context;
        private readonly ICommand _innerCommand;
        private const int AvailableCheckDelayMilliseconds = 5000;

        public CreateApplicationTypeCommand(CommandContext context, ICommand innerCommand) : base(context.Logger)
        {
            _context = context;
            _innerCommand = innerCommand;
        }

        internal int AvailableCheckDelay { get; set; } = AvailableCheckDelayMilliseconds;
        
        public async Task RunAsync()
        {
            var item = _context.CurrentDeploymentItem;
            LogInfo(StageTypes.Deployment, $"Creating application type {item.ApplicationTypeName}...");
            
            try
            {
                var provAppTypeDesc = new ProvisionApplicationTypeDescription(
                    item.ApplicationTypeBuildPath, false, ApplicationPackageCleanupPolicy.Automatic);
                await _context.AppTypeClient.ProvisionApplicationTypeAsync(provAppTypeDesc);

                var created = await CheckCreateApplicationTypeStatusAsync(item);
                
                LogStatus(created);
                
                if (created)
                    await _innerCommand.RunAsync();
            }
            catch (Exception error)
            {
                LogError(
                    StageTypes.Deployment,
                    $"Failed to create application type {item.ApplicationTypeName}: {error.Message}");
            }
        }

        private async Task<bool> CheckCreateApplicationTypeStatusAsync(DeploymentItem item)
        {
            var startTime = DateTime.Now;

            var appTypeInfo = await GetApplicationTypeInfo(item);

            if (appTypeInfo == null)
            {
                return false;
            }
                
            while (appTypeInfo.Status != ApplicationTypeStatus.Available)
            {
                var currentTime = DateTime.Now;

                if (currentTime > startTime.AddSeconds(item.MaxApplicationReadyWaitTime))
                {
                    return false;
                }
                    
                await Task.Delay(AvailableCheckDelay);
                appTypeInfo = await GetApplicationTypeInfo(item);

                if (appTypeInfo == null)
                {
                    return false;
                }
            }

            return appTypeInfo.Status == ApplicationTypeStatus.Available;
        }
        
        private void LogStatus(bool success)
        {
            var item = _context.CurrentDeploymentItem;
            
            if (success)
            {
                LogInfo(StageTypes.Deployment, $"Created application type {item.ApplicationTypeName} successfully");
            }
            else
            {
                var message =
                    $"The application type {item.ApplicationTypeName} failed to be available " +
                    $"within {item.MaxApplicationReadyWaitTime} seconds";
                LogError(
                    StageTypes.Deployment, $"Failed to create application type {item.ApplicationTypeName}: {message}");
            }
        }
        
        private async Task<ApplicationTypeInfo> GetApplicationTypeInfo(DeploymentItem item)
        {
            var appTypeName = item.ApplicationTypeName;
            var appTypeInfoList = await _context.AppTypeClient.GetApplicationTypeInfoListByNameAsync(appTypeName);
            return appTypeInfoList.Data.FirstOrDefault(p => p.Version == item.ApplicationTypeVersion);
        }
    }
}
