namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using EA.Weee.Domain.Lookup;
    using EA.Weee.RequestHandlers.Mappings;
    using FluentAssertions;
    using System;
    using Xunit;

    public class PanAreaMapTests
    {
        private readonly PanAreaMap mapper;

        public PanAreaMapTests()
        {
            mapper = new PanAreaMap();
        }

        [Fact]
        public void Map_GivenSource_PanAreaDataMustBeMapped()
        {
            var panArea = new PanArea()
            {
                Name = "PName",
                CompetentAuthorityId = Guid.NewGuid(),
                Id = Guid.NewGuid()
            };

            var result = mapper.Map(panArea);

            result.Name.Should().Be(panArea.Name);
            result.Id.Should().Be(panArea.Id);
            result.CompetentAuthorityId.Should().Be(panArea.CompetentAuthorityId);
        }
    }
}
