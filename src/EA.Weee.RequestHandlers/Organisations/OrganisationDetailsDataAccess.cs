namespace EA.Weee.RequestHandlers.Organisations
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using Domain.Scheme;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Organisation;

    public class OrganisationDetailsDataAccess : IOrganisationDetailsDataAccess
    {
        private readonly WeeeContext context;
        
        public OrganisationDetailsDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<Organisation> FetchOrganisationAsync(Guid organisationId)
        {
            var organisation = await context.Organisations
                .SingleOrDefaultAsync(o => o.Id == organisationId);

            if (organisation == null)
            {
                var errorMessage = $"No organisation was found with an ID of \"{organisationId}\".";
                throw new Exception(errorMessage);
            }

            return organisation;
        }

        public Task<Scheme> FetchSchemeAsync(Guid organisationId)
        {
            return context.Schemes
                .Include(s => s.CompetentAuthority)
                .Include(s => s.Address)
                .Include(s => s.Contact)
                .FirstOrDefaultAsync(s => s.OrganisationId == organisationId);
        }

        public async Task<Country> FetchCountryAsync(Guid countryId)
        {
            var country = await context.Countries.FindAsync(countryId);

            if (country == null)
            {
                var errorMessage = $"No country was found with an ID of \"{countryId}\".";
                throw new Exception(errorMessage);
            }

            return country;
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync(); 
        }
    }
}
