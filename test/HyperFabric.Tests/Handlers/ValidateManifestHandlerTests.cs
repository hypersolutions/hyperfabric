using HyperFabric.Handlers;
using HyperFabric.Logging;
using HyperFabric.Tests.Builders;
using HyperFabric.Validation;
using Moq;
using Shouldly;
using Xunit;

namespace HyperFabric.Tests.Handlers
{
    public class ValidateManifestHandlerTests
    {
        [Fact]
        public void ManifestWithNoErrors_Handle_ReturnsTrue()
        {
            var manifest = TestManifestBuilder.From("http://localhost:19080").Build();
            var validator = new Mock<IValidator>();
            var logger = new Mock<ILogger>();
            validator.Setup(v => v.Validate(manifest)).Returns(new ValidationResult());
            var service = new ValidateManifestHandler(new[] {validator.Object}, logger.Object);

            var success = service.Handle(manifest);
            
            success.ShouldBeTrue();
        }
        
        [Fact]
        public void ManifestWithError_Handle_ReturnsFalse()
        {
            var manifest = TestManifestBuilder.From("http://localhost:19080").Build();
            var validator = new Mock<IValidator>();
            var logger = new Mock<ILogger>();
            var validatorResult = new ValidationResult();
            validatorResult.AddError("Connection", "Invalid format.");
            validator.Setup(v => v.Validate(manifest)).Returns(validatorResult);
            var service = new ValidateManifestHandler(new[] {validator.Object}, logger.Object);

            var success = service.Handle(manifest);
            
            success.ShouldBeFalse();
        }
        
        [Fact]
        public void ManifestWithErrors_Handle_ThrowsException()
        {
            var manifest = TestManifestBuilder.From("http://localhost:19080").Build();
            var validator = new Mock<IValidator>();
            var logger = new Mock<ILogger>();
            var validatorResult = new ValidationResult();
            validatorResult.AddError("Connection", "Invalid format.");
            validatorResult.AddError("Thumbprint", "Invalid format.");
            validator.Setup(v => v.Validate(manifest)).Returns(validatorResult);
            var service = new ValidateManifestHandler(new[] {validator.Object}, logger.Object);

            var success = service.Handle(manifest);
            
            success.ShouldBeFalse();
        }
    }
}
