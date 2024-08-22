namespace EA.Weee.DataAccess.DataAccess
{
    using System.Threading.Tasks;
    using EA.Weee.Domain.Organisation;

    public interface IOrganisationTransactionDataAccess
    {
        Task<OrganisationTransaction> FindIncompleteTransactionForCurrentUserAsync();
        Task<OrganisationTransaction> UpdateOrCreateTransactionAsync(string organisationJson);
        Task<OrganisationTransaction> CompleteTransactionAsync(string organisationJson);
    }
}