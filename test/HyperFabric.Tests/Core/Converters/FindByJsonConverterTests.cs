using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using HyperFabric.Core.Converters;
using Shouldly;
using Xunit;
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace HyperFabric.Tests.Core.Converters
{
    public class X509FindTypeJsonConverterTests
    {
        [Theory]
        [InlineData("FindBySubjectName")]
        [InlineData("SubjectName")]
        [InlineData("findbysubjectname")]
        [InlineData("subjectname")]
        [InlineData("FINDBYSUBJECTNAME")]
        [InlineData("SUBJECTNAME")]
        public void KnownX509FindType_Read_ReturnsEnumValue(string findOption)
        {
            var options = new JsonSerializerOptions {PropertyNameCaseInsensitive = true};
            options.Converters.Add(new X509FindTypeJsonConverter());

            var result = JsonSerializer.Deserialize<TestClass>($"{{\"findType\": \"{findOption}\"}}", options);
            
            result.FindType.ShouldBe(X509FindType.FindBySubjectName);
        }

        [Fact]
        public void UnknownX509FindType_Read_ReturnsDefaultValue()
        {
            var options = new JsonSerializerOptions {PropertyNameCaseInsensitive = true};
            options.Converters.Add(new X509FindTypeJsonConverter());

            var result = JsonSerializer.Deserialize<TestClass>("{\"findType\": \"FindByName\"}", options);
            
            result.FindType.ShouldBe(X509FindType.FindByThumbprint);
        }

        [Fact]
        public void FromTestClass_Write_SetsFindType()
        {
            var options = new JsonSerializerOptions {PropertyNameCaseInsensitive = true};
            options.Converters.Add(new X509FindTypeJsonConverter());

            var result = JsonSerializer.Serialize(new TestClass{FindType = X509FindType.FindByIssuerName}, options);
            
            result.ShouldBe("{\"FindType\":\"FindByIssuerName\"}");
        }
        
        private class TestClass
        {
            public X509FindType FindType { get; set; }
        }
    }
}
