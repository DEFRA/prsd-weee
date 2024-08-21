namespace EA.Weee.Web.Tests.Unit.Controllers
{
    using Authorization;
    using FakeItEasy;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Web.Controllers;
    using Xunit;

    public class ErrorsControllerTests
    {
        private readonly IWeeeAuthorization weeeAuthorization;

        public ErrorsControllerTests()
        {
            weeeAuthorization = A.Fake<IWeeeAuthorization>();
        }

        [Fact]
        public async Task HttpGet_ErrorRedirect_UserIsNotLoggedIn_RedirectsToExternalSignIn()
        {
            A.CallTo(() => weeeAuthorization.GetAuthorizationState())
                .Returns(AuthorizationState.NotLoggedIn());

            var result = await ErrorsController().ErrorRedirect();

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("Account", routeValues["controller"]);
            Assert.Equal("SignIn", routeValues["action"]);
            Assert.Equal(string.Empty, routeValues["area"]);
        }

        [Fact]
        public async Task HttpGet_ErrorRedirect_UserIsLoggedIn_RedirectsToDefaultLoginAction()
        {
            var accessToken = "letmein";
            var defaultLoginAction = new RedirectToRouteResult(new RouteValueDictionary(new { action = "gosomewhere " }));

            A.CallTo(() => weeeAuthorization.GetAuthorizationState())
                .Returns(AuthorizationState.LoggedIn(accessToken, defaultLoginAction));

            var result = await ErrorsController().ErrorRedirect();

            Assert.Equal(defaultLoginAction, result);
        }

        private ErrorsController ErrorsController()
        {
            return new ErrorsController(weeeAuthorization);
        }
    }
}
