using System.Xml.Linq;
using HyperFabric.Extensions;
using Shouldly;
using Xunit;

namespace HyperFabric.Tests.Extensions
{
    public class XElementExtensionsTests
    {
        [Fact]
        public void ForUnknownAttribute_GetAttributeValue_ReturnsDefault()
        {
            var element = XElement.Parse("<test id='100'/>");

            var result = element.GetAttributeValue<int>("unknown");
            
            result.ShouldBe(0);
        }
        
        [Fact]
        public void ForInt_GetAttributeValue_ReturnsIntValue()
        {
            var element = XElement.Parse("<test id='100'/>");

            var result = element.GetAttributeValue<int>("id");
            
            result.ShouldBe(100);
        }
        
        [Fact]
        public void ForString_GetAttributeValue_ReturnsStringValue()
        {
            var element = XElement.Parse("<test name='homer'/>");

            var result = element.GetAttributeValue<string>("name");
            
            result.ShouldBe("homer");
        }
    }
}
