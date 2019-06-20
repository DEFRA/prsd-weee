namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EA.Weee.Domain;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Mappings;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class AatfSiteMapTests
    {
        private readonly AatfSiteMap map;
        private readonly Guid countryId;
        private readonly Country country;
        private readonly Aatf aatf;

        public AatfSiteMapTests()
        {
            map = new AatfSiteMap();
            countryId = Guid.NewGuid();
            country = new Country(countryId, "UK - England");
            aatf = A.Fake<Aatf>();
        }

        [Fact]
        public void Map_GivenSourceIsNull_ArgumentNullExceptionExpected()
        {
            Action action = () => map.Map(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenSource_AddressDataIsMapped()
        {
            var aatfAddress = new AatfAddress(
                "Name",
                "Address1",
                "Address2",
                "Town",
                "County",
                "PO12 3ST",
                country);

            var aatfAddress2 = new AatfAddress(
                "Name2",
                "Address12",
                "Address22",
                "Town2",
                "County2",
                "PO12 3ST2",
                country);

            var aatfAddressList = new List<AatfAddress>() { aatfAddress, aatfAddress2 };
            var weeeReusedList = new List<WeeeReusedAmount>();

            var source = new AatfAddressObligatedAmount(aatfAddressList, weeeReusedList);

            var result = map.Map(source);

            result.AddressData.Count(a => a.Name == "Name"
                                       && a.Address1 == "Address1"
                                       && a.Address2 == "Address2"
                                       && a.TownOrCity == "Town"
                                       && a.CountyOrRegion == "County"
                                       && a.Postcode == "PO12 3ST"
                                       && a.CountryId == countryId
                                       && a.CountryName == "UK - England").Should().Be(1);

            result.AddressData.Count(a => a.Name == "Name2"
                                       && a.Address1 == "Address12"
                                       && a.Address2 == "Address22"
                                       && a.TownOrCity == "Town2"
                                       && a.CountyOrRegion == "County2"
                                       && a.Postcode == "PO12 3ST2"
                                       && a.CountryId == countryId
                                       && a.CountryName == "UK - England").Should().Be(1);

            result.AddressData.Count().Should().Be(2);
        }

        [Fact]
        public void Map_GivenNullAddresses_BlankAddressReturned()
        {
            var weeeReusedList = new List<WeeeReusedAmount>();

            var source = new AatfAddressObligatedAmount(null, weeeReusedList);

            var result = map.Map(source);

            result.AddressData.Should().HaveCount(0);
        }

        [Fact]
        public void Map_GivenSource_ObligatedDataIsMapped()
        {
            var weeeReused = ReturnWeeeReused(aatf, A.Dummy<Return>());

            var aatfAddressList = new List<AatfAddress>();
            var weeeReusedList = new List<WeeeReusedAmount>()
            {
                new WeeeReusedAmount(weeeReused, 1, 1.000m, 2.000m),
                new WeeeReusedAmount(weeeReused, 2, 3.000m, 4.000m)
            };

            var source = new AatfAddressObligatedAmount(aatfAddressList, weeeReusedList);

            var result = map.Map(source);

            result.ObligatedData.Count(o => o.CategoryId == 1 && o.B2C == 1 && o.B2B == 2).Should().Be(1);
            result.ObligatedData.Count(o => o.CategoryId == 2 && o.B2C == 3 && o.B2B == 4).Should().Be(1);
            result.ObligatedData.Count().Should().Be(2);
        }

        public WeeeReused ReturnWeeeReused(Aatf aatf, Return @return)
        {
            var weeeReused = new WeeeReused(aatf, @return);

            return weeeReused;
        }
    }
}