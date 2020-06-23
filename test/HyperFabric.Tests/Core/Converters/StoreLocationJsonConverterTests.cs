using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using HyperFabric.Core.Converters;
using Shouldly;
using Xunit;
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace HyperFabric.Tests.Core.Converters
{
    public class StoreLocationJsonConverterTests
    {
        [Theory]
        [InlineData("LocalMachine")]
        [InlineData("localmachine")]
        [InlineData("LOCALMACHINE")]
        public void KnownLocation_Read_ReturnsEnumValue(string location)
        {
            var options = new JsonSerializerOptions {PropertyNameCaseInsensitive = true};
            options.Converters.Add(new StoreLocationJsonConverter());

            var result = JsonSerializer.Deserialize<TestClass>($"{{\"location\": \"{location}\"}}", options);
            
            result.Location.ShouldBe(StoreLocation.LocalMachine);
        }

        [Fact]
        public void UnknownLocation_Read_ReturnsDefaultValue()
        {
            var options = new JsonSerializerOptions {PropertyNameCaseInsensitive = true};
            options.Converters.Add(new StoreLocationJsonConverter());

            var result = JsonSerializer.Deserialize<TestClass>("{\"location\": \"AnyUser\"}", options);
            
            result.Location.ShouldBe(StoreLocation.CurrentUser);
        }
        
        [Fact]
        public void FromTestClass_Write_SetsLocation()
        {
            var options = new JsonSerializerOptions {PropertyNameCaseInsensitive = true};
            options.Converters.Add(new StoreLocationJsonConverter());

            var result = JsonSerializer.Serialize(new TestClass{Location = StoreLocation.LocalMachine}, options);
            
            result.ShouldBe("{\"Location\":\"LocalMachine\"}");
        }

        private class TestClass
        {
            public StoreLocation Location { get; set; }
        }
    }
}
