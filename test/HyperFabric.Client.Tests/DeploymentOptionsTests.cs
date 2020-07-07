using Shouldly;
using Xunit;

namespace HyperFabric.Client.Tests
{
    public class DeploymentOptionsTests
    {
        [Fact]
        public void JsonAsPath_JsonPath_ReturnsValue()
        {
            var options = new DeploymentOptions {Json = @"c:\temp"};
            
            options.JsonPath.ShouldBe(@"c:\temp");
        }
        
        [Fact]
        public void JsonAsPath_JsonString_ReturnsNull()
        {
            var options = new DeploymentOptions {Json = @"c:\temp"};
            
            options.JsonString.ShouldBeNull();
        }
        
        [Fact]
        public void JsonAsString_JsonString_ReturnsValue()
        {
            var options = new DeploymentOptions {Json = "{}"};
            
            options.JsonString.ShouldBe("{}");
        }
        
        [Fact]
        public void JsonAsString_JsonPath_ReturnsNull()
        {
            var options = new DeploymentOptions {Json = "{}"};
            
            options.JsonPath.ShouldBeNull();
        }
    }
}
