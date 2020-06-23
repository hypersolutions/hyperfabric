using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HyperFabric.Core;
using HyperFabric.Integration.Tests.Builders;
using Shouldly;
using Xunit;

namespace HyperFabric.Integration.Tests
{
    public class DeploymentServiceLogTests : TestBase
    {
        private readonly string _outputPath = Path.Combine(TestInfo.OutputDir, "out.json");
        
        [Fact]
        public async Task SinglePackage_RunAsync_LogContainsNoErrors()
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
            
                await DeploymentService.RunAsync(manifest, new[]{"File"});

                var log = OutputParser.Parse(_outputPath);
                ShouldContainNoErrors(log);
            }
            finally
            {
                await TearDownAsync(packages);
            }
        }
        
        [Fact]
        public async Task SinglePackage_RunAsync_LogContainsPreparationMessages()
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
            
                await DeploymentService.RunAsync(manifest, new[]{"File"});

                var log = OutputParser.Parse(_outputPath);
                ShouldContainPreparationMessage(log, "Processing the manifest");
                ShouldContainPreparationMessage(log, "Populating the manifest with calculated values");
                ShouldContainPreparationMessage(log, "About to run the deployment on the manifest...");
                ShouldContainPreparationMessage(log, "Created a fabric client to connect to the cluster");
                ShouldContainPreparationMessage(log, "Checking the connection to the cluster...");
                ShouldContainPreparationMessage(log, "Connection to the cluster succeeded");
                ShouldContainPreparationMessage(log, "Checking the cluster is healthy...");
                ShouldContainPreparationMessage(log, "Cluster is healthy");
            }
            finally
            {
                await TearDownAsync(packages);
            }
        }
        
        [Fact]
        public async Task SinglePackage_RunAsync_LogContainsDeploymentMessages()
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
            
                await DeploymentService.RunAsync(manifest, new[]{"File"});
                
                var log = OutputParser.Parse(_outputPath);
                ShouldContainDeploymentMessage(log, "Coping packages to the local working directory...");
                ShouldContainDeploymentMessage(log, "Copy package App1 locally succeeded");
                ShouldContainDeploymentMessage(log, "Removing application type App1Type...");
                ShouldContainDeploymentMessage(log, "Removed application type App1Type successfully");
                ShouldContainDeploymentMessage(log, "Copying package App1 to the image store...");
                ShouldContainDeploymentMessage(log, "Copied the package App1 to the image store");
                ShouldContainDeploymentMessage(log, "Creating application type App1Type...");
                ShouldContainDeploymentMessage(log, "Created application type App1Type successfully");
                ShouldContainDeploymentMessage(log, 
                    "Checking to see if application fabric:/app1 requires creation or upgrade...");
                ShouldContainDeploymentMessage(log, "Creating application fabric:/app1...");
                ShouldContainDeploymentMessage(log, "Created application fabric:/app1 successfully");
            }
            finally
            {
                await TearDownAsync(packages);
            }
        }
        
        [Fact]
        public async Task SinglePackage_RunAsync_LogContainsCleanupMessages()
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
            
                await DeploymentService.RunAsync(manifest, new[]{"File"});

                var log = OutputParser.Parse(_outputPath);
                ShouldContainCleanupMessage(log, "Checking the cluster is healthy...");
                ShouldContainCleanupMessage(log, "Cluster is healthy");
                ShouldContainCleanupMessage(log, "Local folder has been cleaned");
            }
            finally
            {
                await TearDownAsync(packages);
            }
        }

        [Fact]
        public async Task PackageAlreadyExists_RunAsync_LogContainsDeploymentErrorMessage()
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
                        ParameterFile = packages[0].TestParameterPath
                    })
                    .Build();
            
                var succeeded = await DeploymentService.RunAsync(manifest, new[]{"File"});
                succeeded.ShouldBeTrue();
                
                manifest = TestManifestBuilder
                    .From(TestInfo.Connection)
                    .WithGroup(new DeploymentItem
                    {
                        PackagePath = packages[0].TestPackagePath,
                        ParameterFile = packages[0].TestParameterPath
                    })
                    .Build();
                
                succeeded = await DeploymentService.RunAsync(manifest, new[]{"File"});
                succeeded.ShouldBeFalse();
                
                var log = OutputParser.Parse(_outputPath);
                ShouldContainDeploymentMessage(
                    log, "Failed to create application type App1Type: Application type and version already exists");
            }
            finally
            {
                await TearDownAsync(packages);
            }
        }
        
        [Fact]
        public async Task UpgradePackage_RunAsync_LogContainsUpgradeDeploymentMessage()
        {
            var packages = new[]
            {
                new PackageInfo("App1Type", "1.0.0", "fabric:/app1"),
                new PackageInfo("App1Type", "1.0.1", "fabric:/app1")
            };
            
            try
            {
                await SetupAsync(packages);
            
                var manifest = TestManifestBuilder
                    .From(TestInfo.Connection)
                    .WithGroup(new DeploymentItem
                    {
                        PackagePath = packages[0].TestPackagePath,
                        ParameterFile = packages[0].TestParameterPath
                    })
                    .WithGroup(new DeploymentItem
                    {
                        PackagePath = packages[1].TestPackagePath,
                        ParameterFile = packages[1].TestParameterPath,
                        MaxApplicationReadyWaitTime = 60
                    })
                    .Build();
            
                var succeeded = await DeploymentService.RunAsync(manifest, new[]{"File"});
                succeeded.ShouldBeTrue();
                
                var log = OutputParser.Parse(_outputPath);
                ShouldContainDeploymentMessage(log, "Upgrading application fabric:/app1 to version 1.0.1...");
                ShouldContainDeploymentMessage(log, "Upgraded application fabric:/app1 successfully");
            }
            finally
            {
                await TearDownAsync(packages);
            }
        }
        
        // Need to review this as the app name changes!
        
        /*[Fact]
        public async Task ConfigUpgradePackage_RunAsync_LogContainsUpgradeDeploymentMessage()
        {
            var packages = new[]
            {
                new PackageInfo("App1Type", "1.0.0", "fabric:/app1"),
                new PackageInfo("App1Type", "1.0.1", "fabric:/app1", "1.0.1", "1.0.0", "1.0.1")
            };
            
            try
            {
                await SetupAsync(packages);
            
                var manifest = TestManifestBuilder
                    .From(TestInfo.Connection)
                    .WithGroup(new DeploymentItem
                    {
                        PackagePath = packages[0].TestPackagePath,
                        ParameterFile = packages[0].TestParameterPath
                    })
                    .WithGroup(new DeploymentItem
                    {
                        PackagePath = packages[1].TestPackagePath,
                        ParameterFile = packages[1].TestParameterPath,
                        MaxApplicationReadyWaitTime = 60
                    })
                    .Build();
            
                var succeeded = await DeploymentService.RunAsync(manifest, new[]{"File"});
                succeeded.ShouldBeTrue();
                
                var log = OutputParser.Parse(_outputPath);
                ShouldContainDeploymentMessage(log, "Upgrading application fabric:/app1 to version 1.0.1...");
                ShouldContainDeploymentMessage(log, "Upgraded application fabric:/app1 successfully");
            }
            finally
            {
                await TearDownAsync(packages);
            }
        }*/
        
        private static void ShouldContainNoErrors(IEnumerable<TestLogMessage> log)
        {
            log.All(l => l.LogLevel == "Ok").ShouldBeTrue();
        }
        
        private static void ShouldContainPreparationMessage(IEnumerable<TestLogMessage> log, string message)
        {
            log.Count(l => l.Stage == "Preparation" && l.Message == message).ShouldBe(1);
        }
        
        private static void ShouldContainDeploymentMessage(IEnumerable<TestLogMessage> log, string message)
        {
            log.Count(l => l.Stage == "Deployment" && l.Message == message).ShouldBe(1);
        }
        
        private static void ShouldContainCleanupMessage(IEnumerable<TestLogMessage> log, string message)
        {
            log.Count(l => l.Stage == "Cleanup" && l.Message == message).ShouldBe(1);
        }
    }
}
