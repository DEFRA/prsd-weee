namespace EA.Weee.Web.Areas.Producer.Filters
{
    using EA.Weee.Core;
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Web.Mvc;
    using AuthorizationContext = System.Web.Mvc.AuthorizationContext;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class AuthorizeDirectRegistrantClaimsAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        private readonly string[] claims;
        private const string RouteId = "directRegistrantId";

        public AuthorizeDirectRegistrantClaimsAttribute(params string[] claims)
        {
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
            if (!routeData.Values.TryGetValue(RouteId, out var routeIdValue))
            {
                filterContext.Result = new HttpStatusCodeResult(400, "Route ID not found");
                return;
            }

            var routeId = routeIdValue.ToString();

            if (filterContext.HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                if (claims.All(c => claimsIdentity.Claims
                        .Select(cl => cl.Value)
                        .Contains(routeId)))
                {
                    return; // User claims are authorized
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