namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using EA.Weee.Web.Areas.Admin.Controllers;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using FluentAssertions;
    using System.Web.Mvc;
    using Xunit;

    public class ObligationsControllerTests
    {
        [Fact]
        public void Controller_ShouldInheritFromObligationsBaseController()
        {
            typeof(ObligationsController).Should().BeDerivedFrom<ObligationsBaseController>();
        }

        [Fact]
        public void ChooseAuthorityGet_IsDecoratedWith_HttpGetAttribute()
        {
            typeof(ObligationsController).GetMethod("SelectAuthority").Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void HoldingPost_IsDecoratedWith_HttpPostAttribute()
        {
            typeof(ObligationsController).GetMethod("Holding").Should()
                .BeDecoratedWith<HttpPostAttribute>();
        }
    }
}
