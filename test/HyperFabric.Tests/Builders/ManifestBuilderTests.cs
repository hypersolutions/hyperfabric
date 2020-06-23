using System;
using System.IO;
using System.Linq;
using HyperFabric.Builders;
using Shouldly;
using Xunit;

namespace HyperFabric.Tests.Builders
{
    public class ManifestBuilderTests : TestBase<ManifestBuilder>
    {
        private readonly string _manifestJson =
            Path.Combine(TestInfo.OutputDir, "Support", "manifest.json");
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("c:\temp")]
        public void InvalidPath_FromFile_ThrowsException(string path)
        {
            var exception = Should.Throw<FileNotFoundException>(() => Subject.FromFile(path));
            
            exception.Message.ShouldBe("Unable to find the manifest file.");
        }
        
        [Fact]
        public void ValidPath_FromFile_HydratesManifestClusterDetails()
        {
            var manifest = Subject.FromFile(_manifestJson);
            
            manifest.ClusterDetails.Connection.ShouldBe("http://localhost:19080");
            manifest.ClusterDetails.FindByValue.ShouldBe("EC0C11751FBE401BAE21685EF4A8C46B");
        }
        
        [Fact]
        public void ValidPath_FromFile_HydratesManifestGroups()
        {
            var manifest = Subject.FromFile(_manifestJson);
            
            manifest.Groups.Count().ShouldBe(1);
        }
        
        [Fact]
        public void ValidPath_FromFile_HydratesManifestItems()
        {
            var manifest = Subject.FromFile(_manifestJson);

            var group = manifest.Groups.First();
            group.Items.Count().ShouldBe(2);
        }
        
        [Fact]
        public void ValidPath_FromFile_HydratesManifestFirstItemDetails()
        {
            var manifest = Subject.FromFile(_manifestJson);

            var group = manifest.Groups.First();
            var item = group.Items.First();
            item.PackagePath.ShouldBe("C:\\Temp\\App1Pkg");
            item.ParameterFile.ShouldBe("Local.1Node.xml");
            item.RemoveApplicationFirst.ShouldBe(true);
            item.MaxApplicationReadyWaitTime.ShouldBe(50);
        }
        
        [Fact]
        public void ValidPath_FromFile_HydratesManifestSecondItemDetails()
        {
            var manifest = Subject.FromFile(_manifestJson);

            var group = manifest.Groups.First();
            var item = group.Items.Last();
            item.PackagePath.ShouldBe("C:\\Temp\\App2Pkg");
            item.ParameterFile.ShouldBe("Local.5Node.xml");
            item.RemoveApplicationFirst.ShouldBe(false);
            item.MaxApplicationReadyWaitTime.ShouldBe(100);
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void InvalidJson_FromString_ThrowsException(string json)
        {
            var exception = Should.Throw<ArgumentException>(() => Subject.FromString(json));
            
            exception.Message.ShouldContain("Invalid json string provided.");
        }
        
        [Fact]
        public void ValidPath_FromString_HydratesManifestClusterDetails()
        {
            var json = File.ReadAllText(_manifestJson);
            
            var manifest = Subject.FromString(json);
            
            manifest.ClusterDetails.Connection.ShouldBe("http://localhost:19080");
            manifest.ClusterDetails.FindByValue.ShouldBe("EC0C11751FBE401BAE21685EF4A8C46B");
        }
        
        [Fact]
        public void ValidPath_FromString_HydratesManifestGroups()
        {
            var json = File.ReadAllText(_manifestJson);
            
            var manifest = Subject.FromString(json);
            
            manifest.Groups.Count().ShouldBe(1);
        }
        
        [Fact]
        public void ValidPath_FromString_HydratesManifestItems()
        {
            var json = File.ReadAllText(_manifestJson);
            
            var manifest = Subject.FromString(json);

            var group = manifest.Groups.First();
            group.Items.Count().ShouldBe(2);
        }
        
        [Fact]
        public void ValidPath_FromString_HydratesManifestFirstItemDetails()
        {
            var json = File.ReadAllText(_manifestJson);
            
            var manifest = Subject.FromString(json);

            var group = manifest.Groups.First();
            var item = group.Items.First();
            item.PackagePath.ShouldBe("C:\\Temp\\App1Pkg");
            item.ParameterFile.ShouldBe("Local.1Node.xml");
            item.RemoveApplicationFirst.ShouldBe(true);
            item.MaxApplicationReadyWaitTime.ShouldBe(50);
        }
        
        [Fact]
        public void ValidPath_FromString_HydratesManifestSecondItemDetails()
        {
            var json = File.ReadAllText(_manifestJson);
            
            var manifest = Subject.FromString(json);

            var group = manifest.Groups.First();
            var item = group.Items.Last();
            item.PackagePath.ShouldBe("C:\\Temp\\App2Pkg");
            item.ParameterFile.ShouldBe("Local.5Node.xml");
            item.RemoveApplicationFirst.ShouldBe(false);
            item.MaxApplicationReadyWaitTime.ShouldBe(100);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void InvalidConnection_FromClusterDetails_ThrowsException(string connection)
        {
            var exception = Should.Throw<ArgumentException>(() => Subject.FromClusterDetails(connection));
            
            exception.Message.ShouldContain("Invalid connection provided.");
        }
        
        [Fact]
        public void FromConnection_FromClusterDetails_ReturnsManifestClusterDetails()
        {
            var manifest = Subject.FromClusterDetails("http://localhost:19080");
            
            manifest.ClusterDetails.Connection.ShouldBe("http://localhost:19080");
        }
        
        [Fact]
        public void FromConnectionAndThumbprint_FromClusterDetails_ReturnsManifestClusterDetails()
        {
            var manifest = Subject.FromClusterDetails("http://localhost:19080", "EC0C11751FBE401BAE21685EF4A8C46B");
            
            manifest.ClusterDetails.Connection.ShouldBe("http://localhost:19080");
            manifest.ClusterDetails.FindByValue.ShouldBe("EC0C11751FBE401BAE21685EF4A8C46B");
        }
    }
}
