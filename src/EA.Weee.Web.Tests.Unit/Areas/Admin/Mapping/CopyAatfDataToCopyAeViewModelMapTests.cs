namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Mapping
{
    using System;
    using AutoFixture;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Admin.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Admin.ViewModels.CopyAatf;
    using FluentAssertions;
    using Xunit;

    public class CopyAatfDataToCopyAeViewModelMapTests
    {
        private readonly Fixture fixture;
        private readonly CopyAatfDataToCopyAeViewModelMap map;

        public CopyAatfDataToCopyAeViewModelMapTests()
        {
            fixture = new Fixture();
            map = new CopyAatfDataToCopyAeViewModelMap();
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
            var competentAuthorityId = "EA";
            var aatfData = CreateAatfData(competentAuthorityId);

            var result = map.Map(aatfData);
            AssertResults(aatfData, result, competentAuthorityId);
            Assert.NotNull(result.ApprovalDate);
        }

        [Fact]
        public void Map_GivenValidSource_WithNoApprovalDate_PropertiesShouldBeMapped_ApprovalDateShouldBeDefaultDateTime()
        {
            var competentAuthorityId = "EA";
            var aatfData = CreateAatfData(competentAuthorityId);
            aatfData.ApprovalDate = default(DateTime);

            var result = map.Map(aatfData);

            AssertResults(aatfData, result, competentAuthorityId);
            Assert.Null(result.ApprovalDate);
        }

        private static void AssertResults(AatfData aatfData, CopyAeViewModel result, string competentAuthorityId)
        {
            Assert.Equal(aatfData.Id, result.Id);
            Assert.Equal(aatfData.Name, result.Name);
            Assert.Equal(aatfData.ApprovalNumber, result.ApprovalNumber);
            Assert.Equal(aatfData.SiteAddress, result.SiteAddressData);
            Assert.Equal(competentAuthorityId, result.CompetentAuthorityId);
            Assert.Equal(AatfStatus.Approved.Value, result.StatusValue);
            Assert.Equal(AatfSize.Large.Value, result.SizeValue);
            Assert.Equal(FacilityType.Ae, result.FacilityType);
        }

        private AatfData CreateAatfData(string competentAuthorityId)
        {
            var competentAuthority = fixture.Build<UKCompetentAuthorityData>()
                .With(ca => ca.Abbreviation, competentAuthorityId)
                .With(ca => ca.Name, "Environment Agency")
                .Create();

            var contactData = fixture.Build<AatfContactData>().Create();

            return fixture.Build<AatfData>()
                .With(a => a.CompetentAuthority, competentAuthority)
                .With(a => a.AatfStatus, AatfStatus.Approved)
                .With(a => a.Size, AatfSize.Large)
                .With(a => a.FacilityType, FacilityType.Ae)
                .With(a => a.AatfId, Guid.NewGuid())
                .With(a => a.Contact, contactData)
                .Create();
        }
    }
}
