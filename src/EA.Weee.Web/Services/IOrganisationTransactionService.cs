namespace EA.Weee.Web.Services
{
    using EA.Weee.Core.Organisations;
    using System;
    using System.Threading.Tasks;

    public interface IOrganisationTransactionService
    {
        Task CaptureData<T>(string accessToken, T viewModel) where T : class;

        Task<OrganisationTransactionData> GetOrganisationTransactionData(string accessToken);

        Task DeleteOrganisationTransactionData(string accessToken);

        Task<Guid> CompleteTransaction(string accessToken);
    }
}