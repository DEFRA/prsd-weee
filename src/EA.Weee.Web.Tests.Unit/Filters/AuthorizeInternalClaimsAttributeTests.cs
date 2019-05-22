namespace EA.Weee.Web.Tests.Unit.Filters
{
    using EA.Weee.Security;
    using EA.Weee.Web.Filters;
    using FakeItEasy;
    using System.Collections.Generic;
    using System.Net;
    using System.Security.Claims;
    using System.Web;
    using System.Web.Mvc;
    using Xunit;

    public class AuthorizeInternalClaimsAttributeTests
    {
        private readonly AuthorizationContext context;
        private readonly AuthorizeInternalClaimsAttribute attribute;

        public AuthorizeInternalClaimsAttributeTests()
        {
            this.context = A.Fake<AuthorizationContext>();
            this.attribute = new AuthorizeInternalClaimsAttribute(new string[] { Claims.InternalAdmin });

            this.context.HttpContext = A.Fake<HttpContextBase>();
        }

        [Theory]
        [InlineData(Claims.InternalAdmin, false)]
        [InlineData(Claims.CanAccessInternalArea, true)]
        public void AuthoriseUserWithClaims_ReturnsCorrectStatusCode(string claim, bool forbiddenResult)
        {
            SetUpClaimsPrincipal(claim);

            attribute.OnAuthorization(context);

            HttpStatusCodeResult result = context.Result as HttpStatusCodeResult;

            Assert.Equal(forbiddenResult, result.StatusCode == (int)HttpStatusCode.Forbidden);
        }

        private void SetUpClaimsPrincipal(string claimValue)
        {
            List<Claim> userClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Role, claimValue)
            };

            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal();
            claimsPrincipal.AddIdentity(new ClaimsIdentity(userClaims));

            A.CallTo(() => context.HttpContext.User).Returns(claimsPrincipal);
        }
    }
}
