using System.IO;
using System.Linq;
using HyperFabric.Core;
using HyperFabric.Handlers;
using HyperFabric.Tests.Builders;
using Shouldly;
using Xunit;

namespace HyperFabric.Tests.Handlers
{
    public class PopulateManifestHandlerTests : TestBase<PopulateManifestHandler>
    {
        [Fact]
        public void ValidParameterFile_Handle_UpdatesApplicationId()
        {
            var packagePath = Path.Combine(TestInfo.OutputDir, "Support", "Packages", "App1");
            var parameterFile = Path.Combine(packagePath, "Parameters", "Cloud.xml");
            var manifest = TestManifestBuilder
                .From("http://localhost:19080")
                .WithGroup(new DeploymentItem{PackagePath = packagePath, ParameterFile = parameterFile})
                .Build();
            
            Subject.Handle(manifest);
            
            var applicationId = manifest.Groups.First().Items.First().ApplicationId;
            applicationId.ShouldBe("App1");
        }
        
        [Fact]
        public void ValidParameterFile_Handle_UpdatesParameters()
        {
            var packagePath = Path.Combine(TestInfo.OutputDir, "Support", "Packages", "App1");
            var parameterFile = Path.Combine(packagePath, "Parameters", "Cloud.xml");
            var manifest = TestManifestBuilder
                .From("http://localhost:19080")
                .WithGroup(new DeploymentItem{PackagePath = packagePath, ParameterFile = parameterFile})
                .Build();
            
            Subject.Handle(manifest);
            
            var parameters = manifest.Groups.First().Items.First().Parameters;
            parameters.ContainsKey("ConnectionString").ShouldBeTrue();
        }
        
        [Fact]
        public void ValidPackagePath_Handle_UpdatesApplicationManifestFile()
        {
            var packagePath = Path.Combine(TestInfo.OutputDir, "Support", "Packages", "App1");
            var parameterFile = Path.Combine(packagePath, "Parameters", "Cloud.xml");
            var manifest = TestManifestBuilder
                .From("http://localhost:19080")
                .WithGroup(new DeploymentItem{PackagePath = packagePath, ParameterFile = parameterFile})
                .Build();
            
            Subject.Handle(manifest);

            var item = manifest.Groups.First().Items.First();
            var appManifestPath = item.ApplicationManifestFile;
            appManifestPath.ShouldBe(Path.Combine(item.PackagePath, "ApplicationManifest.xml"));
        }
        
        [Fact]
        public void ValidParameterFile_Handle_UpdatesApplicationName()
        {
            var packagePath = Path.Combine(TestInfo.OutputDir, "Support", "Packages", "App1");
            var parameterFile = Path.Combine(packagePath, "Parameters", "Cloud.xml");
            var manifest = TestManifestBuilder
                .From("http://localhost:19080")
                .WithGroup(new DeploymentItem{PackagePath = packagePath, ParameterFile = parameterFile})
                .Build();

            Subject.Handle(manifest);
            
            var applicationName = manifest.Groups.First().Items.First().ApplicationName;
            applicationName.ShouldBe("fabric:/App1");
        }
        
        [Fact]
        public void ValidPackagePath_Handle_UpdatesApplicationTypeName()
        {
            var packagePath = Path.Combine(TestInfo.OutputDir, "Support", "Packages", "App1");
            var parameterFile = Path.Combine(packagePath, "Parameters", "Cloud.xml");
            var manifest = TestManifestBuilder
                .From("http://localhost:19080")
                .WithGroup(new DeploymentItem{PackagePath = packagePath, ParameterFile = parameterFile})
                .Build();

            Subject.Handle(manifest);

            var applicationType = manifest.Groups.First().Items.First().ApplicationTypeName;
            applicationType.ShouldBe("App1Type");
        }
        
        [Fact]
        public void ValidPackagePath_Handle_UpdatesApplicationTypeVersion()
        {
            var packagePath = Path.Combine(TestInfo.OutputDir, "Support", "Packages", "App1");
            var parameterFile = Path.Combine(packagePath, "Parameters", "Cloud.xml");
            var manifest = TestManifestBuilder
                .From("http://localhost:19080")
                .WithGroup(new DeploymentItem{PackagePath = packagePath, ParameterFile = parameterFile})
                .Build();

            Subject.Handle(manifest);

            var applicationVersion = manifest.Groups.First().Items.First().ApplicationTypeVersion;
            applicationVersion.ShouldBe("1.0.0");
        }
    }
}
