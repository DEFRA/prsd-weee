namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Mapping.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.AatfReturn;
    using EA.Weee.Core.Shared;
    using FakeItEasy;
    using FluentAssertions;
    using Web.Areas.AatfReturn.Mappings.ToViewModel;
    using Xunit;

    public class SiteAddressDataToReusedOffSiteCreateSiteViewModelMapTests
    {
        private readonly SiteAddressDataToReusedOffSiteCreateSiteViewModelMap mapper;

        public SiteAddressDataToReusedOffSiteCreateSiteViewModelMapTests()
        {
            mapper = new SiteAddressDataToReusedOffSiteCreateSiteViewModelMap();
        }

        [Fact]
        public void Map_GivenNullSource_ShouldThrowArgumentNullException()
        {
            Action call = () => mapper.Map(null);

            call.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenValidSource_PropertiesShouldBeMapped()
        {
            var transfer = new SiteAddressDataToReusedOffSiteCreateSiteViewModelMapTransfer()
            {
                OrganisationId = Guid.NewGuid(),
                AatfId = Guid.NewGuid(),
                ReturnId = Guid.NewGuid(),
                SiteId = Guid.NewGuid(),
                Countries = A.Fake<IList<CountryData>>()
            };

            var returnedSite = A.Fake<SiteAddressData>();
            returnedSite.Id = transfer.SiteId;
            var returnedSiteSummary = A.Fake<AddressTonnageSummary>();
            returnedSiteSummary.AddressData = new List<SiteAddressData>() { returnedSite };
            transfer.ReturnedSites = returnedSiteSummary;

            var result = mapper.Map(transfer);

            result.AatfId.Should().Be(transfer.AatfId);
            result.OrganisationId.Should().Be(transfer.OrganisationId);
            result.ReturnId.Should().Be(transfer.ReturnId);
            result.AddressData.Id.Should().Be(transfer.ReturnedSites.AddressData.ElementAt(0).Id);
        }
    }
}
