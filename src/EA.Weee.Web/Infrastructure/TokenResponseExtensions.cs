namespace EA.Weee.Web.Infrastructure
{
    using Prsd.Core.Web.Extensions;
    using System.Security.Claims;
    using Thinktecture.IdentityModel.Client;

    public static class TokenResponseExtensions
    {
        public static ClaimsIdentity GenerateUserIdentity(this TokenResponse response)
        {
            return response.GenerateUserIdentity(Constants.WeeeAuthType);
        }
    }
}