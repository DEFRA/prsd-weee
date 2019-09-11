namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using FluentAssertions;
    using Web.Areas.Admin.Controllers;
    using Web.Areas.Admin.Controllers.Base;
    using Xunit;

    public class ReportsBaseControllerTests
    {
        [Fact]
        public void ReportsBaseController_ShouldInheritFromAdminController()
        {
            typeof(ReportsBaseController).Should().BeDerivedFrom<AdminController>();
        }
    }
}
