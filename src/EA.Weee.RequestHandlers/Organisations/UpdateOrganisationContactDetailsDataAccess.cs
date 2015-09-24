﻿namespace EA.Weee.RequestHandlers.Organisations
{
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Organisation;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class UpdateOrganisationContactDetailsDataAccess : IUpdateOrganisationContactDetailsDataAccess
    {
        private readonly WeeeContext context;
        
        public UpdateOrganisationContactDetailsDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<Organisation> FetchOrganisationAsync(Guid organisationId, byte[] contactRowVersion, byte[] addressRowVersion)
        {
            Organisation organisation = await context.Organisations.FindAsync(organisationId);

            if (organisation == null)
            {
                string errorMessage = string.Format("No organisation was found with an ID of \"{0}\".", organisationId);
                throw new Exception(errorMessage);
            }

            context.Entry(organisation.Contact).OriginalValues.SetValues(new { RowVersion = contactRowVersion });
            context.Entry(organisation.OrganisationAddress).OriginalValues.SetValues(new { RowVersion = addressRowVersion });

            return organisation;
        }

        public async Task<Country> FetchCountryAsync(Guid countryId)
        {
            Country country = await context.Countries.FindAsync(countryId);

            if (country == null)
            {
                string errorMessage = string.Format("No country was found with an ID of \"{0}\".", countryId);
                throw new Exception(errorMessage);
            }

            return country;
        }

        public async Task SaveAsync()
        {
            try
            {
                await context.SaveChangesAsync(); 
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new ConcurrencyException("Failed to update database.", ex);
            }
        }
    }
}
