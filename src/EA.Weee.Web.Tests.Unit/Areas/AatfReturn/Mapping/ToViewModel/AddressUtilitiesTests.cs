namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Mapping.ToViewModel
{
    using System;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class AddressUtilitiesTests
    {
        private readonly IAddressUtilities addressUtilities;

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
    }
}