using System;
using System.Threading;
using System.Threading.Tasks;
using HyperFabric.Commands;
using HyperFabric.Core;
using Microsoft.ServiceFabric.Client;
using Microsoft.ServiceFabric.Common;
using Moq;
using Xunit;

namespace HyperFabric.Tests.Commands
{
    public class UpgradeApplicationCommandTests : CommandTestBase
    {
        private readonly Mock<IApplicationClient> _appClient;
        private readonly DeploymentItem _item;
        private readonly UpgradeApplicationCommand _command;
        
        public UpgradeApplicationCommandTests()
        {
            _appClient = new Mock<IApplicationClient>();
            var fabricClient = new Mock<IServiceFabricClient>();
            fabricClient.Setup(c => c.Applications).Returns(_appClient.Object);
            _item = new DeploymentItem
            {
                PackagePath = @"c:\temp\pkg", 
                ApplicationTypeName = "AppType", 
                ApplicationName = "fabric:/app",
                ApplicationId = "app",
                ApplicationTypeVersion = "1.0.0",
                ApplicationTypeBuildPath = "pkg"
            };
            var context = new CommandContext
            {
                FabricClient = fabricClient.Object,
                Logger = Logger.Object,
                CurrentDeploymentItem = _item
            };
            _command = new UpgradeApplicationCommand(context);
        }

        [Fact]
        public async Task WithAppInfo_RunAsync_CallsStartApplicationUpgradeAsyncOnce()
        {
            await _command.RunAsync();
            
            _appClient
                .Verify(c => c.StartApplicationUpgradeAsync(_item.ApplicationId, It.Is<ApplicationUpgradeDescription>(
                    d => d.Name == _item.ApplicationName && 
                         d.TargetApplicationTypeVersion == _item.ApplicationTypeVersion), 
                    60, CancellationToken.None), Times.Once);
        }
        
        [Theory]
        [InlineData(UpgradeState.RollingBackCompleted)]
        [InlineData(UpgradeState.RollingForwardCompleted)]
        public async Task NoErrors_RunAsync_LogsSuccess(UpgradeState upgradeState)
        {
            var appInfo = new ApplicationUpgradeProgressInfo(
                _item.ApplicationName,
                _item.ApplicationTypeName,
                _item.ApplicationTypeVersion,
                upgradeState:upgradeState);
            _appClient
                .Setup(c => c.GetApplicationUpgradeAsync(_item.ApplicationId, 60, CancellationToken.None))
                .Returns(Task.FromResult(appInfo));
            
            await _command.RunAsync();

            ShouldContainsLogMessage($"Upgraded application {_item.ApplicationName} successfully");
        }
        
        [Fact]
        public async Task NotAvailableOnFirstCheck_RunAsync_LogsSuccess()
        {
            var appInfo1 = new ApplicationUpgradeProgressInfo(
                _item.ApplicationName,
                _item.ApplicationTypeName,
                _item.ApplicationTypeVersion,
                upgradeState:UpgradeState.RollingForwardInProgress);
            var appInfo2 = new ApplicationUpgradeProgressInfo(
                _item.ApplicationName,
                _item.ApplicationTypeName,
                _item.ApplicationTypeVersion,
                upgradeState:UpgradeState.RollingForwardCompleted);
            _appClient
                .SetupSequence(c => c.GetApplicationUpgradeAsync(_item.ApplicationId, 60, CancellationToken.None))
                .Returns(Task.FromResult(appInfo1))
                .Returns(Task.FromResult(appInfo2));
            
            await _command.RunAsync();
            
            ShouldContainsLogMessage($"Upgraded application {_item.ApplicationName} successfully");
        }
        
        [Fact]
        public async Task NotAvailableInTimelyManner_RunAsync_LogsFailure()
        {
            _item.MaxApplicationReadyWaitTime = 5;
            _command.AvailableCheckDelay = 5000;
            var appInfo1 = new ApplicationUpgradeProgressInfo(
                _item.ApplicationName,
                _item.ApplicationTypeName,
                _item.ApplicationTypeVersion,
                upgradeState:UpgradeState.RollingForwardInProgress);
            var appInfo2 = new ApplicationUpgradeProgressInfo(
                _item.ApplicationName,
                _item.ApplicationTypeName,
                _item.ApplicationTypeVersion,
                upgradeState:UpgradeState.RollingForwardInProgress);
            _appClient
                .SetupSequence(c => c.GetApplicationUpgradeAsync(_item.ApplicationId, 60, CancellationToken.None))
                .Returns(Task.FromResult(appInfo1))
                .Returns(Task.FromResult(appInfo2));

            await _command.RunAsync();
            
            ShouldContainsLogMessage(
                $"Failed to upgrade application {_item.ApplicationName}: " +
                $"The application failed to startup within {_item.MaxApplicationReadyWaitTime} seconds");
        }
        
        [Fact]
        public async Task ExceptionRaised_RunAsync_LogsFailure()
        {
            _appClient
                .SetupSequence(c => c.GetApplicationUpgradeAsync(_item.ApplicationId, 60, CancellationToken.None))
                .Throws(new Exception("Connection failed."));
            
            await _command.RunAsync();
            
            ShouldContainsLogMessage(
                $"Failed to upgrade application {_item.ApplicationName} to " +
                $"version {_item.ApplicationTypeVersion}: Connection failed");
        }
    }
}
