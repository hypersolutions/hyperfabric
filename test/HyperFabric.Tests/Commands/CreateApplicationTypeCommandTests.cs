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
    public class CreateApplicationTypeCommandTests : CommandTestBase
    {
        private readonly Mock<IApplicationTypeClient> _appTypeClient;
        private readonly Mock<ICommand> _innerCommand;
        private readonly DeploymentItem _item;
        private readonly CreateApplicationTypeCommand _command;
        
        public CreateApplicationTypeCommandTests()
        {
            _appTypeClient = new Mock<IApplicationTypeClient>();
            _innerCommand = new Mock<ICommand>();
            var fabricClient = new Mock<IServiceFabricClient>();
            fabricClient.Setup(c => c.ApplicationTypes).Returns(_appTypeClient.Object);
            _item = new DeploymentItem
            {
                PackagePath = @"c:\temp\pkg", 
                ApplicationTypeName = "AppType", 
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
            _command = new CreateApplicationTypeCommand(context, _innerCommand.Object);
        }

        [Fact]
        public async Task NoErrors_RunAsync_LogsSuccess()
        {
            var appTypeInfoList = new PagedData<ApplicationTypeInfo>(ContinuationToken.Empty, new[]
            {
                new ApplicationTypeInfo("app", "1.0.0", null, ApplicationTypeStatus.Available)
            });
            _appTypeClient
                .Setup(c => c.GetApplicationTypeInfoListByNameAsync(
                    _item.ApplicationTypeName, null, false, null, 0, 60, CancellationToken.None))
                .Returns(Task.FromResult(appTypeInfoList));
            
            await _command.RunAsync();
            
            ShouldContainsLogMessage($"Created application type {_item.ApplicationTypeName} successfully");
        }
        
        [Fact]
        public async Task NotAvailableOnFirstCheck_RunAsync_LogsSuccess()
        {
            _command.AvailableCheckDelay = 500;
            var appTypeInfoList1 = new PagedData<ApplicationTypeInfo>(ContinuationToken.Empty, new[]
            {
                new ApplicationTypeInfo("app", "1.0.0", null, ApplicationTypeStatus.Provisioning)
            });
            var appTypeInfoList2 = new PagedData<ApplicationTypeInfo>(ContinuationToken.Empty, new[]
            {
                new ApplicationTypeInfo("app", "1.0.0", null, ApplicationTypeStatus.Available)
            });
            _appTypeClient
                .SetupSequence(c => c.GetApplicationTypeInfoListByNameAsync(
                    _item.ApplicationTypeName, null, false, null, 0, 60, CancellationToken.None))
                .Returns(Task.FromResult(appTypeInfoList1))
                .Returns(Task.FromResult(appTypeInfoList2));
            
            await _command.RunAsync();
            
            ShouldContainsLogMessage($"Created application type {_item.ApplicationTypeName} successfully");
        }
        
        [Fact]
        public async Task NotAvailableInTimelyManner_RunAsync_LogsFailure()
        {
            _item.MaxApplicationReadyWaitTime = 5;
            _command.AvailableCheckDelay = 5000;
            var appTypeInfoList1 = new PagedData<ApplicationTypeInfo>(ContinuationToken.Empty, new[]
            {
                new ApplicationTypeInfo("app", "1.0.0", null, ApplicationTypeStatus.Provisioning)
            });
            var appTypeInfoList2 = new PagedData<ApplicationTypeInfo>(ContinuationToken.Empty, new[]
            {
                new ApplicationTypeInfo("app", "1.0.0", null, ApplicationTypeStatus.Provisioning)
            });
            _appTypeClient
                .SetupSequence(c => c.GetApplicationTypeInfoListByNameAsync(
                    _item.ApplicationTypeName, null, false, null, 0, 60, CancellationToken.None))
                .Returns(Task.FromResult(appTypeInfoList1))
                .Returns(Task.FromResult(appTypeInfoList2));
            
            await _command.RunAsync();
            
            ShouldContainsLogMessage(
                $"Failed to create application type {_item.ApplicationTypeName}: " + 
                $"The application type {_item.ApplicationTypeName} failed to be available " +
                $"within {_item.MaxApplicationReadyWaitTime} seconds");
        }
        
        [Fact]
        public async Task NotFoundAppTypeInfo_RunAsync_LogsFailure()
        {
            var appTypeInfoList = new PagedData<ApplicationTypeInfo>(
                ContinuationToken.Empty, new ApplicationTypeInfo[0]);
            _appTypeClient
                .Setup(c => c.GetApplicationTypeInfoListByNameAsync(
                    _item.ApplicationTypeName, null, false, null, 0, 60, CancellationToken.None))
                .Returns(Task.FromResult(appTypeInfoList));
            
            await _command.RunAsync();
            
            ShouldContainsLogMessage(
                $"Failed to create application type {_item.ApplicationTypeName}: " + 
                $"The application type {_item.ApplicationTypeName} failed to be available " +
                $"within {_item.MaxApplicationReadyWaitTime} seconds");
        }
        
        [Fact]
        public async Task NotFoundAppTypeInfoSecondPass_RunAsync_LogsFailure()
        {
            _item.MaxApplicationReadyWaitTime = 5;
            _command.AvailableCheckDelay = 5000;
            var appTypeInfoList1 = new PagedData<ApplicationTypeInfo>(ContinuationToken.Empty, new[]
            {
                new ApplicationTypeInfo("app", "1.0.0", null, ApplicationTypeStatus.Provisioning)
            });
            var appTypeInfoList2 = new PagedData<ApplicationTypeInfo>(
                ContinuationToken.Empty, new ApplicationTypeInfo[0]);
            _appTypeClient
                .SetupSequence(c => c.GetApplicationTypeInfoListByNameAsync(
                    _item.ApplicationTypeName, null, false, null, 0, 60, CancellationToken.None))
                .Returns(Task.FromResult(appTypeInfoList1))
                .Returns(Task.FromResult(appTypeInfoList2));
            
            await _command.RunAsync();
            
            ShouldContainsLogMessage(
                $"Failed to create application type {_item.ApplicationTypeName}: " + 
                $"The application type {_item.ApplicationTypeName} failed to be available " +
                $"within {_item.MaxApplicationReadyWaitTime} seconds");
        }
        
        [Fact]
        public async Task WithPackage_RunAsync_CallsInnerCommandRunAsync()
        {
            var appTypeInfoList = new PagedData<ApplicationTypeInfo>(ContinuationToken.Empty, new[]
            {
                new ApplicationTypeInfo("app", "1.0.0", null, ApplicationTypeStatus.Available)
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
                .Setup(c => c.ProvisionApplicationTypeAsync(
                    It.Is<ProvisionApplicationTypeDescription>(
                        d => d.ApplicationTypeBuildPath == _item.ApplicationTypeBuildPath), 60, CancellationToken.None))
                .Throws(new Exception("Connection failed."));
            
            await _command.RunAsync();
            
            ShouldContainsLogMessage(
                $"Failed to create application type {_item.ApplicationTypeName}: Connection failed");
        }
    }
}
