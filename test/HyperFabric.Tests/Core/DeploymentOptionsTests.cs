using HyperFabric.Core;
using Shouldly;
using Xunit;

namespace HyperFabric.Tests.Core
{
    public class DeploymentOptionsTests
    {
        private readonly DeploymentOptions _deploymentItem = new DeploymentOptions();
        
        [Fact]
        public void NotSet_NumberOfParallelDeployments_UsesDefaultValue()
        {
            _deploymentItem.NumberOfParallelDeployments.ShouldBe(5);
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(11)]
        public void OutOfRangeNumber_NumberOfParallelDeployments_UsesDefaultValue(int value)
        {
            _deploymentItem.NumberOfParallelDeployments = value;
            
            _deploymentItem.NumberOfParallelDeployments.ShouldBe(5);
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(8)]
        public void InRangeNumber_NumberOfParallelDeployments_UsesValue(int value)
        {
            _deploymentItem.NumberOfParallelDeployments = value;
            
            _deploymentItem.NumberOfParallelDeployments.ShouldBe(value);
        }
    }
}
