using HyperFabric.Core;
using Shouldly;
using Xunit;

namespace HyperFabric.Tests.Core
{
    public class DeploymentItemTests
    {
        private readonly DeploymentItem _deploymentItem = new DeploymentItem();

        [Fact]
        public void NotSet_MaxApplicationReadyWaitTime_UsesDefaultValue()
        {
            _deploymentItem.MaxApplicationReadyWaitTime.ShouldBe(10);
        }
        
        [Theory]
        [InlineData(4)]
        [InlineData(301)]
        public void OutOfRangeReadyWaitTime_MaxApplicationReadyWaitTime_UsesDefaultValue(int value)
        {
            _deploymentItem.MaxApplicationReadyWaitTime = value;
            
            _deploymentItem.MaxApplicationReadyWaitTime.ShouldBe(10);
        }
        
        [Theory]
        [InlineData(5)]
        [InlineData(100)]
        [InlineData(300)]
        public void InRangeReadyWaitTime_MaxApplicationReadyWaitTime_UsesValue(int value)
        {
            _deploymentItem.MaxApplicationReadyWaitTime = value;
            
            _deploymentItem.MaxApplicationReadyWaitTime.ShouldBe(value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void InvalidPackagePath_ApplicationManifestFile_ReturnsNull(string packagePath)
        {
            _deploymentItem.PackagePath = packagePath;
            
            _deploymentItem.ApplicationManifestFile.ShouldBeNull();
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void InvalidParameterFile_ApplicationId_ReturnsNull(string parameterFile)
        {
            _deploymentItem.ParameterFile = parameterFile;

            var applicationId = _deploymentItem.ApplicationId;
            
            applicationId.ShouldBeNull();
        }
    }
}
