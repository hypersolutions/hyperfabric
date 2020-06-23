using System;
using System.Threading;
using System.Threading.Tasks;
using HyperFabric.Commands;
using HyperFabric.Core;
using HyperFabric.Logging;
using Microsoft.ServiceFabric.Client;
using Microsoft.ServiceFabric.Common;
using Moq;
using Xunit;

namespace HyperFabric.Tests.Commands
{
    public class CheckClusterHealthCommandTests : CommandTestBase
    {
        private readonly CheckClusterHealthCommand _command;

        public CheckClusterHealthCommandTests()
        {
            var context = new CommandContext
            {
                FabricClient = FabricClient.Object, 
                Logger = Logger.Object, 
                Manifest = new Manifest()
            };
            context.Manifest.Options.CheckClusterHealthWaitTime = 1;
            _command = new CheckClusterHealthCommand(context, InnerCommand.Object, StageTypes.Preparation);
            _command.HealthCheckDelay = 200;
        }
        
        [Fact]
        public async Task HealthyClusterStateFirstTme_RunAsync_LogsSuccess()
        {
            var cluster = new Mock<IClusterClient>();
            var healthChunkResult = Task.FromResult(new ClusterHealthChunk(HealthState.Ok));
            cluster.Setup(c => c.GetClusterHealthChunkAsync(60, CancellationToken.None)).Returns(healthChunkResult);
            FabricClient.Setup(c => c.Cluster).Returns(cluster.Object);

            await _command.RunAsync();

            ShouldContainsLogMessage("Cluster is healthy");
        }
        
        [Fact]
        public async Task HealthyClusterStateSecondTime_RunAsync_LogsSuccess()
        {
            var cluster = new Mock<IClusterClient>();
            var healthChunkResult1 = Task.FromResult(new ClusterHealthChunk(HealthState.Unknown));
            var healthChunkResult2 = Task.FromResult(new ClusterHealthChunk(HealthState.Ok));
            cluster
                .SetupSequence(c => c.GetClusterHealthChunkAsync(60, CancellationToken.None))
                .Returns(healthChunkResult1)
                .Returns(healthChunkResult2);
            FabricClient.Setup(c => c.Cluster).Returns(cluster.Object);

            await _command.RunAsync();

            ShouldContainsLogMessage("Cluster is healthy");
        }

        [Theory]
        [InlineData(HealthState.Error)]
        [InlineData(HealthState.Invalid)]
        [InlineData(HealthState.Unknown)]
        [InlineData(HealthState.Warning)]
        public async Task UnhealthyClusterState_RunAsync_LogsFailure(HealthState status)
        {
            var cluster = new Mock<IClusterClient>();
            var healthChunkResult = Task.FromResult(new ClusterHealthChunk(status));
            cluster.Setup(c => c.GetClusterHealthChunkAsync(60, CancellationToken.None)).Returns(healthChunkResult);
            FabricClient.Setup(c => c.Cluster).Returns(cluster.Object);

            await _command.RunAsync();

            ShouldContainsLogMessage($"The cluster failed to become healthy within 1 seconds");
        }
        
        [Fact]
        public async Task ClusterHealthException_RunAsync_LogsFailure()
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
        public async Task UnhealthyCluster_RunAsync_NeverInnerCommandRunAsync()
        {
            var cluster = new Mock<IClusterClient>();
            var healthChunkResult = Task.FromResult(new ClusterHealthChunk(HealthState.Warning));
            cluster.Setup(c => c.GetClusterHealthChunkAsync(60, CancellationToken.None)).Returns(healthChunkResult);
            FabricClient.Setup(c => c.Cluster).Returns(cluster.Object);

            await _command.RunAsync();

            InnerCommand.Verify(c => c.RunAsync(), Times.Never);
        }
        
        [Fact]
        public async Task HealthyCluster_RunAsync_CallsInnerCommandRunAsync()
        {
            var cluster = new Mock<IClusterClient>();
            var healthChunkResult = Task.FromResult(new ClusterHealthChunk(HealthState.Ok));
            cluster.Setup(c => c.GetClusterHealthChunkAsync(60, CancellationToken.None)).Returns(healthChunkResult);
            FabricClient.Setup(c => c.Cluster).Returns(cluster.Object);

            await _command.RunAsync();

            InnerCommand.Verify(c => c.RunAsync(), Times.Once);
        }
        
        [Fact]
        public async Task NoHealthCheckRequired_RunAsync_CallsInnerCommandRunAsync()
        {
            //_context.Manifest.Options.CheckClusterHealth = false;
            var cluster = new Mock<IClusterClient>();
            var healthChunkResult = Task.FromResult(new ClusterHealthChunk(HealthState.Ok));
            cluster.Setup(c => c.GetClusterHealthChunkAsync(60, CancellationToken.None)).Returns(healthChunkResult);
            FabricClient.Setup(c => c.Cluster).Returns(cluster.Object);

            await _command.RunAsync();

            InnerCommand.Verify(c => c.RunAsync(), Times.Once);
        }
    }
}
