namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using System.Web.Mvc;
    using Web.Areas.Admin.Controllers;
    using Web.Areas.Admin.ViewModels;
    using Xunit;

    public class HomeControllerTests
    {
        [Fact]
        public void HttpGet_ChooseActivity_ShouldReturnsChooseActivityView()
        {
            var controller = new HomeController();
            var result = controller.ChooseActivity();
            var viewResult = ((ViewResult)result);
            Assert.Equal("ChooseActivity", viewResult.ViewName);
        }

        [Fact]
        public async void HttpPost_ChooseActivity_ModelIsInvalid_ShouldRedirectViewWithError()
        {
            var controller = new HomeController();
            controller.ModelState.AddModelError("Key", "Any error");

            var result = controller.ChooseActivity(new InternalUserActivityViewModel());

            Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }
    }
}
