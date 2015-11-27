namespace EA.Weee.RequestHandlers.Organisations
{
    using EA.Weee.Domain;
    using EA.Weee.Domain.Organisation;
    using System;
    using System.Threading.Tasks;

    public interface IUpdateOrganisationContactDetailsDataAccess
    {
        Task<Organisation> FetchOrganisationAsync(Guid organisationId);
        
        Task<Country> FetchCountryAsync(Guid countryId);
        
        Task SaveAsync();
    }
}
