namespace EA.Weee.DataAccess.DataAccess
{
    using EA.Weee.Domain.Organisation;
    using System.Threading.Tasks;

    public interface IOrganisationTransactionDataAccess
    {
        Task<OrganisationTransaction> FindIncompleteTransactionForCurrentUserAsync();
        Task<OrganisationTransaction> UpdateOrCreateTransactionAsync(string organisationJson);
        Task<OrganisationTransaction> CompleteTransactionAsync(Organisation organisation);
        Task DeleteAsync();
    }
}