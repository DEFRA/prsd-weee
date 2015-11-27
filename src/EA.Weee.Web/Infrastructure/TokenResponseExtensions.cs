namespace EA.Weee.Web.Infrastructure
{
    using System.Security.Claims;
    using Prsd.Core.Web.Extensions;
    using Thinktecture.IdentityModel.Client;

    public static class TokenResponseExtensions
    {
        public static ClaimsIdentity GenerateUserIdentity(this TokenResponse response)
        {
            return response.GenerateUserIdentity(Constants.WeeeAuthType);
        }
    }
}