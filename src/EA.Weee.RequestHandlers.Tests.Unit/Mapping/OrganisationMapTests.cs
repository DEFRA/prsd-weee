namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using Core.Shared;
    using Domain.Organisation;
    using FakeItEasy;
    using FluentAssertions;
    using Mappings;
    using Prsd.Core.Mapper;
    using Xunit;

    public class OrganisationMapTests
    {
        private readonly OrganisationMap map;
        private readonly IMap<Address, AddressData> addressMap;

        public OrganisationMapTests()
        {
            addressMap = A.Fake<IMap<Address, AddressData>>();

            map = new OrganisationMap(addressMap);
        }

        [Fact]
        public void Map_GivenOrganisationIsProducerBalancingScheme_IsBalancingSchemeShouldBeTrue()
        {
            //arrange
            var organisation = A.Fake<Organisation>();
            A.CallTo(() => organisation.ProducerBalancingScheme).Returns(null);

            //act
            var result = map.Map(organisation);

            //assert
            result.IsBalancingScheme.Should().BeFalse();
        }

        [Fact]
        public void Map_GivenOrganisationIsNotProducerBalancingScheme_IsBalancingSchemeShouldBeFalse()
        {
            //arrange
            var organisation = A.Fake<Organisation>();
            A.CallTo(() => organisation.ProducerBalancingScheme).Returns(new ProducerBalancingScheme());

            //act
            var result = map.Map(organisation);

            //assert
            result.IsBalancingScheme.Should().BeTrue();
        }
    }
}
