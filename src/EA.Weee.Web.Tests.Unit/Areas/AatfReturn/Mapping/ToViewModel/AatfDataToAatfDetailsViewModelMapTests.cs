namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Mapping.ToViewModel
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Admin.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;
    using FluentAssertions;
    using System;
    using Xunit;

    public class AatfDataToAatfDetailsViewModelMapTests
    {
        private readonly AatfDataToAatfDetailsViewModel map;

        public AatfDataToAatfDetailsViewModelMapTests()
        {
            map = new AatfDataToAatfDetailsViewModel();
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNullExceptionExpected()
        {
            Action action = () => map.Map(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenValidSource_WithApprovalDate_PropertiesShouldBeMapped()
        {
            AatfData aatfData = new AatfData(Guid.NewGuid(), "AatfName", "12345", CreateUkCompetentAuthorityData(), AatfStatus.Approved, CreateAatfAddressData(), AatfSize.Large, DateTime.Now);

            AatfDetailsViewModel result = map.Map(aatfData);

            result.Should().BeEquivalentTo(aatfData);
            Assert.NotNull(result.ApprovalDate);
        }

        [Fact]
        public void Map_GivenValidSource_WithNoApprovalDate_PropertiesShouldBeMapped_ApprovalDateShouldBeDefaultDatetime()
        {
            AatfData aatfData = new AatfData(Guid.NewGuid(), "AatfName", "12345", CreateUkCompetentAuthorityData(), AatfStatus.Approved, CreateAatfAddressData(), AatfSize.Large, default(DateTime));

            AatfDetailsViewModel result = map.Map(aatfData);

            Assert.Equal(aatfData.Id, result.Id);
            Assert.Equal(aatfData.Name, result.Name);
            Assert.Equal(aatfData.ApprovalNumber, result.ApprovalNumber);
            Assert.Equal(aatfData.CompetentAuthority, result.CompetentAuthority);
            Assert.Equal(aatfData.AatfStatus, result.AatfStatus);
            Assert.Equal(aatfData.SiteAddress, result.SiteAddress);
            Assert.Equal(aatfData.Size, result.Size);
            Assert.Null(result.ApprovalDate);
        }

        private UKCompetentAuthorityData CreateUkCompetentAuthorityData()
        {
            return new UKCompetentAuthorityData()
            {
                Abbreviation = "EA",
                CountryId = Guid.NewGuid(),
                Name = "Environmental Agency"
            };
        }

        private AatfAddressData CreateAatfAddressData()
        {
            return new AatfAddressData("ABC", "Here", "There", "Bath", "BANES", "BA2 2PL", Guid.NewGuid(), "England");
        }
    }
}
