namespace EA.Weee.Web.Areas.Producer.Filters
{
    using EA.Weee.Core;
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Web.Mvc;
    using AuthorizationContext = System.Web.Mvc.AuthorizationContext;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class AuthorizeRouteClaimsAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        private readonly string[] claims;
        private readonly string routeIdParam;

        public AuthorizeRouteClaimsAttribute(string routeIdParam, params string[] claims)
        {
            this.routeIdParam = routeIdParam;
            this.claims = claims;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (filterContext.Result is HttpUnauthorizedResult)
            {
                // The base authorization failed, so we don't need to do additional checks
                return;
            }

            var routeData = filterContext.RouteData;
            if (!routeData.Values.TryGetValue(routeIdParam, out var routeIdValue))
            {
                filterContext.Result = new HttpStatusCodeResult(400, $"Route ID '{routeIdParam}' not found");
                return;
            }

            var routeId = routeIdValue.ToString();

            if (filterContext.HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                if (claims.All(claimType =>
                        claimsIdentity.HasClaim(c => c.Type == claimType && string.Equals(c.Value, routeId, StringComparison.InvariantCultureIgnoreCase))))
                {
                    return;
                }
            }

            // User claims are not authorized
            filterContext.Result = new HttpStatusCodeResult((int)System.Net.HttpStatusCode.Forbidden);
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