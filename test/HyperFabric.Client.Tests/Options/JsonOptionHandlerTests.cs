using HyperFabric.Client.Options;
using Shouldly;
using Xunit;

namespace HyperFabric.Client.Tests.Options
{
    public class JsonOptionHandlerTests
    {
        [Theory]
        [InlineData("{}")]
        [InlineData(" { } ")]
        [InlineData("{'name': 'Homer Simpson', 'age': 40}")]
        public void JsonString_Handle_SetsJsonString(string json)
        {
            var handler = new JsonOptionHandler();
            var options = new DeploymentOptions {Json = json};
            
            handler.Handle(options);
            
            options.JsonString.ShouldBe(json.Trim());
        }
        
        [Fact]
        public void JsonString_Handle_SetsJsonPath()
        {
            var handler = new JsonOptionHandler();
            var options = new DeploymentOptions {Json = @"c:\temp\pkg"};
            
            handler.Handle(options);
            
            options.JsonPath.ShouldBe(options.Json);
        }
    }
}
