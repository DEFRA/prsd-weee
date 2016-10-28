namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using FakeItEasy;
    using Web.Areas.Admin.Controllers;
    using Web.Areas.Admin.ViewModels.TestEmail;
    using Weee.Requests.Admin;
    using Xunit;

    public class TestEmailControllerTests
    {
        private readonly IWeeeClient weeeClient;

        public TestEmailControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
        }

        [Fact]
        public void PostIndex_InvokeApi()
        {
            // Arrange
            var controller = TestEmailController();

            // Act
            var result = controller.Index(A.Dummy<TestEmailViewModel>());

            // Assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<SendTestEmail>._))
                .MustHaveHappened();
        }

        [Fact]
        public async Task PostIndex_InvokeApiWithSuccess_ReturnsSuccess()
        {
            // Arrange
            var controller = TestEmailController();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<SendTestEmail>._))
                .Returns(true);

            // Act
            var result = await controller.Index(A.Dummy<TestEmailViewModel>());

            // Assert
            Assert.NotNull(result);
            Assert.IsType<RedirectToRouteResult>(result);

            var redirectValues = ((RedirectToRouteResult)result).RouteValues;
            Assert.Equal("Success", redirectValues["action"]);
        }

        [Fact]
        public async Task PostIndex_InvokeApiWithFailure_ReturnsError()
        {
            // Arrange
            var controller = TestEmailController();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<SendTestEmail>._))
                .Returns(false);

            // Act
            var result = await controller.Index(new TestEmailViewModel());

            // Assert
            Assert.NotNull(result);

            var model = ((ViewResult)result).Model;
            Assert.IsType<TestEmailViewModel>(model);

            Assert.False(controller.ModelState.IsValid);
        }

        public TestEmailController TestEmailController()
        {
            return new TestEmailController(() => weeeClient);
        }
    }
}
