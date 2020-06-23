using System;
using System.Linq;
using HyperFabric.Validation;
using Shouldly;
using Xunit;

namespace HyperFabric.Tests.Validation
{
    public class ValidationResultTests
    {
        [Fact]
        public void NoErrors_Success_IsTrue()
        {
            var result = new ValidationResult();
            
            result.Success.ShouldBeTrue();
        }
        
        [Fact]
        public void SingleError_Success_IsFalse()
        {
            var result = new ValidationResult();
            
            result.AddError("Connection", "Invalid format.");
            
            result.Success.ShouldBeFalse();
        }
        
        [Fact]
        public void MultipleErrors_Success_IsFalse()
        {
            var result = new ValidationResult();
            
            result.AddError("Connection", "Invalid format.");
            result.AddError("Thumbprint", "Invalid format.");
            
            result.Success.ShouldBeFalse();
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void InvalidConnection_AddError_ThrowsException(string property)
        {
            var result = new ValidationResult();

            var exception = Should.Throw<ArgumentException>(() => result.AddError(property, "Invalid format."));

            exception.Message.ShouldBe("Property not provided. (Parameter 'property')");
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void InvalidError_AddError_ThrowsException(string error)
        {
            var result = new ValidationResult();

            var exception = Should.Throw<ArgumentException>(() => result.AddError("Connection", error));

            exception.Message.ShouldBe("Error message not provided. (Parameter 'error')");
        }
        
        [Fact]
        public void SingleError_Errors_ContainsError()
        {
            var result = new ValidationResult();
            
            result.AddError("Connection", "Invalid format.");
            
            var error = result.Errors.FirstOrDefault(e => e.Property == "Connection" && e.Error == "Invalid format.");
            error.ShouldNotBeNull();
        }
        
        [Fact]
        public void MultipleErrors_Errors_ContainsErrors()
        {
            var result = new ValidationResult();
            
            result.AddError("Connection", "Invalid format.");
            result.AddError("Thumbprint", "Invalid format.");
            
            var error = result.Errors.FirstOrDefault(e => e.Property == "Connection" && e.Error == "Invalid format.");
            error.ShouldNotBeNull();
            error = result.Errors.FirstOrDefault(e => e.Property == "Thumbprint" && e.Error == "Invalid format.");
            error.ShouldNotBeNull();
        }
    }
}
