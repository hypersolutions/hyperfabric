using System.Text.Json;
using HyperFabric.Core.Converters;
using Microsoft.ServiceFabric.Common;
using Shouldly;
using Xunit;

namespace HyperFabric.Tests.Core.Converters
{
    public class UpgradeModeJsonConverterTests
    {
        [Theory]
        [InlineData("UnmonitoredAuto")]
        [InlineData("unmonitoredauto")]
        [InlineData("UNMONITOREDAUTO")]
        public void KnownUpgradeMode_Read_ReturnsEnumValue(string mode)
        {
            var options = new JsonSerializerOptions {PropertyNameCaseInsensitive = true};
            options.Converters.Add(new UpgradeModeJsonConverter());

            var result = JsonSerializer.Deserialize<TestClass>($"{{\"mode\": \"{mode}\"}}", options);
            
            result.Mode.ShouldBe(UpgradeMode.UnmonitoredAuto);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("Unknown")]
        public void UnknownUpgradeMode_Read_ReturnsDefaultValue(string mode)
        {
            var options = new JsonSerializerOptions {PropertyNameCaseInsensitive = true};
            options.Converters.Add(new UpgradeModeJsonConverter());

            var result = JsonSerializer.Deserialize<TestClass>($"{{\"mode\": \"{mode}\"}}", options);
            
            result.Mode.ShouldBe(UpgradeMode.UnmonitoredAuto);
        }
        
        [Fact]
        public void FromTestClass_Write_SetsMode()
        {
            var options = new JsonSerializerOptions {PropertyNameCaseInsensitive = true};
            options.Converters.Add(new UpgradeModeJsonConverter());

            var result = JsonSerializer.Serialize(new TestClass{Mode = UpgradeMode.Monitored}, options);
            
            result.ShouldBe("{\"Mode\":\"Monitored\"}");
        }

        private class TestClass
        {
            public UpgradeMode Mode { get; set; }
        }
    }
}
