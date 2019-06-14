namespace EA.Weee.Core.Tests.Unit.Search
{
    using EA.Weee.Core.Search;
    using EA.Weee.Core.Shared;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
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
                    Address1 = "Here",
                    Address2 = "Empty",
                    TownOrCity = "There",
                    Postcode = "Empty",
                    CountryName = "England"
                }
            };

            Assert.Equal("Test org (Here, There, England)", result.NameWithAddress);
        }
    }
}
