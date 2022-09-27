namespace EA.Weee.Web.Services
{
    using System;
    using System.Security.Claims;
    using System.Web;
    using Core;
    using IdentityModel;
    using Infrastructure;

    public class HttpContextService : IHttpContextService
    {
        public string GetAccessToken()
        {
            return HttpContext.Current.User.GetAccessToken();
        }

        public bool HasOrganisationClaim(Guid organisationId)
        {
            var user = HttpContext.Current.User;

            switch (user)
            {
                case null:
                    return false;
                case ClaimsPrincipal claimsPrincipal:
                    return claimsPrincipal.HasClaimValue(WeeeClaimTypes.OrganisationAccess, organisationId.ToString());
                default:
                    return false;
            }
        }
    }
}