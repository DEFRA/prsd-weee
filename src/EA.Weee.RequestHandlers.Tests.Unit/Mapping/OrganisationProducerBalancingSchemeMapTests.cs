namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using System;
    using AutoFixture;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.RequestHandlers.Mappings;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class OrganisationProducerBalancingSchemeMapTests : SimpleUnitTestBase
    {
        private readonly OrganisationProducerBalancingSchemeMap map;
        private readonly IMapper mapper;

        public OrganisationProducerBalancingSchemeMapTests()
        {
            mapper = A.Fake<IMapper>();

            map = new OrganisationProducerBalancingSchemeMap(mapper);
        }

        [Fact]
        public void Map_GivenProducerBalancingSchemeWithOrganisation_PropertiesShouldBeMapped()
        {
            // arrange
            var organisationName = TestFixture.Create<string>();
            var organisationId = Guid.NewGuid();
            var organisation = new Organisation();

            ObjectInstantiator<Organisation>.SetProperty(o => o.Id, organisationId, organisation);
            ObjectInstantiator<Organisation>.SetProperty(o => o.Name, organisationName, organisation);
         
            var producerBalancingScheme = A.Fake<ProducerBalancingScheme>();

            A.CallTo(() => producerBalancingScheme.Organisation).Returns(organisation);

            // act
            var result = map.Map(producerBalancingScheme);

            // arrange
            result.Id.Should().Be(organisationId);
            result.DisplayName.Should().Be(organisationName);
        }

        [Fact]
        public void Map_GivenProducerBalancingSchemeWithEmptyOrganisationDetails_PropertiesShouldBeMapped()
        {
            // arrange
            var organisation = new Organisation();

            var producerBalancingScheme = A.Fake<ProducerBalancingScheme>();

            A.CallTo(() => producerBalancingScheme.Organisation).Returns(organisation);

            // act
            var result = map.Map(producerBalancingScheme);

            // arrange
            result.Id.Should().Be(Guid.Empty);
            result.DisplayName.Should().Be(null);
        }
    }
}
