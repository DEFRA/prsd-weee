namespace EA.Weee.RequestHandlers.Organisations
{
    using System;
    using System.Threading.Tasks;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Organisation;

    public interface IUpdateOrganisationContactDetailsDataAccess
    {
        Task<Organisation> FetchOrganisationAsync(Guid organisationId);
        
        Task<Country> FetchCountryAsync(Guid countryId);
        
        Task SaveAsync();
    }
}
