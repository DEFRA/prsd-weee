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

    public class AatfContactDataMapTests
    {
        private readonly AatfContactDataMap map;

        public AatfContactDataMapTests()
        {
            map = new AatfContactDataMap();
        }

        [Fact]
        public void Map_GivenSourceIsNull__BlankContactReturned()
        {
            var result = map.Map(null);
            
            result.FirstName.Should().Be(null);
            result.LastName.Should().Be(null);
            result.Position.Should().Be(null);
            result.Address1.Should().Be(null);
            result.Address2.Should().Be(null);
            result.TownOrCity.Should().Be(null);
            result.CountyOrRegion.Should().Be(null);
            result.Postcode.Should().Be(null);
            result.Country.Should().Be(null);
            result.Telephone.Should().Be(null);
            result.Email.Should().Be(null);
        }

        [Fact]
        public void Map_GivenSource_ContactDataIsMapped()
        {
            Guid countryId = Guid.NewGuid();
            string countryName = "Country";

            AatfContactAddressData addressData = new AatfContactAddressData("Name", "Address1", "Address2", "Town", "County", "Postcode", countryId, countryName);

            var aatfContact = new AatfContactData(
                Guid.NewGuid(),
                "First Name",
                "Last Name",
                "Position",
                addressData,
                "01234 567890",
                "email@email.com");
         
            var result = map.Map(aatfContact);

            result.FirstName.Should().Be("First Name");
            result.LastName.Should().Be("Last Name");
            result.Position.Should().Be("Position");
            result.Address1.Should().Be("Address1");
            result.Address2.Should().Be("Address2");
            result.TownOrCity.Should().Be("Town");
            result.CountyOrRegion.Should().Be("County");
            result.Postcode.Should().Be("Postcode");
            result.CountryId.Should().Be(countryId);
            result.Telephone.Should().Be("01234 567890");
            result.Email.Should().Be("email@email.com");
        }
    }
}
