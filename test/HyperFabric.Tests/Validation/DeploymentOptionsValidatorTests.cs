using System.Linq;
using HyperFabric.Tests.Builders;
using HyperFabric.Validation;
using Shouldly;
using Xunit;

namespace HyperFabric.Tests.Validation
{
    public class DeploymentOptionsValidatorTests
    {
        private readonly DeploymentOptionsValidator _validator = new DeploymentOptionsValidator();

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void InvalidWorkingDirectory_Validate_ReturnsError(string workingDir)
        {
            var manifest = TestManifestBuilder
                .From("http://localhost:19080")
                .WithWorkingDirectory(workingDir)
                .Build();

            var result = _validator.Validate(manifest);
            
            var error = result.Errors.FirstOrDefault(
                e => e.Property == "WorkingDirectory" && 
                     e.Error == "The working directory is invalid or cannot be found.");
            error.ShouldNotBeNull();
        }
        
        [Fact]
        public void UnknownWorkingDirectory_Validate_ReturnsError()
        {
            var manifest = TestManifestBuilder
                .From("http://localhost:19080")
                .WithWorkingDirectory(@"q:\xyz\abc")
                .Build();

            var result = _validator.Validate(manifest);
            
            var error = result.Errors.FirstOrDefault(
                e => e.Property == "WorkingDirectory" && 
                     e.Error == "The working directory is invalid or cannot be found.");
            error.ShouldNotBeNull();
        }
        
        [Fact]
        public void InvalidWorkingDirectory_Validate_ReturnsSuccessAsFalse()
        {
            var manifest = TestManifestBuilder
                .From("http://localhost:19080")
                .WithWorkingDirectory(null)
                .Build();

            var result = _validator.Validate(manifest);
            
            result.Success.ShouldBeFalse();
        }
        
        [Fact]
        public void ValidWorkingDirectory_Validate_ReturnsSuccessAsTrue()
        {
            var manifest = TestManifestBuilder
                .From("http://localhost:19080")
                .WithWorkingDirectory(TestInfo.OutputDir)
                .Build();

            var result = _validator.Validate(manifest);
            
            result.Success.ShouldBeTrue();
        }
        
        [Fact]
        public void DefaultWorkingDirectory_Validate_ReturnsSuccessAsTrue()
        {
            var manifest = TestManifestBuilder.From("http://localhost:19080").Build();

            var result = _validator.Validate(manifest);
            
            result.Success.ShouldBeTrue();
        }
        
        [Theory]
        [InlineData(9)]
        [InlineData(301)]
        public void InvalidClusterHealthWaitTime_Validate_ReturnsError(int waitTime)
        {
            var manifest = TestManifestBuilder
                .From("http://localhost:19080")
                .WithClusterHealthWaitTime(waitTime)
                .Build();

            var result = _validator.Validate(manifest);
            
            var error = result.Errors.FirstOrDefault(
                e => e.Property == "CheckClusterHealthWaitTime" && 
                     e.Error == "The cluster health wait time is out of range: 10 to 300 seconds.");
            error.ShouldNotBeNull();
        }
        
        [Fact]
        public void InvalidClusterHealthWaitTime_Validate_ReturnsSuccessAsFalse()
        {
            var manifest = TestManifestBuilder
                .From("http://localhost:19080")
                .WithClusterHealthWaitTime(0)
                .Build();

            var result = _validator.Validate(manifest);
            
            result.Success.ShouldBeFalse();
        }
    }
}
