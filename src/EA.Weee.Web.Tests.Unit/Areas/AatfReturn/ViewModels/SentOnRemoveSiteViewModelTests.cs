namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.ViewModels
{
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using FluentAssertions;
    using System;
    using System.Web.Mvc;
    using Xunit;

    public class SentOnRemoveSiteViewModelTests
    {
        private readonly SentOnRemoveSiteViewModel model;

        public SentOnRemoveSiteViewModelTests()
        {
            model = new SentOnRemoveSiteViewModel();
        }

        [Fact]
        public void SentOnRemoveSiteViewModel_SiteAddressShouldHaveAllowHtmlAttribute()
        {
            var t = typeof(SentOnRemoveSiteViewModel);
            var pi = t.GetProperty("SiteAddress");
            var hasAttribute = Attribute.IsDefined(pi, typeof(AllowHtmlAttribute));

            hasAttribute.Should().Be(true);
        }

        [Fact]
        public void SentOnRemoveSiteViewModel_OperatorAddressShouldHaveAllowHtmlAttribute()
        {
            var t = typeof(SentOnRemoveSiteViewModel);
            var pi = t.GetProperty("OperatorAddress");
            var hasAttribute = Attribute.IsDefined(pi, typeof(AllowHtmlAttribute));

            hasAttribute.Should().Be(true);
        }
    }
}
