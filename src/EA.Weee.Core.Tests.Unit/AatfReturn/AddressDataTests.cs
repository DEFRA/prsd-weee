namespace EA.Weee.Core.Tests.Unit.AatfReturn
{
    using Core.AatfReturn;
    using FluentAssertions;
    using Xunit;

    public class AddressDataTests
    {
        public const string NameSpan = "<span>Name,</span>";

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ToAccessibleDisplayString_WithoutOptionalFields_ReturnsCorrectAddress(bool includeName)
        {
            var address = new AatfAddressData()
            {
                Name = "Name",
                Address1 = "Address 1",
                TownOrCity = "Town",
                CountryName = "Country"
            };

            var expected = "<span>Address 1,</span><span>Town,</span><span>Country</span>";
            if (includeName)
            {
                address.Name = "Name";
                expected = $"{NameSpan}{expected}";
            }

            var result = AddressData.ToAccessibleDisplayString(address, includeName);

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ToAccessibleDisplayString_WithOptionalAddress2_ReturnsCorrectAddress(bool includeName)
        {
            var address = new AatfAddressData()
            {
                Address1 = "Address 1",
                Address2 = "Address 2",
                TownOrCity = "Town",
                CountryName = "Country"
            };

            var expected = "<span>Address 1,</span><span>Address 2,</span><span>Town,</span><span>Country</span>";
            if (includeName)
            {
                address.Name = "Name";
                expected = $"{NameSpan}{expected}";
            }

            var result = AddressData.ToAccessibleDisplayString(address, includeName);

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ToAccessibleDisplayString_OperatorAddressIsNull_ReturnsNonBreakingWhiteSpace(bool includeName)
        {
            var expected = "&nbsp";

            var result = AddressData.ToAccessibleDisplayString(null, includeName);

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ToAccessibleDisplayString_WithOptionalCounty_ReturnsCorrectAddress(bool includeName)
        {
            var address = new AatfAddressData()
            {
                Address1 = "Address 1",
                TownOrCity = "Town",
                CountyOrRegion = "County",
                CountryName = "Country"
            };

            var expected = "<span>Address 1,</span><span>Town,</span><span>County,</span><span>Country</span>";
            if (includeName)
            {
                address.Name = "Name";
                expected = $"{NameSpan}{expected}";
            }

            var result = AddressData.ToAccessibleDisplayString(address, includeName);

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ToAccessibleDisplayString_WithOptionalPostcode_ReturnsCorrectAddress(bool includeName)
        {
            var address = new AatfAddressData()
            {
                Address1 = "Address 1",
                TownOrCity = "Town",
                Postcode = "Postcode",
                CountryName = "Country"
            };

            var expected = "<span>Address 1,</span><span>Town,</span><span>Postcode,</span><span>Country</span>";
            if (includeName)
            {
                address.Name = "Name";
                expected = $"{NameSpan}{expected}";
            }

            var result = AddressData.ToAccessibleDisplayString(address, includeName);

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ToAccessibleDisplayString_WithAllOptionalFields_ReturnsCorrectAddress(bool includeName)
        {
            var address = new AatfAddressData()
            {
                Address1 = "Address 1",
                Address2 = "Address 2",
                TownOrCity = "Town",
                CountyOrRegion = "County",
                Postcode = "Postcode",
                CountryName = "Country"
            };

            var expected = "<span>Address 1,</span><span>Address 2,</span><span>Town,</span><span>County,</span><span>Postcode,</span><span>Country</span>";
            if (includeName)
            {
                address.Name = "Name";
                expected = $"{NameSpan}{expected}";
            }

            var result = AddressData.ToAccessibleDisplayString(address, includeName);

            result.Should().Be(expected);
        }
    }
}
