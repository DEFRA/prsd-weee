    namespace EA.Weee.Web.Infrastructure
    {
        using System.Linq;
        using System.Security.Claims;
        using System.Web.Mvc;
        using AuthorizationContext = System.Web.Mvc.AuthorizationContext;

        public class OrganisationRequiredAttribute : AuthorizeAttribute
        {
            public override void OnAuthorization(AuthorizationContext filterContext)
            {
                var skipAuthorization = filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)
                                     || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)
                                     || filterContext.ActionDescriptor.ActionName.Equals("CreateNewOrganisation")
                                     || filterContext.ActionDescriptor.ActionName.Equals("SelectOrganisation");

                if (skipAuthorization)
                {
                    return;
                }

                var identity = (ClaimsIdentity)filterContext.HttpContext.User.Identity;
                var organisationRegistered = identity.HasClaim(c => c.Type.Equals(Requests.ClaimTypes.OrganisationId));

                if (!organisationRegistered)
                {
                    filterContext.Result = new RedirectResult("/NewOrganisation/CreateNewOrganisation");
                }
            }
        }
    }