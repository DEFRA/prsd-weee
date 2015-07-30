namespace EA.Weee.Web.Filters
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using AuthorizationContext = System.Web.Mvc.AuthorizationContext;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class AuthorizeClaimsAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        private readonly string[] claims;

        public AuthorizeClaimsAttribute(params string[] claims)
        {
            this.claims = claims;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            var claimsIdentity = filterContext.HttpContext.User.Identity as ClaimsIdentity;

            if (claimsIdentity != null)
            {
                if (claims.All(c => claimsIdentity.Claims
                    .Where(cl => cl.Type == ClaimTypes.AuthenticationMethod).Select(cl => cl.Value).Contains(c)))
                {
                    return; // User claims are authorized
                }
            }

            // User claims are not authorized
            filterContext.Result = new System.Web.Mvc.HttpStatusCodeResult((int)System.Net.HttpStatusCode.Forbidden);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAuthenticated)
            {
                filterContext.Result = new System.Web.Mvc.HttpStatusCodeResult((int)System.Net.HttpStatusCode.Forbidden);
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}