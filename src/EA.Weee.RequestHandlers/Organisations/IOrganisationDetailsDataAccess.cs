namespace EA.Weee.RequestHandlers.Organisations
{
    using System;
    using System.Threading.Tasks;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Domain.Scheme;

    public interface IOrganisationDetailsDataAccess
    {
        Task<Organisation> FetchOrganisationAsync(Guid organisationId);

        Task<Scheme> FetchSchemeAsync(Guid organisationId);

        Task<Country> FetchCountryAsync(Guid countryId);
        
        Task SaveAsync();
    }
}
