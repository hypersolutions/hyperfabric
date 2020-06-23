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
    public class RemoveApplicationTypeCommandTests : CommandTestBase
    {
        private readonly Mock<IApplicationClient> _appClient;
        private readonly Mock<IApplicationTypeClient> _appTypeClient;
        private readonly Mock<ICommand> _innerCommand;
        private readonly DeploymentItem _item;
        private readonly RemoveApplicationTypeCommand _command;

        public RemoveApplicationTypeCommandTests()
        {
            _appClient = new Mock<IApplicationClient>();
            _appTypeClient = new Mock<IApplicationTypeClient>();
            _innerCommand = new Mock<ICommand>();
            var fabricClient = new Mock<IServiceFabricClient>();
            fabricClient.Setup(c => c.Applications).Returns(_appClient.Object);
            fabricClient.Setup(c => c.ApplicationTypes).Returns(_appTypeClient.Object);
            _item = new DeploymentItem {ApplicationTypeName = "AppType", ApplicationId = "app"};
            var context = new CommandContext
            {
                FabricClient = fabricClient.Object,
                Logger = Logger.Object,
                CurrentDeploymentItem = _item
            };
            _command = new RemoveApplicationTypeCommand(context, _innerCommand.Object);
        }
        
        [Fact]
        public async Task UnknownApplicationId_RunAsync_DoesNotDeleteExistingApps()
        {
            _appClient
                .Setup(c => c.GetApplicationInfoAsync(
                    _item.ApplicationId, false, 60, CancellationToken.None))
                .Returns(Task.FromResult((ApplicationInfo)null));
            
            await _command.RunAsync();
            
            _appClient.Verify(c => c.DeleteApplicationAsync(
                It.IsAny<string>(), null, 60, CancellationToken.None), Times.Never);
        }
        
        [Fact]
        public async Task KnownApplicationId_RunAsync_DoesNotDeleteExistingApps()
        {
            _appClient
                .Setup(c => c.GetApplicationInfoAsync(
                    _item.ApplicationId, false, 60, CancellationToken.None))
                .Returns(Task.FromResult(new ApplicationInfo("app", "fabric:/app")));
            
            await _command.RunAsync();
            
            _appClient.Verify(c => c.DeleteApplicationAsync("app", null, 60, CancellationToken.None), Times.Once);
        }
        
        [Fact]
        public async Task FromExistingApps_RunAsync_UnprovisionsAllAppVersions()
        {
            var appTypeInfoList = new PagedData<ApplicationTypeInfo>(ContinuationToken.Empty, new[]
            {
                new ApplicationTypeInfo("app", "1.0.1"),
                new ApplicationTypeInfo("app", "1.0.0") 
            });
            _appTypeClient
                .Setup(c => c.GetApplicationTypeInfoListByNameAsync(
                    _item.ApplicationTypeName, null, false, null, 0, 60, CancellationToken.None))
                .Returns(Task.FromResult(appTypeInfoList));
            
            await _command.RunAsync();

            ShouldCallUnprovisionApplicationTypeAsyncOnce("app", "1.0.0");
            ShouldCallUnprovisionApplicationTypeAsyncOnce("app", "1.0.1");
        }
        
        [Fact]
        public async Task NoErrors_RunAsync_LogsSuccess()
        {
            var appTypeInfoList = new PagedData<ApplicationTypeInfo>(ContinuationToken.Empty, new[]
            {
                new ApplicationTypeInfo("app", "1.0.1"),
                new ApplicationTypeInfo("app", "1.0.0") 
            });
            _appTypeClient
                .Setup(c => c.GetApplicationTypeInfoListByNameAsync(
                    _item.ApplicationTypeName, null, false, null, 0, 60, CancellationToken.None))
                .Returns(Task.FromResult(appTypeInfoList));
            
            await _command.RunAsync();
            
            ShouldContainsLogMessage($"Removed application type {_item.ApplicationTypeName} successfully");
        }
        
        [Fact]
        public async Task FromExistingApps_RunAsync_CallsInnerCommandRunAsync()
        {
            var appTypeInfoList = new PagedData<ApplicationTypeInfo>(ContinuationToken.Empty, new[]
            {
                new ApplicationTypeInfo("app", "1.0.1"),
                new ApplicationTypeInfo("app", "1.0.0") 
            });
            _appTypeClient
                .Setup(c => c.GetApplicationTypeInfoListByNameAsync(
                    _item.ApplicationTypeName, null, false, null, 0, 60, CancellationToken.None))
                .Returns(Task.FromResult(appTypeInfoList));
            
            await _command.RunAsync();
            
            _innerCommand.Verify(c => c.RunAsync(), Times.Once);
        }
        
        [Fact]
        public async Task ExceptionRaised_RunAsync_LogsFailure()
        {
            _appTypeClient
                .Setup(c => c.GetApplicationTypeInfoListByNameAsync(
                    _item.ApplicationTypeName, null, false, null, 0, 60, CancellationToken.None))
                .Throws(new Exception("Connection failed."));
            
            await _command.RunAsync();
            
            ShouldContainsLogMessage(
                $"Failed to remove application type {_item.ApplicationTypeName}: Connection failed.");
        }
        
        private void ShouldCallUnprovisionApplicationTypeAsyncOnce(string appTypeName, string version)
        {
            _appTypeClient.Verify(c => c.UnprovisionApplicationTypeAsync(
                appTypeName, It.Is<UnprovisionApplicationTypeDescriptionInfo>(i => i.ApplicationTypeVersion == version), 
                60, CancellationToken.None), Times.Once);
        }
    }
}
