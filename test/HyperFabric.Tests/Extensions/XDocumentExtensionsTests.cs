using System.Linq;
using System.Xml.Linq;
using HyperFabric.Extensions;
using Shouldly;
using Xunit;

namespace HyperFabric.Tests.Extensions
{
    public class XDocumentExtensionsTests
    {
        [Fact]
        public void NotExists_FindElement_ReturnsElement()
        {
            var document = XDocument.Parse("<manifest><item id='10'/></manifest>");
            
            var element = document.FindElement("unknown");
            
            element.ShouldBeNull();
        }
        
        [Fact]
        public void Exists_FindElement_ReturnsElement()
        {
            var document = XDocument.Parse("<manifest><item id='10'/></manifest>");

            var element = document.FindElement("item");
            
            element.ShouldNotBeNull();
        }
        
        [Fact]
        public void ExistsNested_FindElement_ReturnsElement()
        {
            var document = XDocument.Parse("<manifest><item id='10'><sub-item>test</sub-item></item></manifest>");

            var element = document.FindElement("sub-item");
            
            element.ShouldNotBeNull();
        }
        
        [Fact]
        public void Exists_FindElements_ReturnsElement()
        {
            var document = XDocument.Parse("<manifest><item id='10'/><item id='20'/></manifest>");

            var elements = document.FindElements("item");
            
            elements.Count().ShouldBe(2);
        }
        
        [Fact]
        public void ExistsNested_FindElements_ReturnsElement()
        {
            var document = XDocument.Parse("<manifest><item><sub-item id='10'/><sub-item id='20'/></item></manifest>");

            var elements = document.FindElements("sub-item");
            
            elements.Count().ShouldBe(2);
        }
    }
}
