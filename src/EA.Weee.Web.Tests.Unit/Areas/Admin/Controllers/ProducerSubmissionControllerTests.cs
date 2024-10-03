namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using FluentAssertions;
    using Web.Areas.Admin.Controllers.Base;
    using Xunit;

    public class ProducerSubmissionController
    {
        [Fact]
        public void Controller_ShouldInheritFromAdminBaseController()
        {
            typeof(ProducerSubmissionController).Should().BeDerivedFrom<AdminController>();
        }
    }
}