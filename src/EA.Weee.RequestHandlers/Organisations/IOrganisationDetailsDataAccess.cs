namespace EA.Weee.RequestHandlers.Organisations
{
    using EA.Weee.Domain;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Domain.Scheme;
    using System;
    using System.Threading.Tasks;

    public interface IOrganisationDetailsDataAccess
    {
        Task<Organisation> FetchOrganisationAsync(Guid organisationId);

        Task<Scheme> FetchSchemeAsync(Guid organisationId);

        Task<Country> FetchCountryAsync(Guid countryId);

        Task SaveAsync();
    }
}
