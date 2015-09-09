namespace EA.Weee.RequestHandlers.Organisations.FindMatchingOrganisations
{
    using System.Threading.Tasks;
    using Domain.Organisation;

    public interface IFindMatchingOrganisationsDataAccess
    {
        Task<Organisation[]> GetOrganisationsBySimpleSearchTerm(string searchTerm);
    }
}
