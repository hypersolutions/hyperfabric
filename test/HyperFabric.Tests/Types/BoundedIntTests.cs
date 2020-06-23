using HyperFabric.Types;
using Shouldly;
using Xunit;

namespace HyperFabric.Tests.Types
{
    public class BoundedIntTests
    {
        [Theory]
        [InlineData(-1)]
        [InlineData(9)]
        [InlineData(21)]
        public void OutOfRangeValue_Value_ReturnsDefault(int value)
        {
            var boundedInt = new BoundedInt(10, 20, 15);
            
            boundedInt.Value = value;
            
            boundedInt.Value.ShouldBe(15);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void ValueInRange_Value_ReturnsValue(int value)
        {
            var boundedInt = new BoundedInt(1, 3, 2);
            
            boundedInt.Value = value;
            
            boundedInt.Value.ShouldBe(value);
        }

        [Fact]
        public void ValueInRange_Value_CastsImplicitlyToInt()
        {
            var boundedInt = new BoundedInt(1, 3, 2);

            int value = boundedInt;
            
            value.ShouldBe(2);
        }
    }
}
