namespace EA.Weee.Api.Client.Actions
{
    using System.Threading.Tasks;
    using Entities;

    public interface IErrorLog
    {
        Task<bool> Create(string accessToken, ErrorData errorData);

        Task<ErrorData> Get(string accessToken, string id, string applicationName = "");

        Task<PagedErrorDataList> GetList(string accessToken, int pageIndex, int pageSize, string applicationName = "");
    }
}