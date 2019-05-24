namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Validation
{
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;
    using FluentAssertions;
    using System;
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
        public void ReusedRemoveSiteViewModel_SiteAddressShouldHaveAllowHtmlAttribute()
        {
            var t = typeof(AatfDetailsViewModel);
            var pi = t.GetProperty("OrganisationAddress");
            var hasAttribute = Attribute.IsDefined(pi, typeof(AllowHtmlAttribute));

            hasAttribute.Should().Be(true);
        }
    }
}
