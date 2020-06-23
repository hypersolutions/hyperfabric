using System.IO;
using HyperFabric.Extensions;
using Shouldly;
using Xunit;

namespace HyperFabric.Tests.Extensions
{
    public class StringExtensionsTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void InvalidPathToFabricFile_GetFabricAttributeValue_ReturnsNull(string path)
        {
            var value = path.GetFabricAttributeValue("ApplicationManifest", "ApplicationTypeName");
            
            value.ShouldBeNull();
        }
        
        [Fact]
        public void UnknownPathToFabricFile_GetFabricAttributeValue_ReturnsNull()
        {
            var path = Path.Combine(TestInfo.OutputDir, "Support", "UnknownManifest.xml");

            var value = path.GetFabricAttributeValue("ApplicationManifest", "ApplicationTypeName");

            value.ShouldBeNull();
        }
        
        [Fact]
        public void UnknownElementInFabricFile_GetFabricAttributeValue_ReturnsNull()
        {
            var path = Path.Combine(TestInfo.OutputDir, "Support", "ApplicationManifest.xml");

            var value = path.GetFabricAttributeValue("Unknown", "ApplicationTypeName");
            
            value.ShouldBeNull();
        }
        
        [Fact]
        public void UnknownAttributeInFabricFile_GetFabricAttributeValue_ReturnsNull()
        {
            var path = Path.Combine(TestInfo.OutputDir, "Support", "ApplicationManifest.xml");

            var value = path.GetFabricAttributeValue("ApplicationManifest", "Unknown");
            
            value.ShouldBeNull();
        }
        
        [Fact]
        public void PathToFabricFile_GetFabricAttributeValue_ReturnsValue()
        {
            var path = Path.Combine(TestInfo.OutputDir, "Support", "ApplicationManifest.xml");

            var value = path.GetFabricAttributeValue("ApplicationManifest", "ApplicationTypeName");
            
            value.ShouldBe("App1");
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void InvalidFabricParameterFile_GetFabricParameterDictionary_ReturnsEmpty(string path)
        {
            var parameters = path.GetFabricParameterDictionary();
            
            parameters.ShouldBeEmpty();
        }
        
        [Fact]
        public void UnknownFabricParameterFile_GetFabricParameterDictionary_ReturnsEmpty()
        {
            var path = Path.Combine(TestInfo.OutputDir, "Support", "Local.1Node.xml");
            
            var parameters = path.GetFabricParameterDictionary();
            
            parameters.ShouldBeEmpty();
        }
        
        [Fact]
        public void FabricParameterFile_GetFabricParameterDictionary_ReturnsEmpty()
        {
            var path = Path.Combine(TestInfo.OutputDir, "Support", "Cloud.xml");
            
            var parameters = path.GetFabricParameterDictionary();
            
            parameters.ContainsKey("ConnectionString").ShouldBeTrue();
        }
    }
}
