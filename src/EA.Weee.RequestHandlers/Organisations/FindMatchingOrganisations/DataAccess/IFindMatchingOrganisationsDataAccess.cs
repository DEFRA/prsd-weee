namespace EA.Weee.RequestHandlers.Organisations.FindMatchingOrganisations.DataAccess
{
    using System;
    using System.Threading.Tasks;
    using Domain.Organisation;

    public interface IFindMatchingOrganisationsDataAccess
    {
        Task<Organisation[]> GetOrganisationsBySimpleSearchTerm(string searchTerm, Guid userId);
    }
}
