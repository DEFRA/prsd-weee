namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain.Lookup;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class PanAreaMapTests
    {
        private readonly IMap<PanArea, PanAreaData> mapper;

        public PanAreaMapTests()
        {
            mapper = A.Fake<IMap<PanArea, PanAreaData>>();
        }

        [Fact]
        public void Map_GivenSource_PanAreaDataMustBeMapped()
        {
            var panName = "PName";
            var panId = Guid.NewGuid();
            var panCompId = Guid.NewGuid();

            var panArea = A.Fake<PanArea>();

            A.CallTo(() => panArea.Name).Returns(panName);
            A.CallTo(() => panArea.Id).Returns(panId);
            A.CallTo(() => panArea.CompetentAuthorityId).Returns(panCompId);

            var result = mapper.Map(panArea);

            result.Name.Should().Be(panName);
            result.Id.Should().Be(panId);
            result.CompetentAuthorityId.Should().Be(panCompId);
        }
    }
}
