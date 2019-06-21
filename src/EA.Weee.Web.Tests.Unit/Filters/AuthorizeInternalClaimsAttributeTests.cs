namespace EA.Weee.Web.Tests.Unit.Filters
{
    using EA.Weee.Security;
    using EA.Weee.Web.Filters;
    using FakeItEasy;
    using System.Collections.Generic;
    using System.Net;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Web;
    using System.Web.Mvc;
    using FluentAssertions;
    using Prsd.Core.Domain;
    using Xunit;

    public class AuthorizeInternalClaimsAttributeTests
    {
        private readonly AuthorizationContext context;
        private readonly AuthorizeInternalClaimsAttribute attribute;
        private readonly ClaimsIdentity claimsIdentity;

        public AuthorizeInternalClaimsAttributeTests()
        {
            this.attribute = new AuthorizeInternalClaimsAttribute(new string[] { "InternalAdmin" });

            this.context = A.Fake<AuthorizationContext>();
            HttpContextBase httpContextBase = A.Fake<HttpContextBase>();
            ClaimsPrincipal principal = A.Fake<ClaimsPrincipal>();
            this.claimsIdentity = A.Fake<ClaimsIdentity>();

            A.CallTo(() => httpContextBase.User).Returns(principal);
            A.CallTo(() => httpContextBase.User.Identity).Returns(claimsIdentity);
            A.CallTo(() => claimsIdentity.IsAuthenticated).Returns(true);
            A.CallTo(() => this.context.HttpContext).Returns(httpContextBase);
        }

        [Fact]
        public void AuthoriseUserWithClaims_GivenUserDoesNotHaveClaim_HttpStatusCodeShouldBeForbidden()
        {
            SetUpClaimsPrincipal("Internal");

            attribute.OnAuthorization(context);

            HttpStatusCodeResult result = context.Result as HttpStatusCodeResult;

            result.StatusCode.Should().Be((int)HttpStatusCode.Forbidden);
        }

        [Fact]
        public void AuthoriseUserWithClaims_GivenUserHasClaim_ResultShouldBeNull()
        {
            SetUpClaimsPrincipal("InternalAdmin");

            attribute.OnAuthorization(context);

            HttpStatusCodeResult result = context.Result as HttpStatusCodeResult;

            result.Should().BeNull();
        }

        private void SetUpClaimsPrincipal(string claimValue)
        {
            List<Claim> userClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Role, claimValue)
            };

            A.CallTo(() => this.claimsIdentity.Claims).Returns(userClaims);
        }
    }
}
