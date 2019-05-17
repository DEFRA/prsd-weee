namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Mapping.ToViewModel
{
    using AutoFixture;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Admin.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;
    using FluentAssertions;
    using System;
    using Xunit;

    public class AatfDataToAatfEditDetailsViewModelMapTests
    {
        private readonly Fixture fixture;
        private readonly AatfDataToAatfEditDetailsViewModelMap map;

        public AatfDataToAatfEditDetailsViewModelMapTests()
        {
            fixture = new Fixture();
            map = new AatfDataToAatfEditDetailsViewModelMap();
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
            var aatfData = CreateAatfData();

            var result = map.Map(aatfData);
            AssertResults(aatfData, result);
            Assert.NotNull(result.ApprovalDate);
        }

        [Fact]
        public void Map_GivenValidSource_WithNoApprovalDate_PropertiesShouldBeMapped_ApprovalDateShouldBeDefaultDateTime()
        {
            var aatfData = CreateAatfData();
            aatfData.ApprovalDate = default(DateTime);

            var result = map.Map(aatfData);

            AssertResults(aatfData, result);
            Assert.Null(result.ApprovalDate);
        }

        private static void AssertResults(AatfData aatfData, AatfEditDetailsViewModel result)
        {
            Assert.Equal(aatfData.Id, result.Id);
            Assert.Equal(aatfData.Name, result.Name);
            Assert.Equal(aatfData.ApprovalNumber, result.ApprovalNumber);
            Assert.Equal(aatfData.SiteAddress, result.SiteAddress);
            Assert.Equal(CompetentAuthority.England, result.CompetentAuthority);
            Assert.Equal(AatfStatusEnum.Approved, result.AatfStatus);
            Assert.Equal(AatfSizeEnum.Large, result.Size);
        }

        private AatfData CreateAatfData()
        {
            var competentAuthority = fixture.Build<UKCompetentAuthorityData>().With(ca => ca.Name, "Environment Agency").Create();
            return fixture.Build<AatfData>()
                .With(a => a.CompetentAuthority, competentAuthority)
                .With(a => a.AatfStatus, AatfStatus.Approved)
                .With(a => a.Size, AatfSize.Large)
                .Create();
        }
    }
}
