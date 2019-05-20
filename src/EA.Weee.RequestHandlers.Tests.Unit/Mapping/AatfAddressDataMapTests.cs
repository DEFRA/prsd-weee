namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EA.Prsd.Core.Domain;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Mappings;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class AatfAddressDataMapTests
    {
        private readonly AatfAddressDataMap map;

        public AatfAddressDataMapTests()
        {
            map = new AatfAddressDataMap();
        }

        [Fact]
        public void Map_GivenSourceIsNull__BlankContactReturned()
        {
            var result = map.Map(null);
            
            result.Name.Should().Be(null);
            result.Address1.Should().Be(null);
            result.Address2.Should().Be(null);
            result.TownOrCity.Should().Be(null);
            result.CountyOrRegion.Should().Be(null);
            result.Postcode.Should().Be(null);
            result.Country.Should().Be(null);
        }

        [Fact]
        public void Map_GivenSource_AddressDataIsMapped()
        {
            Guid countryId = Guid.NewGuid();
            string countryName = "Country";

            AatfAddressData addressData = new AatfAddressData("Name", "Address1", "Address2", "Town", "County", "Postcode", countryId, countryName);

            var result = map.Map(addressData);

            result.Name.Should().Be("Name");
            result.Address1.Should().Be("Address1");
            result.Address2.Should().Be("Address2");
            result.TownOrCity.Should().Be("Town");
            result.CountyOrRegion.Should().Be("County");
            result.Postcode.Should().Be("Postcode");
            result.CountryId.Should().Be(countryId);
        }
    }
}
