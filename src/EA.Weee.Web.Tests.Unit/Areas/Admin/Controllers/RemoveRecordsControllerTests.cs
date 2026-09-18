namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using System.Web.Mvc;
    using Api.Client;
    using EA.Weee.Web.Areas.Admin.ViewModels.RemoveRecords;
    using FakeItEasy;
    using FluentAssertions;
    using Services;
    using TestHelpers;
    using Web.Areas.Admin.Controllers;
    using Web.Areas.Admin.Controllers.Base;
    using Xunit;

    public class RemoveRecordsControllerTests
    {
        private readonly IWeeeClient weeeClient;

        public RemoveRecordsControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
        }

        [Fact]
        public void Controller_ShouldInheritFromAdminBaseController()
        {
            typeof(RemoveRecordsController).Should().BeDerivedFrom<AdminController>();
        }

        [Fact]
        public void GetChooseActivity_ReturnsView()
        {
            var result = RemoveRecordsController().ChooseActivity();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void PostChooseActivity_ModelIsInvalid_ShouldRedirectViewWithModel()
        {
            var controller = RemoveRecordsController();
            controller.ModelState.AddModelError("Key", "Any error");

            var model = new RemoveRecordsViewModel
            {
                SelectedValue = "Value"
            };
            var result = controller.ChooseActivity(model);

            Assert.IsType<ViewResult>(result);
            Assert.Equal(model, ((ViewResult)(result)).Model);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public void PostChooseActivity_RemovePCSSelected_RedirectsToRemovePCS()
        {
            var result = RemoveRecordsController().ChooseActivity(new RemoveRecordsViewModel
            {
                SelectedValue = InternalRemoveRecordsActivity.RemovePCS
            });

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("RemovePCS", routeValues["action"]);
        }

        private RemoveRecordsController RemoveRecordsController()
        {
            IAppConfiguration configService = A.Fake<IAppConfiguration>();
            var controller = new RemoveRecordsController();
            new HttpContextMocker().AttachToController(controller);
            return controller;
        }
    }
}
