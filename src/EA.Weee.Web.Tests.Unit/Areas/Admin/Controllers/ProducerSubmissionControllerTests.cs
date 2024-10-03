namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using EA.Weee.Web.Areas.Admin.Controllers;
    using FluentAssertions;
    using Web.Areas.Admin.Controllers.Base;
    using Xunit;

    public class ProducerSubmissionControllerUnitTests
    {
        [Fact]
        public void Controller_ShouldInheritFromAdminBaseController()
        {
            typeof(ProducerSubmissionController).Should().BeDerivedFrom<AdminController>();
        }
    }
}