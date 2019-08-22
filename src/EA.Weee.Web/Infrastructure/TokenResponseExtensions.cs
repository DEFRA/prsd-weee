namespace EA.Weee.Web.Infrastructure
{
    using IdentityModel.Client;
    using Prsd.Core.Web.Extensions;
    using System.Security.Claims;

    public static class TokenResponseExtensions
    {
        public static ClaimsIdentity GenerateUserIdentity(this TokenResponse response)
        {
            return response.GenerateUserIdentity(Constants.WeeeAuthType);
        }
    }
}