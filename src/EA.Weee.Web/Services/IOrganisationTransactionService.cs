namespace EA.Weee.Web.Services
{
    using System.Threading.Tasks;

    public interface IOrganisationTransactionService
    {
        Task CaptureData<T>(string accessToken, T viewModel) where T : class;
    }
}