using System;
using System.Threading.Tasks;
using HyperFabric.Core;
using HyperFabric.Logging;
using Microsoft.ServiceFabric.Common;

namespace HyperFabric.Commands
{
    public sealed class CreateApplicationCommand : Command, ICommand
    {
        private readonly CommandContext _context;
        private const int AvailableCheckDelayMilliseconds = 5000;
        
        public CreateApplicationCommand(CommandContext context) : base(context.Logger)
        {
            _context = context;
        }

        internal int AvailableCheckDelay { get; set; } = AvailableCheckDelayMilliseconds;
        
        public async Task RunAsync()
        {
            var item = _context.CurrentDeploymentItem;
            
            try
            {
                LogInfo(StageTypes.Deployment, $"Creating application {item.ApplicationName}...");
                var appDesc = new ApplicationDescription(
                    item.ApplicationName,
                    item.ApplicationTypeName,
                    item.ApplicationTypeVersion,
                    item.Parameters);

                await _context.AppClient.CreateApplicationAsync(appDesc);

                var created = await CheckCreateApplicationStatusAsync(item);

                if (created)
                {
                    LogInfo(StageTypes.Deployment, $"Created application {item.ApplicationName} successfully");
                }
                else
                {
                    var message = "The application failed to startup within " +
                                  $"{item.MaxApplicationReadyWaitTime} seconds.";
                    LogError(StageTypes.Deployment, $"Failed to create application {item.ApplicationName}: {message}");
                }
            }
            catch (Exception error)
            {
                LogError(
                    StageTypes.Deployment, $"Failed to create application {item.ApplicationName}: {error.Message}");
            }
        }

        private async Task<bool> CheckCreateApplicationStatusAsync(DeploymentItem item)
        {
            var startTime = DateTime.Now;

            var appInfo = await _context.AppClient.GetApplicationInfoAsync(item.ApplicationId);

            while (appInfo.Status != ApplicationStatus.Ready && appInfo.HealthState != HealthState.Ok)
            {
                var currentTime = DateTime.Now;

                if (currentTime > startTime.AddSeconds(item.MaxApplicationReadyWaitTime))
                {
                    return false;
                }

                await Task.Delay(AvailableCheckDelay);
                appInfo = await _context.AppClient.GetApplicationInfoAsync(item.ApplicationId);
            }

            return appInfo.Status == ApplicationStatus.Ready;
        }
    }
}
