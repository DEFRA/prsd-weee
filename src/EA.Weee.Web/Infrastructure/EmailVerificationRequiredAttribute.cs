namespace EA.Weee.Web.Infrastructure
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Web.Mvc;
    using Thinktecture.IdentityModel.Client;
    using AuthorizationContext = System.Web.Mvc.AuthorizationContext;

    public class EmailVerificationRequiredAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.SkipAuthorisation())
            {
                return;
            }

            var identity = (ClaimsIdentity)filterContext.HttpContext.User.Identity;
            var hasEmailVerifiedClaim = identity.HasClaim(c => c.Type.Equals(JwtClaimTypes.EmailVerified));

            if (hasEmailVerifiedClaim && identity.Claims.Single(c => c.Type.Equals(JwtClaimTypes.EmailVerified)).Value.Equals("false", StringComparison.InvariantCultureIgnoreCase))
            {
                filterContext.Result = new RedirectResult("~/Account/EmailVerificationRequired");
            }
        }
    }
}