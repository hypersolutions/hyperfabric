using System.Threading.Tasks;
using HyperFabric.Core;
using HyperFabric.Integration.Tests.Builders;
using Shouldly;
using Xunit;

namespace HyperFabric.Integration.Tests
{
    public class DeploymentServiceSuccessTests : TestBase
    {
        [Fact]
        public async Task SinglePackage_RunAsync_ReturnsSuccess()
        {
            var packages = new[] {new PackageInfo("App1Type", "1.0.0", "fabric:/app1")};
            
            try
            {
                await SetupAsync(packages);
            
                var manifest = TestManifestBuilder
                    .From(TestInfo.Connection)
                    .WithGroup(new DeploymentItem
                    {
                        PackagePath = packages[0].TestPackagePath,
                        ParameterFile = packages[0].TestParameterPath,
                        RemoveApplicationFirst = true
                    })
                    .Build();
            
                var success = await DeploymentService.RunAsync(manifest, new[]{"File"});
            
                success.ShouldBeTrue();
            }
            finally
            {
                await TearDownAsync(packages);
            }
        }
        
        [Fact]
        public async Task MultiplePackages_RunAsync_ReturnsSuccess()
        {
            var packages = new[]
            {
                new PackageInfo("App1Type", "1.0.0", "fabric:/app1"),
                new PackageInfo("App2Type", "1.0.0", "fabric:/app2"),
                new PackageInfo("App3Type", "1.0.0", "fabric:/app3")
            };
            
            try
            {
                await SetupAsync(packages);

                var manifest = TestManifestBuilder
                    .From(TestInfo.Connection)
                    .WithGroup(new DeploymentItem
                        {
                            PackagePath = packages[0].TestPackagePath,
                            ParameterFile = packages[0].TestParameterPath,
                            RemoveApplicationFirst = true
                        },
                        new DeploymentItem
                        {
                            PackagePath = packages[1].TestPackagePath,
                            ParameterFile = packages[1].TestParameterPath,
                            RemoveApplicationFirst = true
                        },
                        new DeploymentItem
                        {
                            PackagePath = packages[2].TestPackagePath,
                            ParameterFile = packages[2].TestParameterPath,
                            RemoveApplicationFirst = true
                        })
                    .Build();
            
                var success = await DeploymentService.RunAsync(manifest, new[]{"File"});
            
                success.ShouldBeTrue();
            }
            finally
            {
                await TearDownAsync(packages);
            }
        }
    }
}
