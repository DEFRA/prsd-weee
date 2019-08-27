﻿namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using EA.Weee.Domain;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Mappings;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using Xunit;

    public class AatfContactMapTests
    {
        private readonly AatfContactMap map;
        private readonly Guid countryId;
        private readonly Country country;
        private readonly Aatf aatf;

        public AatfContactMapTests()
        {
            map = new AatfContactMap();
            countryId = Guid.NewGuid();
            country = new Country(countryId, "UK - England");
            aatf = A.Fake<Aatf>();
        }

        [Fact]
        public void Map_GivenSourceIsNull__BlankContactReturned()
        {
            var result = map.Map(null);

            result.FirstName.Should().Be(null);
            result.LastName.Should().Be(null);
            result.Position.Should().Be(null);
            result.AddressData.Address1.Should().Be(null);
            result.AddressData.Address2.Should().Be(null);
            result.AddressData.TownOrCity.Should().Be(null);
            result.AddressData.CountyOrRegion.Should().Be(null);
            result.AddressData.CountryName.Should().Be(null);
            result.Telephone.Should().Be(null);
            result.Email.Should().Be(null);
        }

        [Fact]
        public void Map_GivenSource_AddressDataIsMapped()
        {
            var aatfContact = new AatfContact(
                "First Name",
                "Last Name",
                "Position",
                "Address1",
                "Address2",
                "Town",
                "County",
                "PO12 3ST",
                country,
                "01234 567890",
                "email@email.com");

            var result = map.Map(aatfContact);

            result.FirstName.Should().Be("First Name");
            result.LastName.Should().Be("Last Name");
            result.Position.Should().Be("Position");
            result.AddressData.Address1.Should().Be("Address1");
            result.AddressData.Address2.Should().Be("Address2");
            result.AddressData.TownOrCity.Should().Be("Town");
            result.AddressData.CountyOrRegion.Should().Be("County");
            result.AddressData.Postcode.Should().Be("PO12 3ST");
            result.AddressData.CountryName.Should().Be("UK - England");
            result.Telephone.Should().Be("01234 567890");
            result.Email.Should().Be("email@email.com");
        }
    }
}
