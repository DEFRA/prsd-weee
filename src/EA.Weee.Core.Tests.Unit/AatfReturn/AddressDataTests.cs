namespace EA.Weee.Core.Tests.Unit.AatfReturn
{
    using Core.AatfReturn;
    using FluentAssertions;
    using Xunit;

    public class AddressDataTests
    {
        [Theory]
        [InlineData(true, true)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(false, false)]
        public void ToAccessibleDisplayString_WithoutOptionalFields_ReturnsCorrectAddress(bool includeName, bool endLineWithComma)
        {
            var address = new AatfAddressData()
            {
                Name = "Name",
                Address1 = "Address 1",
                TownOrCity = "Town",
                CountryName = "Country"
            };

            var comma = endLineWithComma ? "," : string.Empty;
            var expected = $"<span>Address 1{comma}</span><span>Town{comma}</span><span>Country</span>";

            if (includeName)
            {
                address.Name = "Name";
                expected = $"<span>Name{comma}</span>{expected}";
            }

            var result = address.ToAccessibleDisplayString(includeName, endLineWithComma);

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(false, false)]
        public void ToAccessibleDisplayString_WithOptionalAddress2_ReturnsCorrectAddress(bool includeName, bool endLineWithComma)
        {
            var address = new AatfAddressData()
            {
                Address1 = "Address 1",
                Address2 = "Address 2",
                TownOrCity = "Town",
                CountryName = "Country"
            };

            var comma = endLineWithComma ? "," : string.Empty;
            var expected = $"<span>Address 1{comma}</span><span>Address 2{comma}</span><span>Town{comma}</span><span>Country</span>";

            if (includeName)
            {
                address.Name = "Name";
                expected = $"<span>Name{comma}</span>{expected}";
            }

            var result = address.ToAccessibleDisplayString(includeName, endLineWithComma);

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(false, false)]
        public void ToAccessibleDisplayString_WithOptionalCounty_ReturnsCorrectAddress(bool includeName, bool endLineWithComma)
        {
            var address = new AatfAddressData()
            {
                Address1 = "Address 1",
                TownOrCity = "Town",
                CountyOrRegion = "County",
                CountryName = "Country"
            };

            var comma = endLineWithComma ? "," : string.Empty;
            var expected = $"<span>Address 1{comma}</span><span>Town{comma}</span><span>County{comma}</span><span>Country</span>";

            if (includeName)
            {
                address.Name = "Name";
                expected = $"<span>Name{comma}</span>{expected}";
            }

            var result = address.ToAccessibleDisplayString(includeName, endLineWithComma);

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(false, false)]
        public void ToAccessibleDisplayString_WithOptionalPostcode_ReturnsCorrectAddress(bool includeName, bool endLineWithComma)
        {
            var address = new AatfAddressData()
            {
                Address1 = "Address 1",
                TownOrCity = "Town",
                Postcode = "Postcode",
                CountryName = "Country"
            };

            var comma = endLineWithComma ? "," : string.Empty;
            var expected = $"<span>Address 1{comma}</span><span>Town{comma}</span><span>Postcode{comma}</span><span>Country</span>";

            if (includeName)
            {
                address.Name = "Name";
                expected = $"<span>Name{comma}</span>{expected}";
            }

            var result = address.ToAccessibleDisplayString(includeName, endLineWithComma);

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(false, false)]
        public void ToAccessibleDisplayString_WithAllOptionalFields_ReturnsCorrectAddress(bool includeName, bool endLineWithComma)
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

            var comma = endLineWithComma ? "," : string.Empty;
            var expected = $"<span>Address 1{comma}</span><span>Address 2{comma}</span><span>Town{comma}</span><span>County{comma}</span><span>Postcode{comma}</span><span>Country</span>";

            if (includeName)
            {
                address.Name = "Name";
                expected = $"<span>Name{comma}</span>{expected}";
            }

            var result = address.ToAccessibleDisplayString(includeName, endLineWithComma);

            result.Should().Be(expected);
        }
    }
}
