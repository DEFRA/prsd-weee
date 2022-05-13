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
        public void IndexGet_IsDecoratedWith_HttpGetAttribute()
        {
            typeof(ObligationsController).GetMethod("Index").Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }
    }
}
