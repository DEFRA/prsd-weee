namespace EA.Weee.RequestHandlers.Tests.DataAccess.Scheme.MemberRegistration.GenerateDomainObjects.DataAccess
{
    using System;
    using FakeItEasy;
    using RequestHandlers.Scheme.MemberRegistration.GenerateDomainObjects.DataAccess;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class GenerateFromXmlDataAccessTests
    {
        [Theory]
        [InlineData("Australia", "DDC6551C-D9B2-465C-86DD-670B7D2142C2")]
        [InlineData("UK - England", "184E1785-26B4-4AE4-80D3-AE319B103ACB")]
        [InlineData("UK - Northern Ireland", "7BFB8717-4226-40F3-BC51-B16FDF42550C")]
        public async void GetCountry_ReturnsCorrectCountryForSpecifiedName(string countryName, string countryId)
        {
            using (var database = new DatabaseWrapper())
            {
                var dataAccess = new GenerateFromXmlDataAccess(database.WeeeContext);
                var result = await dataAccess.GetCountry(countryName);

                Assert.Equal(Guid.Parse(countryId), result.Id);
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async void GetCountry_WithNullOrEmptyCountryName_ReturnsNull(string countryName)
        {
            using (var database = new DatabaseWrapper())
            {
                var dataAccess = new GenerateFromXmlDataAccess(database.WeeeContext);
                var result = await dataAccess.GetCountry(countryName);

                Assert.Null(result);
            }
        }

        // TODO: Add unit tests for the 3 new methods added to the IGenerateFromXmlDataAccess interface.
    }
}
