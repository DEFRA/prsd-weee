namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Mapping.ToViewModel
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class ReturnAndAatfToSentOnCreateSiteViewModelMapTests
    {
        private readonly ReturnAndAatfToSentOnCreateSiteViewModelMap map;

        public ReturnAndAatfToSentOnCreateSiteViewModelMapTests()
        {
            map = new ReturnAndAatfToSentOnCreateSiteViewModelMap(A.Fake<IWeeeCache>());
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNullExceptionExpected()
        {
            Action action = () => map.Map(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenNullSiteAddress_SiteAddressShouldNotBeNullAndContainCountryData()
        {
            var orgId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            
            var transfer = new ReturnAndAatfToSentOnCreateSiteViewModelMapTransfer()
            {
                ReturnId = returnId,
                AatfId = aatfId,
                OrganisationId = orgId,
                CountryData = A.Fake<IList<Core.Shared.CountryData>>()
            };

            var siteAddressToCheckAgainst = new AatfAddressData()
            {
                Countries = transfer.CountryData
            };

            var result = map.Map(transfer);

            result.SiteAddressData.Should().BeEquivalentTo(siteAddressToCheckAgainst);
            result.SiteAddressData.Countries.Should().BeEquivalentTo(transfer.CountryData);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(null)]
        public void Map_GivenValidSource_PropertiesShouldBeMapped(bool javascriptDisabled)
        {
            var orgId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            var siteAddressData = new AatfAddressData()
            {
                Name = "Name",
                Address1 = "Address",
                Address2 = "Address2",
                TownOrCity = "Town",
                CountyOrRegion = "County",
                Postcode = "Post",
                CountryName = "CountryName",
                CountryId = Guid.NewGuid()
            };

            var operatorAddressData = new AatfAddressData()
            {
                Name = "SiteOP",
                Address1 = "OPAddress1",
                Address2 = "OPAddress2",
                TownOrCity = "OPTown",
                CountyOrRegion = "OPCounty",
                CountryName = "OPCountryName",
                CountryId = Guid.NewGuid(),
                Countries = A.Fake<IList<Core.Shared.CountryData>>()
            };

            var transfer = new ReturnAndAatfToSentOnCreateSiteViewModelMapTransfer()
            {
                ReturnId = returnId,
                AatfId = aatfId,
                OrganisationId = orgId,
                CountryData = A.Fake<IList<Core.Shared.CountryData>>(),
                SiteAddressData = siteAddressData,
                OperatorAddressData = operatorAddressData,
                JavascriptDisabled = javascriptDisabled
            };

            var result = map.Map(transfer);

            result.OrganisationId.Should().Be(orgId);
            result.ReturnId.Should().Be(returnId);
            result.AatfId.Should().Be(aatfId);
            result.SiteAddressData.Should().BeEquivalentTo(siteAddressData);

            if (javascriptDisabled == true)
            {
                result.OperatorAddressData.Address1.Should().Be(siteAddressData.Address1);
                result.OperatorAddressData.Address2.Should().Be(siteAddressData.Address2);
                result.OperatorAddressData.TownOrCity.Should().Be(siteAddressData.TownOrCity);
                result.OperatorAddressData.CountyOrRegion.Should().Be(siteAddressData.CountyOrRegion);
                result.OperatorAddressData.CountryName.Should().Be(siteAddressData.CountryName);
            }
            else
            {
                result.OperatorAddressData.Should().BeEquivalentTo(operatorAddressData);
            }
        }
    }
}
