using System.Linq;
using HyperFabric.Core;
using HyperFabric.Tests.Builders;
using HyperFabric.Validation;
using Shouldly;
using Xunit;

namespace HyperFabric.Tests.Validation
{
    public class DeploymentGroupValidatorTests
    {
        private readonly DeploymentGroupValidator _validator = new DeploymentGroupValidator();
        
        [Fact]
        public void NullItems_Validate_ReturnsSuccessAsFalse()
        {
            var manifest = TestManifestBuilder.From("http://localhost:19080").WithGroupNullItems().Build();

            var result = _validator.Validate(manifest);
            
            result.Success.ShouldBeFalse();
        }
        
        [Fact]
        public void NullItems_Validate_ReturnsError()
        {
            var manifest = TestManifestBuilder.From("http://localhost:19080").WithGroupNullItems().Build();

            var result = _validator.Validate(manifest);
            
            var error = result.Errors.FirstOrDefault(
                e => e.Property == "Items" && 
                     e.Error == "No deployment items for the group have been provided.");
            error.ShouldNotBeNull();
        }
        
        [Fact]
        public void EmptyItems_Validate_ReturnsError()
        {
            var manifest = TestManifestBuilder.From("http://localhost:19080").WithGroup().Build();

            var result = _validator.Validate(manifest);
            
            var error = result.Errors.FirstOrDefault(
                e => e.Property == "Items" && 
                     e.Error == "No deployment items for the group have been provided.");
            error.ShouldNotBeNull();
        }
        
        [Fact]
        public void WithItems_Validate_ReturnsSuccessAsTrue()
        {
            var manifest = TestManifestBuilder.From("http://localhost:19080").WithGroup(new DeploymentItem()).Build();

            var result = _validator.Validate(manifest);
            
            result.Success.ShouldBeTrue();
        }
        
        [Fact]
        public void WithItems_Validate_ReturnsNoErrors()
        {
            var manifest = TestManifestBuilder.From("http://localhost:19080").WithGroup(new DeploymentItem()).Build();

            var result = _validator.Validate(manifest);
            
            result.Errors.ShouldBeEmpty();
        }
    }
}
