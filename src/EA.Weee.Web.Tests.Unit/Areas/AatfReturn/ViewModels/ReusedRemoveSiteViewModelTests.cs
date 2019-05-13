namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.ViewModels
{
    using System;
    using System.Web.Mvc;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using FluentAssertions;
    using Xunit;

    public class ReusedRemoveSiteViewModelTests
    {
        private readonly ReusedRemoveSiteViewModel model;

        public ReusedRemoveSiteViewModelTests()
        {
            model = new ReusedRemoveSiteViewModel();
        }

        [Fact]
        public void ReusedRemoveSiteViewModel_SiteAddressShouldHaveAllowHtmlAttribute()
        {
            var t = typeof(ReusedRemoveSiteViewModel);
            var pi = t.GetProperty("SiteAddress");
            var hasAttribute = Attribute.IsDefined(pi, typeof(AllowHtmlAttribute));

            hasAttribute.Should().Be(true);
        }
    }
}
