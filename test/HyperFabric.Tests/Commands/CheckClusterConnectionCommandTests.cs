using System;
using System.Threading;
using System.Threading.Tasks;
using HyperFabric.Commands;
using Microsoft.ServiceFabric.Client;
using Microsoft.ServiceFabric.Common;
using Moq;
using Xunit;

namespace HyperFabric.Tests.Commands
{
    public class CheckClusterConnectionCommandTests : CommandTestBase
    {
        private readonly CheckClusterConnectionCommand _command;
        
        public CheckClusterConnectionCommandTests()
        {
            var context = new CommandContext {FabricClient = FabricClient.Object, Logger = Logger.Object};
            _command = new CheckClusterConnectionCommand(context, InnerCommand.Object);
        }
        
        [Fact]
        public async Task ConnectionCheckException_RunAsync_LogsFailure()
        {
            var cluster = new Mock<IClusterClient>();
            cluster
                .Setup(c => c.GetClusterHealthChunkAsync(60, CancellationToken.None))
                .Throws(new Exception("Connection failed."));
            FabricClient.Setup(c => c.Cluster).Returns(cluster.Object);

            await _command.RunAsync();

            ShouldContainsLogMessage("Connection failed.");
        }

        [Fact]
        public async Task ConnectionCheckOk_RunAsync_LogsSuccess()
        {
            var cluster = new Mock<IClusterClient>();
            var healthChunkResult = Task.FromResult(new ClusterHealthChunk(HealthState.Ok));
            cluster.Setup(c => c.GetClusterHealthChunkAsync(60, CancellationToken.None)).Returns(healthChunkResult);
            FabricClient.Setup(c => c.Cluster).Returns(cluster.Object);

            await _command.RunAsync();

            ShouldContainsLogMessage("Connection to the cluster succeeded");
        }
        
        [Fact]
        public async Task ConnectionCheckException_RunAsync_NeverInnerCommandRunAsync()
        {
            var cluster = new Mock<IClusterClient>();
            cluster
                .Setup(c => c.GetClusterHealthChunkAsync(60, CancellationToken.None))
                .Throws(new Exception("Connection failed."));
            FabricClient.Setup(c => c.Cluster).Returns(cluster.Object);

            await _command.RunAsync();

            InnerCommand.Verify(c => c.RunAsync(), Times.Never);
        }
        
        [Fact]
        public async Task ValidConnection_RunAsync_CallsInnerCommandRunAsync()
        {
            var cluster = new Mock<IClusterClient>();
            var healthChunkResult = Task.FromResult(new ClusterHealthChunk(HealthState.Ok));
            cluster.Setup(c => c.GetClusterHealthChunkAsync(60, CancellationToken.None)).Returns(healthChunkResult);
            FabricClient.Setup(c => c.Cluster).Returns(cluster.Object);

            await _command.RunAsync();

            InnerCommand.Verify(c => c.RunAsync(), Times.Once);
        }
    }
}
