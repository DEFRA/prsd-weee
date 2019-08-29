namespace EA.Prsd.Core.Web.OpenId
{
    using System.Threading.Tasks;
    using IdentityModel.Client;

    public interface IUserInfoClient
    {
        Task<UserInfoResponse> GetUserInfoAsync(string accessToken);
    }
}