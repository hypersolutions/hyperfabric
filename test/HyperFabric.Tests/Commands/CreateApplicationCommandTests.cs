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
    public class CreateApplicationCommandTests : CommandTestBase
    {
        private readonly Mock<IApplicationClient> _appClient;
        private readonly DeploymentItem _item;
        private readonly CreateApplicationCommand _command;
        
        public CreateApplicationCommandTests()
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
            _command = new CreateApplicationCommand(context);
        }

        [Fact]
        public async Task WithAppInfo_RunAsync_CallsCreateApplicationAsyncOnce()
        {
            await _command.RunAsync();
            
            _appClient
                .Verify(c => c.CreateApplicationAsync(It.Is<ApplicationDescription>(
                    d => d.Name == _item.ApplicationName && d.TypeName == _item.ApplicationTypeName &&
                         d.TypeVersion == _item.ApplicationTypeVersion), 60, CancellationToken.None), Times.Once);
        }
        
        [Fact]
        public async Task NoErrors_RunAsync_LogsSuccess()
        {
            var appInfo = new ApplicationInfo(
                _item.ApplicationId,
                _item.ApplicationName,
                _item.ApplicationTypeName,
                _item.ApplicationTypeVersion,
                ApplicationStatus.Ready, 
                healthState:HealthState.Ok);
            _appClient
                .Setup(c => c.GetApplicationInfoAsync(_item.ApplicationId, false, 60, CancellationToken.None))
                .Returns(Task.FromResult(appInfo));
            
            await _command.RunAsync();

            ShouldContainsLogMessage($"Created application {_item.ApplicationName} successfully");
        }
        
        [Fact]
        public async Task NotAvailableOnFirstCheck_RunAsync_LogsSuccess()
        {
            var appInfo1 = new ApplicationInfo(
                _item.ApplicationId,
                _item.ApplicationName,
                _item.ApplicationTypeName,
                _item.ApplicationTypeVersion,
                ApplicationStatus.Creating,
                healthState:HealthState.Unknown);
            var appInfo2 = new ApplicationInfo(
                _item.ApplicationId,
                _item.ApplicationName,
                _item.ApplicationTypeName,
                _item.ApplicationTypeVersion,
                ApplicationStatus.Ready,
                healthState:HealthState.Ok);
            _command.AvailableCheckDelay = 500;
            _appClient
                .SetupSequence(c => c.GetApplicationInfoAsync(_item.ApplicationId, false, 60, CancellationToken.None))
                .Returns(Task.FromResult(appInfo1))
                .Returns(Task.FromResult(appInfo2));
            
            await _command.RunAsync();
            
            ShouldContainsLogMessage($"Created application {_item.ApplicationName} successfully");
        }
        
        [Fact]
        public async Task NotAvailableInTimelyManner_RunAsync_LogsFailure()
        {
            _item.MaxApplicationReadyWaitTime = 5;
            _command.AvailableCheckDelay = 5000;
            var appInfo1 = new ApplicationInfo(
                _item.ApplicationId,
                _item.ApplicationName,
                _item.ApplicationTypeName,
                _item.ApplicationTypeVersion,
                ApplicationStatus.Creating,
                healthState:HealthState.Unknown);
            var appInfo2 = new ApplicationInfo(
                _item.ApplicationId,
                _item.ApplicationName,
                _item.ApplicationTypeName,
                _item.ApplicationTypeVersion,
                ApplicationStatus.Creating,
                healthState:HealthState.Unknown);
            _appClient
                .SetupSequence(c => c.GetApplicationInfoAsync(_item.ApplicationId, false, 60, CancellationToken.None))
                .Returns(Task.FromResult(appInfo1))
                .Returns(Task.FromResult(appInfo2));

            await _command.RunAsync();
            
            ShouldContainsLogMessage(
                $"Failed to create application {_item.ApplicationName}: " +
                $"The application failed to startup within {_item.MaxApplicationReadyWaitTime} seconds");
        }
        
        [Fact]
        public async Task ExceptionRaised_RunAsync_LogsFailure()
        {
            _appClient
                .Setup(c => c.GetApplicationInfoAsync(_item.ApplicationId, false, 60, CancellationToken.None))
                .Throws(new Exception("Connection failed"));
            
            await _command.RunAsync();
            
            ShouldContainsLogMessage(
                $"Failed to create application {_item.ApplicationName}: Connection failed");
        }
    }
}
