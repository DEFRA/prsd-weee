namespace EA.Prsd.Core.Web.OAuth
{
    using System.Threading.Tasks;
    using Thinktecture.IdentityModel.Client;

    public interface IOAuthClient
    {
        Task<TokenResponse> GetAccessTokenAsync(string username, string password);
        Task<TokenResponse> GetRefreshTokenAsync(string refreshToken);
    }
}