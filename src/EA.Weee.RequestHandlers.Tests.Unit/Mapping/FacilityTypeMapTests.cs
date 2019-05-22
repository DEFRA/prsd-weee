namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using Domain.AatfReturn;
    using FluentAssertions;
    using Mappings;
    using Xunit;

    public class FacilityTypeMapTests
    {
        private readonly FacilityTypeMap map;

        public FacilityTypeMapTests()
        {
            map = new FacilityTypeMap();
        }

        [Fact]
        public void Map_GivenAatfType_AatfShouldBeReturned()
        {
            var result = map.Map(FacilityType.Aatf);

            result.Should().Be(Core.AatfReturn.FacilityType.Aatf);
        }

        [Fact]
        public void Map_GivenAeType_AeShouldBeReturnedd()
        {
            var result = map.Map(FacilityType.Ae);

            result.Should().Be(Core.AatfReturn.FacilityType.Ae);
        }
    }
}
