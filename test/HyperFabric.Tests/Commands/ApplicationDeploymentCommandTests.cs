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
    public class ApplicationDeploymentCommandTests : CommandTestBase
    {
        private readonly Mock<IApplicationClient> _appClient;
        private readonly Mock<ICommand> _createCommand;
        private readonly Mock<ICommand> _upgradeCommand;
        private readonly DeploymentItem _item;
        private readonly ApplicationDeploymentCommand _command;

        public ApplicationDeploymentCommandTests()
        {
            _appClient = new Mock<IApplicationClient>();
            _createCommand = new Mock<ICommand>();
            _upgradeCommand = new Mock<ICommand>();
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
            _command = new ApplicationDeploymentCommand(context, _createCommand.Object, _upgradeCommand.Object);
        }

        [Fact]
        public async Task NoExistingApp_RunAsync_CallsCreateCommand()
        {
            _appClient
                .Setup(c => c.GetApplicationInfoAsync(_item.ApplicationId, false, 60, CancellationToken.None))
                .Returns(Task.FromResult((ApplicationInfo) null));

            await _command.RunAsync();
            
            _createCommand.Verify(c => c.RunAsync(), Times.Once);
        }
        
        [Theory]
        [InlineData("0.9.9")]
        [InlineData("1.0.1")]
        public async Task ExistingAppVersionDiffers_RunAsync_CallsUpgradeCommand(string typeVersion)
        {
            _appClient
                .Setup(c => c.GetApplicationInfoAsync(_item.ApplicationId, false, 60, CancellationToken.None))
                .Returns(Task.FromResult(new ApplicationInfo(typeVersion:typeVersion)));

            await _command.RunAsync();
            
            _upgradeCommand.Verify(c => c.RunAsync(), Times.Once);
        }
        
        [Fact]
        public async Task ExistingAppVersionMatches_RunAsync_NeverCallsCommands()
        {
            _appClient
                .Setup(c => c.GetApplicationInfoAsync(_item.ApplicationId, false, 60, CancellationToken.None))
                .Returns(Task.FromResult(new ApplicationInfo(typeVersion:"1.0.0")));

            await _command.RunAsync();
            
            _upgradeCommand.Verify(c => c.RunAsync(), Times.Never);
            _createCommand.Verify(c => c.RunAsync(), Times.Never);
        }
    }
}
