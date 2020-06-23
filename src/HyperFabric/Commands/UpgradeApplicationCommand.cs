using System;
using System.Threading.Tasks;
using HyperFabric.Core;
using HyperFabric.Logging;
using Microsoft.ServiceFabric.Common;

namespace HyperFabric.Commands
{
    public sealed class UpgradeApplicationCommand : Command, ICommand
    {
        private readonly CommandContext _context;
        private const int AvailableCheckDelayMilliseconds = 5000;
        
        public UpgradeApplicationCommand(CommandContext context) : base(context.Logger)
        {
            _context = context;
        }

        internal int AvailableCheckDelay { get; set; } = AvailableCheckDelayMilliseconds;
        
        public async Task RunAsync()
        {
            var item = _context.CurrentDeploymentItem;
            
            try
            {
                LogInfo(
                    StageTypes.Deployment, 
                    $"Upgrading application {item.ApplicationName} to version {item.ApplicationTypeVersion}...");
                
                var appUpgradeDesc = new ApplicationUpgradeDescription(
                    item.ApplicationName, 
                    item.ApplicationTypeVersion, 
                    UpgradeKind.Rolling, 
                    item.Parameters, 
                    item.UpgradeMode ?? UpgradeMode.UnmonitoredAuto);
                
                await _context.AppClient.StartApplicationUpgradeAsync(item.ApplicationId, appUpgradeDesc);
                
                var upgraded = await CheckUpgradeApplicationStatusAsync(item);

                if (upgraded)
                {
                    LogInfo(StageTypes.Deployment, $"Upgraded application {item.ApplicationName} successfully");
                }
                else
                {
                    var message = "The application failed to startup within " +
                                  $"{item.MaxApplicationReadyWaitTime} seconds";
                    LogError(StageTypes.Deployment, $"Failed to upgrade application {item.ApplicationName}: {message}");
                }
            }
            catch (Exception error)
            {
                LogError(
                    StageTypes.Deployment, 
                    $"Failed to upgrade application {item.ApplicationName} to version " +
                    $"{item.ApplicationTypeVersion}: {error.Message}");
            }
        }
        
        private async Task<bool> CheckUpgradeApplicationStatusAsync(DeploymentItem item)
        {
            var startTime = DateTime.Now;

            var appInfo = await _context.AppClient.GetApplicationUpgradeAsync(item.ApplicationId);
            var upgraded = appInfo.UpgradeState == UpgradeState.RollingForwardCompleted ||
                           appInfo.UpgradeState == UpgradeState.RollingBackCompleted;
            
            while (!upgraded)
            {
                var currentTime = DateTime.Now;

                if (currentTime > startTime.AddSeconds(item.MaxApplicationReadyWaitTime))
                {
                    return false;
                }

                await Task.Delay(AvailableCheckDelay);
                appInfo = await _context.AppClient.GetApplicationUpgradeAsync(item.ApplicationId);
                upgraded = appInfo.UpgradeState == UpgradeState.RollingForwardCompleted ||
                           appInfo.UpgradeState == UpgradeState.RollingBackCompleted;
            }

            LogInfo(
                StageTypes.Deployment, $"Application {item.ApplicationName} upgrade state is {appInfo.UpgradeState}");

            return true;
        }
    }
}
