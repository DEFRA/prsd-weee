using System.Threading.Tasks;

namespace EA.Weee.Api.Client
{
    public interface IOAuthTokenProvider
    {
        Task<string> GetAccessTokenAsync();
    }
}