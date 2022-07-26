namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using System;
    using AutoFixture;
    using Domain.Scheme;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.RequestHandlers.Mappings;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class SchemeOrganisationMapTests : SimpleUnitTestBase
    {
        private readonly SchemeOrganisationMap map;
        private readonly IMapper mapper;

        public SchemeOrganisationMapTests()
        {
            mapper = A.Fake<IMapper>();

            map = new SchemeOrganisationMap(mapper);
        }

        [Fact]
        public void Map_GivenNullSchemeSource_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => map.Map(null));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenScheme_PropertiesShouldBeMapped()
        {
            //arrange
            var schemeName = TestFixture.Create<string>();
            var id = Guid.NewGuid();
            var scheme = new Scheme(id);

            ObjectInstantiator<Scheme>.SetProperty(o => o.SchemeName, schemeName, scheme);

            //act
            var result = map.Map(scheme);

            //assert
            result.Id.Should().Be(id);
            result.DisplayName.Should().Be(schemeName);
        }
    }
}
