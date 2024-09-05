namespace EA.Weee.Core.Tests.Unit.Constant
{
    using EA.Weee.Core.Constants;
    using FluentAssertions;
    using System;
    using Xunit;

    public class UkCountryTests
    {
        [Fact]
        public void ValidIds_ShouldContainAllFourCountries()
        {
            UkCountry.ValidIds.Should().HaveCount(4);
            UkCountry.ValidIds.Should().Contain(UkCountry.Ids.England);
            UkCountry.ValidIds.Should().Contain(UkCountry.Ids.Wales);
            UkCountry.ValidIds.Should().Contain(UkCountry.Ids.Scotland);
            UkCountry.ValidIds.Should().Contain(UkCountry.Ids.NorthernIreland);
        }

        [Theory]
        [InlineData("England", "184E1785-26B4-4AE4-80D3-AE319B103ACB")]
        [InlineData("Wales", "DB83F5AB-E745-49CF-B2CA-23FE391B67A8")]
        [InlineData("Scotland", "4209EE95-0882-42F2-9A5D-355B4D89EF30")]
        [InlineData("Northern Ireland", "7BFB8717-4226-40F3-BC51-B16FDF42550C")]
        [InlineData("Great Britain", "184E1785-26B4-4AE4-80D3-AE319B103ACB")]
        [InlineData("United Kingdom", "184E1785-26B4-4AE4-80D3-AE319B103ACB")]
        [InlineData("Not specified", "00000000-0000-0000-0000-000000000000")]
        public void GetIdByName_ShouldReturnCorrectGuid(string countryName, string expectedGuidString)
        {
            var expectedGuid = Guid.Parse(expectedGuidString);
            var result = UkCountry.GetIdByName(countryName);
            result.Should().Be(expectedGuid);
        }

        [Theory]
        [InlineData("england")]
        [InlineData("WALES")]
        [InlineData("Scotland")]
        [InlineData("nOrThErN iReLaNd")]
        public void GetIdByName_ShouldBeCaseInsensitive(string countryName)
        {
            var result = UkCountry.GetIdByName(countryName);
            result.Should().NotBe(Guid.Empty);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void GetIdByName_WithNullOrEmptyString_ShouldReturnEmptyGuid(string countryName)
        {
            var result = UkCountry.GetIdByName(countryName);
            result.Should().Be(Guid.Empty);
        }

        [Fact]
        public void GetIdByName_WithInvalidCountryName_ShouldThrowArgumentException()
        {
            Action act = () => UkCountry.GetIdByName("Invalid Country");
            act.Should().Throw<ArgumentException>()
               .WithMessage("No matching GUID found for country: Invalid Country");
        }

        [Theory]
        [InlineData("184E1785-26B4-4AE4-80D3-AE319B103ACB", true)]  // England
        [InlineData("DB83F5AB-E745-49CF-B2CA-23FE391B67A8", true)]  // Wales
        [InlineData("4209EE95-0882-42F2-9A5D-355B4D89EF30", true)]  // Scotland
        [InlineData("7BFB8717-4226-40F3-BC51-B16FDF42550C", true)]  // Northern Ireland
        [InlineData("00000000-0000-0000-0000-000000000000", false)] // Empty GUID
        [InlineData("12345678-1234-1234-1234-123456789012", false)] // Random GUID
        public void IsValidId_ShouldReturnCorrectResult(string guidString, bool expectedResult)
        {
            var guid = Guid.Parse(guidString);
            var result = UkCountry.IsValidId(guid);
            result.Should().Be(expectedResult);
        }
    }
}