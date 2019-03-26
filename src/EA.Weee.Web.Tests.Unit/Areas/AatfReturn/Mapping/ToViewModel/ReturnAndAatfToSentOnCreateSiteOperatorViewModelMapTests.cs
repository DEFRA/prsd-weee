namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Mapping.ToViewModel
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class ReturnAndAatfToSentOnCreateSiteOperatorViewModelMapTests
    {
        private readonly ReturnAndAatfToSentOnCreateSiteOperatorViewModelMap map;

        public ReturnAndAatfToSentOnCreateSiteOperatorViewModelMapTests()
        {
            map = new ReturnAndAatfToSentOnCreateSiteOperatorViewModelMap(A.Fake<IWeeeCache>());
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNullExceptionExpected()
        {
            Action action = () => map.Map(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(null)]
        public void Map_GivenValidSource_PropertiesShouldBeMapped(bool javascriptDisabled)
        {
            var orgId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            var weeeSentOnId = Guid.NewGuid();
            var siteAddressData = new AatfAddressData()
            {
                Name = "Site",
                Address1 = "Address1",
                Address2 = "Address2",
                TownOrCity = "Town",
                CountyOrRegion = "County",
                CountryName = "CountryName"
            };

            var transfer = new ReturnAndAatfToSentOnCreateSiteOperatorViewModelMapTransfer(A.Fake<IList<Core.Shared.CountryData>>())
            {
                ReturnId = returnId,
                AatfId = aatfId,
                OrganisationId = orgId,
                WeeeSentOnId = weeeSentOnId,
                SiteAddressData = siteAddressData,
                JavascriptDisabled = javascriptDisabled
            };

            var result = map.Map(transfer);

            result.OrganisationId.Should().Be(orgId);
            result.ReturnId.Should().Be(returnId);
            result.AatfId.Should().Be(aatfId);
            result.WeeeSentOnId.Should().Be(weeeSentOnId);
            result.SiteAddressData.Should().Be(siteAddressData);

            if (javascriptDisabled == true)
            {
                result.OperatorAddressData.Address1.Should().Be(siteAddressData.Address1);
                result.OperatorAddressData.Address2.Should().Be(siteAddressData.Address2);
                result.OperatorAddressData.TownOrCity.Should().Be(siteAddressData.TownOrCity);
                result.OperatorAddressData.CountyOrRegion.Should().Be(siteAddressData.CountyOrRegion);
                result.OperatorAddressData.CountryName.Should().Be(siteAddressData.CountryName);
            }
        }
    }
}
