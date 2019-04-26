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
        public void Map_GivenValidSource_PropertiesShouldBeMapped()
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
            var transfer = new ReturnAndAatfToSentOnCreateSiteViewModelMapTransfer()
            {
                ReturnId = returnId,
                AatfId = aatfId,
                OrganisationId = orgId,
                CountryData = A.Fake<IList<Core.Shared.CountryData>>(),
                SiteAddressData = siteAddressData
            };

            var result = map.Map(transfer);

            result.OrganisationId.Should().Be(orgId);
            result.ReturnId.Should().Be(returnId);
            result.AatfId.Should().Be(aatfId);
            result.SiteAddressData.Should().BeEquivalentTo(siteAddressData);
        }
    }
}
