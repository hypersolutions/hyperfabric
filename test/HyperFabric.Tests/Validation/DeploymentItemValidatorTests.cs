using System.IO;
using System.Linq;
using HyperFabric.Core;
using HyperFabric.Tests.Builders;
using HyperFabric.Validation;
using Shouldly;
using Xunit;

namespace HyperFabric.Tests.Validation
{
    public class DeploymentItemValidatorTests
    {
        private readonly DeploymentItemValidator _validator = new DeploymentItemValidator();
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void InvalidPackagePath_Validate_ReturnsSuccessAsFalse(string path)
        {
            var manifest = TestManifestBuilder
                .From("http://localhost:19080")
                .WithGroup(new DeploymentItem{PackagePath = path})
                .Build();

            var result = _validator.Validate(manifest);
            
            result.Success.ShouldBeFalse();
        }
        
        [Fact]
        public void UnknownPackagePath_Validate_ReturnsSuccessAsFalse()
        {
            var packagePath = Path.Combine(TestInfo.OutputDir, "TestPackage");
            var manifest = TestManifestBuilder
                .From("http://localhost:19080")
                .WithGroup(new DeploymentItem{PackagePath = packagePath})
                .Build();

            var result = _validator.Validate(manifest);
            
            result.Success.ShouldBeFalse();
        }
        
        [Fact]
        public void UnknownPackagePath_Validate_ReturnsError()
        {
            var packagePath = Path.Combine(TestInfo.OutputDir, "TestPackage");
            var manifest = TestManifestBuilder
                .From("http://localhost:19080")
                .WithGroup(new DeploymentItem{PackagePath = packagePath})
                .Build();

            var result = _validator.Validate(manifest);
            
            var error = result.Errors.FirstOrDefault(
                e => e.Property == "PackagePath" && 
                     e.Error == "The path to the package cannot be found or has not been provided.");
            error.ShouldNotBeNull();
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void InvalidParameterFile_Validate_ReturnsSuccessAsFalse(string file)
        {
            var packagePath = Path.Combine(TestInfo.OutputDir, "Support", "Packages", "App1");
            var manifest = TestManifestBuilder
                .From("http://localhost:19080")
                .WithGroup(new DeploymentItem{PackagePath = packagePath, ParameterFile = file})
                .Build();

            var result = _validator.Validate(manifest);
            
            result.Success.ShouldBeFalse();
        }
        
        [Fact]
        public void UnknownParameterFile_Validate_ReturnsSuccessAsFalse()
        {
            var packagePath = Path.Combine(TestInfo.OutputDir, "Support", "Packages", "App1");
            var parameterFile = Path.Combine(packagePath, "Parameters", "Local.1Node.xml");
            var manifest = TestManifestBuilder
                .From("http://localhost:19080")
                .WithGroup(new DeploymentItem{PackagePath = packagePath, ParameterFile = parameterFile})
                .Build();

            var result = _validator.Validate(manifest);
            
            result.Success.ShouldBeFalse();
        }
        
        [Fact]
        public void UnknownParameterFile_Validate_ReturnsError()
        {
            var packagePath = Path.Combine(TestInfo.OutputDir, "Support", "Packages", "App1");
            var parameterFile = Path.Combine(packagePath, "Parameters", "Local.1Node.xml");
            var manifest = TestManifestBuilder
                .From("http://localhost:19080")
                .WithGroup(new DeploymentItem{PackagePath = packagePath, ParameterFile = parameterFile})
                .Build();

            var result = _validator.Validate(manifest);
            
            var error = result.Errors.FirstOrDefault(
                e => e.Property == "ParameterFile" && 
                     e.Error == "The parameter file cannot be found or has not been provided.");
            error.ShouldNotBeNull();
        }
        
        [Fact]
        public void UnknownApplicationManifestFile_Validate_ReturnsSuccessAsFalse()
        {
            var packagePath = Path.Combine(TestInfo.OutputDir, "Support", "Packages", "Empty");
            var parameterFile = Path.Combine(packagePath, "Parameters", "Cloud.xml");
            var manifest = TestManifestBuilder
                .From("http://localhost:19080")
                .WithGroup(new DeploymentItem{PackagePath = packagePath, ParameterFile = parameterFile})
                .Build();

            var result = _validator.Validate(manifest);
            
            result.Success.ShouldBeFalse();
        }
        
        [Fact]
        public void UnknownApplicationManifestFile_Validate_ReturnsError()
        {
            var packagePath = Path.Combine(TestInfo.OutputDir, "Support", "Packages", "Empty");
            var parameterFile = Path.Combine(packagePath, "Parameters", "Cloud.xml");
            var manifest = TestManifestBuilder
                .From("http://localhost:19080")
                .WithGroup(new DeploymentItem{PackagePath = packagePath, ParameterFile = parameterFile})
                .Build();

            var result = _validator.Validate(manifest);
            
            var error = result.Errors.FirstOrDefault(
                e => e.Property == "ApplicationManifestFile" && 
                     e.Error == "Unable to find the ApplicationManifest.xml file inside the provided PackagePath.");
            error.ShouldNotBeNull();
        }
        
        [Fact]
        public void ParameterFileWithNoApplicationName_Validate_ReturnsSuccessAsFalse()
        {
            var packagePath = Path.Combine(TestInfo.OutputDir, "Support", "Packages", "App1");
            var parameterFile = Path.Combine(packagePath, "Parameters", "InvalidName.xml");
            var manifest = TestManifestBuilder
                .From("http://localhost:19080")
                .WithGroup(new DeploymentItem{PackagePath = packagePath, ParameterFile = parameterFile})
                .Build();

            var result = _validator.Validate(manifest);
            
            result.Success.ShouldBeFalse();
        }
        
        [Fact]
        public void ParameterFileWithNoApplicationName_Validate_ReturnsError()
        {
            var packagePath = Path.Combine(TestInfo.OutputDir, "Support", "Packages", "App1");
            var parameterFile = Path.Combine(packagePath, "Parameters", "InvalidName.xml");
            var manifest = TestManifestBuilder
                .From("http://localhost:19080")
                .WithGroup(new DeploymentItem{PackagePath = packagePath, ParameterFile = parameterFile})
                .Build();

            var result = _validator.Validate(manifest);
            
            var error = result.Errors.FirstOrDefault(
                e => e.Property == "ApplicationName" && 
                     e.Error == "Unable to find the application name in the parameters file.");
            error.ShouldNotBeNull();
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void InvalidPackagePathType_Validate_ReturnsSuccessAsFalse(string packagePath)
        {
            var manifest = TestManifestBuilder
                .From("http://localhost:19080")
                .WithGroup(new DeploymentItem{PackagePath = packagePath})
                .Build();

            var result = _validator.Validate(manifest);
            
            result.Success.ShouldBeFalse();
        }
        
        [Fact]
        public void InvalidPackagePathType_Validate_ReturnsError()
        {
            var manifest = TestManifestBuilder
                .From("http://localhost:19080")
                .WithGroup(new DeploymentItem{PackagePath = null})
                .Build();

            var result = _validator.Validate(manifest);
            
            var error = result.Errors.FirstOrDefault(
                e => e.Property == "ApplicationType" && 
                     e.Error == "Unable to find the application type in the application manifest file.");
            error.ShouldNotBeNull();
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void InvalidPackagePathVersion_Validate_ReturnsSuccessAsFalse(string packagePath)
        {
            var manifest = TestManifestBuilder
                .From("http://localhost:19080")
                .WithGroup(new DeploymentItem{PackagePath = packagePath})
                .Build();

            var result = _validator.Validate(manifest);
            
            result.Success.ShouldBeFalse();
        }
        
        [Fact]
        public void InvalidPackagePathVersion_Validate_ReturnsError()
        {
            var manifest = TestManifestBuilder
                .From("http://localhost:19080")
                .WithGroup(new DeploymentItem{PackagePath = null})
                .Build();

            var result = _validator.Validate(manifest);
            
            var error = result.Errors.FirstOrDefault(
                e => e.Property == "ApplicationVersion" && 
                     e.Error == "Unable to find the application version in the application manifest file.");
            error.ShouldNotBeNull();
        }
    }
}
