namespace EA.Weee.RequestHandlers.Organisations.FindMatchingOrganisations.DataAccess
{
    using Domain.Organisation;
    using System;
    using System.Threading.Tasks;

    public interface IFindMatchingOrganisationsDataAccess
    {
        Task<Organisation[]> GetOrganisationsBySimpleSearchTerm(string searchTerm, Guid userId);
    }
}
