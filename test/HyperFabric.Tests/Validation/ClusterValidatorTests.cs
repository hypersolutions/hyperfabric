using System.Linq;
using HyperFabric.Tests.Builders;
using HyperFabric.Validation;
using Shouldly;
using Xunit;

namespace HyperFabric.Tests.Validation
{
    public class ClusterValidatorTests
    {
        private readonly ClusterValidator _validator = new ClusterValidator();
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("http://")]
        [InlineData("https://")]
        [InlineData("http://localhost")]
        [InlineData("https://localhost/19080")]
        [InlineData("https://localhost:19080/test")]
        public void InvalidConnection_Validate_ReturnsSuccessAsFalse(string connection)
        {
            var manifest = TestManifestBuilder.From(connection).Build();

            var result = _validator.Validate(manifest);

            result.Success.ShouldBeFalse();
        }
        
        [Fact]
        public void InvalidConnection_Validate_ReturnsError()
        {
            var manifest = TestManifestBuilder.From("http://localhost").Build();

            var result = _validator.Validate(manifest);

            var error = result.Errors.FirstOrDefault(
                e => e.Property == "Connection" && 
                     e.Error == "The cluster connection is in an incorrect format.");
            error.ShouldNotBeNull();
        }
        
        [Theory]
        [InlineData("http://localhost:19080")]
        [InlineData("https://localhost:19080")]
        public void ValidConnection_Validate_ReturnsSuccessAsTrue(string connection)
        {
            var manifest = TestManifestBuilder.From(connection).Build();

            var result = _validator.Validate(manifest);

            result.Success.ShouldBeTrue();
        }
        
        [Fact]
        public void InvalidThumbprint_Validate_ReturnsSuccessAsFalse()
        {
            var manifest = TestManifestBuilder.From("http://localhost:19080").WithThumbprint(null).Build();

            var result = _validator.Validate(manifest);

            result.Success.ShouldBeFalse();
        }
        
        [Fact]
        public void InvalidThumbprint_Validate_ReturnsError()
        {
            var manifest = TestManifestBuilder.From("http://localhost:19080").WithThumbprint(null).Build();

            var result = _validator.Validate(manifest);

            var error = result.Errors.FirstOrDefault(
                e => e.Property == "FindByValue" && 
                     e.Error == "The find by value has not been provided.");
            error.ShouldNotBeNull();
        }
        
        [Theory]
        [InlineData("c8501a8153092d60533de9a8446d681298453dff")]
        [InlineData("C8501A8153092D60533DE9A8446D681298453DFF")]
        [InlineData("C8 50 1A 81 53 09 2D 60 53 3D E9 A8 44 6D 68 12 98 45 3D FF")]
        [InlineData(" C8501A8153092D60533DE9A8446D681298453DFF ")]
        public void ValidThumbprint_Validate_ReturnsSuccessAsTrue(string thumbprint)
        {
            var manifest = TestManifestBuilder.From("http://localhost:19080").WithThumbprint(thumbprint).Build();

            var result = _validator.Validate(manifest);

            result.Success.ShouldBeTrue();
        }
    }
}
