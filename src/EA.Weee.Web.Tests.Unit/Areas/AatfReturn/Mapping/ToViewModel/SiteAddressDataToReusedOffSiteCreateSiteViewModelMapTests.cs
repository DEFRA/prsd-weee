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
        public void Map_GivenNullCountryData_ShouldThrowArgumentNullException()
        {
            Action call = () => mapper.Map(new SiteAddressDataToReusedOffSiteCreateSiteViewModelMapTransfer());

            call.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenValidSource_PropertiesShouldBeMapped()
        {
            var id = Guid.NewGuid();
            var siteAddress = new SiteAddressData() { Id = id };
            var addressSummary = new AddressTonnageSummary { AddressData = new List<SiteAddressData> { siteAddress } };
            var source = new SiteAddressDataToReusedOffSiteCreateSiteViewModelMapTransfer()
            {
                AatfId = Guid.NewGuid(),
                OrganisationId = Guid.NewGuid(),
                ReturnId = Guid.NewGuid(),
                Countries = new List<CountryData>(),
                ReturnedSites = addressSummary
            };

            var result = mapper.Map(source);

            result.AatfId.Should().Be(source.AatfId);
            result.OrganisationId.Should().Be(source.OrganisationId);
            result.ReturnId.Should().Be(source.ReturnId);
            result.SiteId.Should().BeNull();
            result.AddressData.Countries.Should().BeSameAs(source.Countries);
            result.HasSites.Should().BeTrue();
        }

        [Fact]
        public void Map_GivenValidSource_PropertiesShouldBeMappedWithNosites()
        {
            var addressSummary = new AddressTonnageSummary { AddressData = new List<SiteAddressData> { } };
            var source = new SiteAddressDataToReusedOffSiteCreateSiteViewModelMapTransfer()
            {
                AatfId = Guid.NewGuid(),
                OrganisationId = Guid.NewGuid(),
                ReturnId = Guid.NewGuid(),
                Countries = new List<CountryData>(),
                ReturnedSites = addressSummary
            };

            var result = mapper.Map(source);

            result.AatfId.Should().Be(source.AatfId);
            result.OrganisationId.Should().Be(source.OrganisationId);
            result.ReturnId.Should().Be(source.ReturnId);
            result.SiteId.Should().BeNull();
            result.AddressData.Countries.Should().BeSameAs(source.Countries);
            result.HasSites.Should().BeFalse();
        }

        [Fact]
        public void Map_GivenSiteAdddressHasBeenSupplied_AddressPropertiesShouldBeMapped()
        {
            var id = Guid.NewGuid();
            var siteAddress = new SiteAddressData() { Id = id };
            var addressSummary = new AddressTonnageSummary { AddressData = new List<SiteAddressData> { siteAddress } };

            var source = new SiteAddressDataToReusedOffSiteCreateSiteViewModelMapTransfer()
            {
                AatfId = Guid.NewGuid(),
                OrganisationId = Guid.NewGuid(),
                ReturnId = Guid.NewGuid(),
                Countries = new List<CountryData>(),
                SiteId = id,
                ReturnedSites = addressSummary
            };

            var result = mapper.Map(source);

            result.SiteId.Should().Be(source.SiteId);
            result.AddressData.Should().BeSameAs(siteAddress);
            result.AddressData.Countries.Should().BeSameAs(source.Countries);
        }
    }
}
