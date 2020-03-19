namespace EA.Prsd.Core.Web.OAuth
{
    using System.Threading.Tasks;
    using IdentityModel.Client;

    public interface IOAuthClientCredentialClient
    {
        Task<TokenResponse> GetClientCredentialsAsync();
    }
}