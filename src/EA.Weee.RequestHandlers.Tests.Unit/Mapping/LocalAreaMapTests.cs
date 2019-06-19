namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using EA.Weee.Domain.Lookup;
    using EA.Weee.RequestHandlers.Mappings;
    using FluentAssertions;
    using System;
    using Xunit;

    public class LocalAreaMapTests
    {
        private readonly LocalAreaMap mapper;

        public LocalAreaMapTests()
        {
            mapper = new LocalAreaMap();
        }

        [Fact]
        public void Map_GivenSource_LocalAreaDataMustBeMapped()
        {
            var localArea = new LocalArea()
            {
                Name = "PName",
                CompetentAuthorityId = Guid.NewGuid(),
                Id = Guid.NewGuid()
            };

            var result = mapper.Map(localArea);

            result.Name.Should().Be(localArea.Name);
            result.Id.Should().Be(localArea.Id);
            result.CompetentAuthorityId.Should().Be(localArea.CompetentAuthorityId);
        }

        [Fact]
        public void Map_GivenNull_NullShouldBeReturned()
        {
            var result = mapper.Map(null);

            result.Should().BeNull();
        }
    }
}
