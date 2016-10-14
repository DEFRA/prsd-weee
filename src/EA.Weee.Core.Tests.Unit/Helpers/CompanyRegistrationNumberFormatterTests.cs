namespace EA.Weee.Core.Tests.Unit.Helpers
{
    using Core.Helpers;
    using Xunit;

    public class CompanyRegistrationNumberFormatterTests
    {
        [Theory]
        [InlineData(null, null)]
        [InlineData("", "")]
        [InlineData("  ", "")]
        [InlineData("000", "")]
        [InlineData("  000  ", "")]
        [InlineData("000  ", "")]
        [InlineData("  000  ", "")]
        [InlineData("  123", "123")]
        [InlineData("123  ", "123")]
        [InlineData("  123  ", "123")]
        [InlineData("  00123  ", "123")]
        [InlineData("  0012300  ", "12300")]
        [InlineData("  0 0 1 2 3  ", "123")]
        [InlineData("  0012 300  ", "12300")]
        [InlineData("  00123 00  ", "12300")]
        [InlineData("12300", "12300")]
        [InlineData("123001", "123001")]
        [InlineData("123", "123")]
        public void FormatCompanyRegistrationNumber_ReturnsFormattedString(string originalValue, string expectedValue)
        {
            var result = CompanyRegistrationNumberFormatter.FormatCompanyRegistrationNumber(originalValue);

            Assert.Equal(expectedValue, result);
        }
    }
}
