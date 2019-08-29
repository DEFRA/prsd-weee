namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Validation
{
    using EA.Prsd.Core;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Admin;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Xunit;

    public class AatfDetailsViewModelTests
    {
        private readonly AatfDetailsViewModel model;

        public AatfDetailsViewModelTests()
        {
            model = new AatfDetailsViewModel();
        }

        [Fact]
        public void AatfDetailsViewModel_OrganisationAddressShouldHaveAllowHtmlAttribute()
        {
            var t = typeof(AatfDetailsViewModel);
            var pi = t.GetProperty("OrganisationAddress");
            var hasAttribute = Attribute.IsDefined(pi, typeof(AllowHtmlAttribute));

            hasAttribute.Should().Be(true);
        }

        [Fact]
        public void AatfDetailsViewModel_SiteAddressShouldHaveAllowHtmlAttribute()
        {
            var t = typeof(AatfDetailsViewModel);
            var pi = t.GetProperty("SiteAddressLong");
            var hasAttribute = Attribute.IsDefined(pi, typeof(AllowHtmlAttribute));

            hasAttribute.Should().Be(true);
        }

        [Fact]
        public void AatfDetailsViewModel_ThereAreRelatedAatfs_HasAnyRelatedEntitiesIsTrue()
        {
            var model = new AatfDetailsViewModel() { AssociatedAatfs = new List<AatfDataList>() { A.Fake<AatfDataList>() } };

            model.HasAnyRelatedEntities.Should().BeTrue();
        }

        [Fact]
        public void AatfDetailsViewModel_ThereAreRelatedAes_HasAnyRelatedEntitiesIsTrue()
        {
            var model = new AatfDetailsViewModel() { AssociatedAes = new List<AatfDataList>() { A.Fake<AatfDataList>() } };

            model.HasAnyRelatedEntities.Should().BeTrue();
        }

        [Fact]
        public void AatfDetailsViewModel_ThereAreRelatedSchemes_HasAnyRelatedEntitiesIsTrue()
        {
            var model = new AatfDetailsViewModel() { AssociatedSchemes = new List<SchemeData>() { A.Fake<SchemeData>() } };

            model.HasAnyRelatedEntities.Should().BeTrue();
        }

        [Fact]
        public void AatfDetailsViewModel_PanAreaIsNotNull_HasPatAreaShouldBeTrue()
        {
            var model = new AatfDetailsViewModel() { PanArea = A.Fake<PanAreaData>() };

            model.HasPatArea.Should().BeTrue();
        }

        [Fact]
        public void AatfDetailsViewModel_LocalAreaIsNotNull_HasLocalAreaShouldBeTrue()
        {
            var model = new AatfDetailsViewModel() { LocalArea = A.Fake<LocalAreaData>() };

            model.HasLocalArea.Should().BeTrue();
        }

        [Fact]
        public void AatfDetailsViewModel_CYIsNotNull_IsLatestComplianceYearShouldBeFalse()
        {
            var model = new AatfDetailsViewModel() { ComplianceYear = A.Dummy<short>() };

            model.IsLatestComplianceYear.Should().BeFalse();
        }

        [Fact]
        public void AatfDetailsViewModel_CYIsNotEqualToLatest_IsLatestComplianceYearShouldBeTrue()
        {
            var model = new AatfDetailsViewModel() { ComplianceYear = A.Dummy<short>(), ComplianceYearList = A.CollectionOfDummy<short>(2) };

            model.IsLatestComplianceYear.Should().BeTrue();
        }

        [Fact]
        public void AatfDetailsViewModel_CYIsEqualToLatest_IsLatestComplianceYearShouldBeTrue()
        {
            var model = new AatfDetailsViewModel() { ComplianceYear = 4, ComplianceYearList = new List<short> {4, 3} };

            model.IsLatestComplianceYear.Should().BeTrue();
        }

        [Fact]
        public void AatfDetailsViewModel_CYIsNotNull_ShowCopyLinkShouldBeFalse()
        {
            var model = new AatfDetailsViewModel() { ComplianceYear = A.Dummy<short>() };

            model.ShowCopyLink.Should().BeFalse();
        }

        [Fact]
        public void AatfDetailsViewModel_RecordExistsforOneCY_ShowCopyLinkShouldBeTrue()
        {
            var currentDate = new DateTime(2019, 2, 11);
            SystemTime.Freeze(currentDate);

            var model = new AatfDetailsViewModel() { CurrentDate = currentDate, ComplianceYear = 2019, ComplianceYearList = new List<short> { 2019 } };

            model.ShowCopyLink.Should().BeTrue();

            SystemTime.Unfreeze();
        }

        [Fact]
        public void AatfDetailsViewModel_RecordExistsforAllCY_ShowCopyLinkShouldBeFalse()
        {
            var currentDate = new DateTime(2019, 2, 11);
            SystemTime.Freeze(currentDate);

            var model = new AatfDetailsViewModel() { CurrentDate = currentDate, ComplianceYear = 2019, ComplianceYearList = new List<short> { 2019, 2020 } };

            model.ShowCopyLink.Should().BeFalse();

            SystemTime.Unfreeze();
        }
    }
}
