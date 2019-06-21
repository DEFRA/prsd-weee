namespace EA.Weee.Core.Tests.Unit.Search
{
    using EA.Weee.Core.Search;
    using EA.Weee.Core.Shared;
    using Xunit;

    public class OrganisationSearchResultTests
    {
        [Fact]
        public void OrganisationSearchResult_NameWithAddressPropertyPopulatedProperly()
        {
            OrganisationSearchResult result = new OrganisationSearchResult()
            {
                Name = "Test org",
                Address = new AddressData()
                {
                    Address1 = "Add1",
                    Address2 = "Add2",
                    TownOrCity = "Town",
                    CountyOrRegion = "County",
                    Postcode = "Postcode",
                    CountryName = "England"
                }
            };

            Assert.Equal("Test org (Add1, Add2, Town, County, Postcode, England)", result.NameWithAddress);
        }

        [Fact]
        public void OrganisationSearchResultOptionalFieldBlank_NameWithAddressPropertyPopulatedProperly()
        {
            OrganisationSearchResult result = new OrganisationSearchResult()
            {
                Name = "Test org",
                Address = new AddressData()
                {
                    Address1 = "Add1",
                    TownOrCity = "Town",
                    CountryName = "England"
                }
            };

            Assert.Equal("Test org (Add1, Town, England)", result.NameWithAddress);
        }
    }
}
