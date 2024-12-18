namespace EA.Weee.Web.Tests.Unit.Areas.Producer.Filters
{
    using EA.Weee.Web.Areas.Producer.Filters;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Xunit;

    public class AuthoriseRouteClaimsAttributeTests
    {
        private readonly System.Web.Mvc.AuthorizationContext context;
        private readonly HttpRequestBase request;
        private readonly IPrincipal principal;
        private readonly IIdentity identity;
        private readonly RouteData routeData;
        private readonly AuthorizeRouteClaimsAttribute attribute;

        public AuthoriseRouteClaimsAttributeTests()
        {
            context = A.Fake<System.Web.Mvc.AuthorizationContext>();
            var httpContext = A.Fake<HttpContextBase>();
            request = A.Fake<HttpRequestBase>();
            principal = A.Fake<IPrincipal>();
            identity = A.Fake<IIdentity>();
            routeData = new RouteData();

            A.CallTo(() => context.HttpContext).Returns(httpContext);
            A.CallTo(() => httpContext.Request).Returns(request);
            A.CallTo(() => httpContext.User).Returns(principal);
            A.CallTo(() => principal.Identity).Returns(identity);
            A.CallTo(() => context.RouteData).Returns(routeData);

            attribute = new AuthorizeRouteClaimsAttribute("testRouteId", "claim1", "claim2");
        }

        [Fact]
        public void OnAuthorization_RouteIdNotFound_SetsBadRequestResult()
        {
            // Arrange
            A.CallTo(() => identity.IsAuthenticated).Returns(true);

            // Act
            attribute.OnAuthorization(context);

            // Assert
            context.Result.Should().BeOfType<HttpStatusCodeResult>()
                .Which.StatusCode.Should().Be(400);
            context.Result.As<HttpStatusCodeResult>().StatusDescription.Should().Be("Route ID 'testRouteId' not found");
        }

        [Fact]
        public void OnAuthorization_UserNotAuthenticated_SetsForbiddenResult()
        {
            // Arrange
            routeData.Values["testRouteId"] = Guid.NewGuid();
            A.CallTo(() => identity.IsAuthenticated).Returns(false);

            // Act
            attribute.OnAuthorization(context);

            // Assert
            context.Result.Should().BeOfType<HttpUnauthorizedResult>()
                .Which.StatusCode.Should().Be(401);
        }

        [Fact]
        public void OnAuthorization_UserAuthenticatedButLacksClaims_SetsForbiddenResult()
        {
            // Arrange
            routeData.Values["testRouteId"] = "123";
            A.CallTo(() => identity.IsAuthenticated).Returns(true);
            var claimsIdentity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim("OtherClaim", "123")
            }, "test");
            A.CallTo(() => principal.Identity).Returns(claimsIdentity);

            // Act
            attribute.OnAuthorization(context);

            // Assert
            context.Result.Should().BeOfType<HttpStatusCodeResult>()
                .Which.StatusCode.Should().Be(403);
        }

        [Fact]
        public void OnAuthorization_UserAuthenticatedWithCorrectClaims_Succeeds()
        {
            // Arrange
            routeData.Values["testRouteId"] = "123";
            A.CallTo(() => identity.IsAuthenticated).Returns(true);
            var claimsIdentity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim("claim1", "123"),
                new Claim("claim2", "123")
            }, "test");
            A.CallTo(() => principal.Identity).Returns(claimsIdentity);

            // Act
            attribute.OnAuthorization(context);

            // Assert
            context.Result.Should().BeNull();
        }

        [Fact]
        public void HandleUnauthorizedRequest_UserNotAuthenticated_SetsUnauthorizedResult()
        {
            // Arrange
            A.CallTo(() => request.IsAuthenticated).Returns(false);

            // Act
            attribute.OnAuthorization(context);

            // Assert
            context.Result.Should().BeOfType<HttpUnauthorizedResult>();
        }
    }
}