namespace EA.Weee.Api.Client
{
    using System.Threading.Tasks;

    public interface IOAuthTokenProvider
    {
        Task<string> GetAccessTokenAsync();
    }
}