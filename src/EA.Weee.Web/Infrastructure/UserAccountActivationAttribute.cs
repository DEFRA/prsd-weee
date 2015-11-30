namespace EA.Weee.Web.Infrastructure
{
    using EA.Weee.Core;
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Web.Mvc;
    using Thinktecture.IdentityModel.Client;
    using AuthorizationContext = System.Web.Mvc.AuthorizationContext;

    public class UserAccountActivationAttribute : AuthorizeAttribute
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
                bool userIsInternal = ((ClaimsIdentity)filterContext.HttpContext.User.Identity).HasClaim(
                    ClaimTypes.AuthenticationMethod, Claims.CanAccessInternalArea);

                if (userIsInternal)
                {
                    filterContext.Result = new RedirectResult("~/Admin/Account/AdminAccountActivationRequired");
                }
                else
                {
                    filterContext.Result = new RedirectResult("~/Account/UserAccountActivationRequired");
                }
            }
        }
    }
}