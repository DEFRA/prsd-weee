namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;
    using FakeItEasy;
    using FluentAssertions;
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
    }
}
