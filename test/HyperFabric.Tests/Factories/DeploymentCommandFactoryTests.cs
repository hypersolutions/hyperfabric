using HyperFabric.Commands;
using HyperFabric.Core;
using HyperFabric.Factories;
using HyperFabric.Tests.Builders;
using Shouldly;
using Xunit;

namespace HyperFabric.Tests.Factories
{
    public class DeploymentCommandFactoryTests : TestBase<DeploymentCommandFactory>
    {
        [Fact]
        public void FromManifest_Create_ReturnsCommand()
        {
            var manifest = TestManifestBuilder
                .From("http://localhost:19080")
                .WithGroup(new DeploymentItem())
                .Build();
            
            var command = Subject.Create(manifest);
            
            command.ShouldBeOfType<CheckClusterConnectionCommand>();
        }
    }
}
