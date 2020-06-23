using System.Linq;
using HyperFabric.Core;
using HyperFabric.Tests.Builders;
using HyperFabric.Validation;
using Shouldly;
using Xunit;

namespace HyperFabric.Tests.Validation
{
    public class ManifestValidatorTests
    {
        private readonly ManifestValidator _validator = new ManifestValidator();
        
        [Fact]
        public void NullClusterDetails_Validate_ReturnsSuccessAsFalse()
        {
            var result = _validator.Validate(new Manifest());
            
            result.Success.ShouldBeFalse();
        }
        
        [Fact]
        public void NullClusterDetails_Validate_ReturnsError()
        {
            var result = _validator.Validate(new Manifest());
            
            var error = result.Errors.FirstOrDefault(
                e => e.Property == "ClusterDetails" && 
                     e.Error == "No cluster details have been provided.");
            error.ShouldNotBeNull();
        }
        
        [Fact]
        public void NullGroups_Validate_ReturnsSuccessAsFalse()
        {
            var manifest = TestManifestBuilder.From("http://localhost:19080").Build();

            var result = _validator.Validate(manifest);
            
            result.Success.ShouldBeFalse();
        }
        
        [Fact]
        public void NullGroups_Validate_ReturnsError()
        {
            var manifest = TestManifestBuilder.From("http://localhost:19080").Build();

            var result = _validator.Validate(manifest);
            
            var error = result.Errors.FirstOrDefault(
                e => e.Property == "Groups" && 
                     e.Error == "No deployment groups have been provided.");
            error.ShouldNotBeNull();
        }
        
        [Fact]
        public void EmptyGroups_Validate_ReturnsError()
        {
            var manifest = TestManifestBuilder.From("http://localhost:19080").WithEmptyGroups().Build();

            var result = _validator.Validate(manifest);
            
            var error = result.Errors.FirstOrDefault(
                e => e.Property == "Groups" && 
                     e.Error == "No deployment groups have been provided.");
            error.ShouldNotBeNull();
        }
        
        [Fact]
        public void WithGroups_Validate_ReturnsSuccessAsTrue()
        {
            var manifest = TestManifestBuilder.From("http://localhost:19080").WithGroup(new DeploymentItem()).Build();

            var result = _validator.Validate(manifest);
            
            result.Success.ShouldBeTrue();
        }
        
        [Fact]
        public void WithGroups_Validate_ReturnsNoErrors()
        {
            var manifest = TestManifestBuilder.From("http://localhost:19080").WithGroup(new DeploymentItem()).Build();

            var result = _validator.Validate(manifest);
            
            result.Errors.ShouldBeEmpty();
        }
    }
}
