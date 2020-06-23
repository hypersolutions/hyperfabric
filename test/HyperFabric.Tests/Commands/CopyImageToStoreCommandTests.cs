using System;
using System.Threading;
using System.Threading.Tasks;
using HyperFabric.Commands;
using HyperFabric.Core;
using Microsoft.ServiceFabric.Client;
using Moq;
using Xunit;

namespace HyperFabric.Tests.Commands
{
    public class CopyImageToStoreCommandTests : CommandTestBase
    {
        private readonly Mock<IImageStoreClient> _imageStoreClient;
        private readonly Mock<ICommand> _innerCommand;
        private readonly DeploymentItem _item;
        private readonly CopyImageToStoreCommand _command;
        
        public CopyImageToStoreCommandTests()
        {
            _imageStoreClient = new Mock<IImageStoreClient>();
            _innerCommand = new Mock<ICommand>();
            var fabricClient = new Mock<IServiceFabricClient>();
            fabricClient.Setup(c => c.ImageStore).Returns(_imageStoreClient.Object);
            _item = new DeploymentItem
            {
                PackagePath = @"c:\temp\pkg", 
                ApplicationTypeName = "AppType", 
                ApplicationId = "app"
            };
            var context = new CommandContext
            {
                FabricClient = fabricClient.Object,
                Logger = Logger.Object,
                CurrentDeploymentItem = _item
            };
            _command = new CopyImageToStoreCommand(context, _innerCommand.Object);
        }
        
        [Fact]
        public async Task WithPackage_RunAsync_CallsUploadApplicationPackageAsyncOnce()
        {
            await _command.RunAsync();
            
            _imageStoreClient.Verify(c =>
                c.UploadApplicationPackageAsync(
                    _item.PackagePath, _item.CompressPackage, null, CancellationToken.None), Times.Once);
        }
        
        [Fact]
        public async Task NoErrors_RunAsync_LogsSuccess()
        {
            await _command.RunAsync();
            
            ShouldContainsLogMessage("Copied the package pkg to the image store");
        }
        
        [Fact]
        public async Task WithPackage_RunAsync_CallsInnerCommandRunAsync()
        {
            await _command.RunAsync();
            
            _innerCommand.Verify(c => c.RunAsync(), Times.Once);
        }
        
        [Fact]
        public async Task ExceptionRaised_RunAsync_LogsFailure()
        {
            _imageStoreClient.Setup(c =>
                c.UploadApplicationPackageAsync(
                    _item.PackagePath, _item.CompressPackage, null, CancellationToken.None))
                .Throws(new Exception("Connection failed."));
            
            await _command.RunAsync();
            
            ShouldContainsLogMessage("Failed to copy pkg to the image store: Connection failed");
        }
    }
}
