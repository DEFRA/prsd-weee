namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using Domain.AatfReturn;
    using FluentAssertions;
    using Mappings;
    using Xunit;

    public class ReturnStatusMapTests
    {
        private readonly ReturnStatusMap map;

        public ReturnStatusMapTests()
        {
            map = new ReturnStatusMap();
        }

        [Fact]
        public void Map_GivenSubmittedStatus_MappedStatusShouldBeSubmitted()
        {
            var result = map.Map(ReturnStatus.Submitted);

            result.Should().Be(Core.AatfReturn.ReturnStatus.Submitted);
        }

        [Fact]
        public void Map_GivenCreatedStatus_MappedStatusShouldBeCreated()
        {
            var result = map.Map(ReturnStatus.Created);

            result.Should().Be(Core.AatfReturn.ReturnStatus.Created);
        }
    }
}
