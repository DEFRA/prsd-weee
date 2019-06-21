namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Mapping.ToViewModel
{
    using System;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using EA.Weee.Web.ViewModels.Shared.Utilities;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class AddressUtilitiesTests
    {
        private readonly AddressUtilities addressUtilities;

        public AddressUtilitiesTests()
        {
            addressUtilities = new AddressUtilities();
        }

        [Fact]
        public void AddressConcatenate_OnAddressData_ReturnsCorrectlyFormattedAddress()
        {
            var addressData = new SiteAddressData(
                "Name",
                "Address1",
                "Address2",
                "Town",
                "County",
                "PO12 3ST",
                A.Dummy<Guid>(),
                "Country");

            var result = addressUtilities.AddressConcatenate(addressData);

            result.Should().Be("Address1, Address2, Town, County, PO12 3ST, Country");
        }

        [Fact]
        public void AddressConcatenate_OnAddressDataWithAllowedNulls_ReturnsCorrectlyFormattedAddress()
        {
            var addressData = new SiteAddressData(
                "Name",
                "Address1",
                null,
                "Town",
                null,
                null,
                A.Dummy<Guid>(),
                "Country");

            var result = addressUtilities.AddressConcatenate(addressData);

            result.Should().Be("Address1, Town, Country");
        }

        [Fact]
        public void StringConcatenate_OnInputs_ReturnsCorrectlyFormattedString()
        {
            var result = addressUtilities.StringConcatenate("My Current Address, To Be Checked", "Element to Add");

            result.Should().Be("My Current Address, To Be Checked, Element to Add");
        }

        [Fact]
        public void FormattedAddress_GivenFullAddress_AddressShouldBeFormattedCorrectly()
        {
            var result = addressUtilities.FormattedAddress(new AatfAddressData("Site name", "Site address 1", "Site address 2", "Site town", "Site county", "GU22 7UY", Guid.NewGuid(), "Site country"));

            result.Should().Be("Site name<br/>Site address 1<br/>Site address 2<br/>Site town<br/>Site county<br/>GU22 7UY<br/>Site country");
        }

        [Fact]
        public void FormattedAddress_GivenFullAddressWithoutSiteName_AddressShouldBeFormattedCorrectly()
        {
            var result = addressUtilities.FormattedAddress(new AatfAddressData("Site name", "Site address 1", "Site address 2", "Site town", "Site county", "GU22 7UY", Guid.NewGuid(), "Site country"), false);

            result.Should().Be("Site address 1<br/>Site address 2<br/>Site town<br/>Site county<br/>GU22 7UY<br/>Site country");
        }

        [Fact]
        public void FormattedAddress_GivenAddressWithoutAddress2_AddressShouldBeFormattedCorrectly()
        {
            var result = addressUtilities.FormattedAddress(new AatfAddressData("Site name", "Site address 1", null, "Site town", "Site county", "GU22 7UY", Guid.NewGuid(), "Site country"));

            result.Should().Be("Site name<br/>Site address 1<br/>Site town<br/>Site county<br/>GU22 7UY<br/>Site country");
        }

        [Fact]
        public void FormattedAddress_GivenAddressWithoutCounty_AddressShouldBeFormattedCorrectly()
        {
            var result = addressUtilities.FormattedAddress(new AatfAddressData("Site name", "Site address 1", "Site address 2", "Site town", null, "GU22 7UY", Guid.NewGuid(), "Site country"));

            result.Should().Be("Site name<br/>Site address 1<br/>Site address 2<br/>Site town<br/>GU22 7UY<br/>Site country");
        }

        [Fact]
        public void FormattedAddress_GivenAddressWithoutPostcode_AddressShouldBeFormattedCorrectly()
        {
            var result = addressUtilities.FormattedAddress(new AatfAddressData("Site name", "Site address 1", "Site address 2", "Site town", "Site county", null, Guid.NewGuid(), "Site country"));

            result.Should().Be("Site name<br/>Site address 1<br/>Site address 2<br/>Site town<br/>Site county<br/>Site country");
        }

        [Fact]
        public void FormattedAddress_GivenAddressWithoutAnyOptionFields_AddressShouldBeFormattedCorrectly()
        {
            var result = addressUtilities.FormattedAddress(new AatfAddressData("Site name", "Site address 1", null, "Site town", null, null, Guid.NewGuid(), "Site country"));

            result.Should().Be("Site name<br/>Site address 1<br/>Site town<br/>Site country");
        }

        [Fact]
        public void FormattedAddress_GivenAddressIsNull_EmptyAddressExpected()
        {
            var result = addressUtilities.FormattedAddress(null);

            result.Should().BeEmpty();
        }
    }
}