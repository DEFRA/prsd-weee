﻿namespace EA.Weee.RequestHandlers.Scheme
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Domain.Scheme;

    public class GetSchemesDataAccess : IGetSchemesDataAccess
    {
        private readonly WeeeContext context;

        public GetSchemesDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<IList<Domain.Scheme.Scheme>> GetCompleteSchemes()
        {
            return await context.Schemes
                .Where(s => s.Organisation.OrganisationStatus.Value == OrganisationStatus.Complete.Value)
                .ToListAsync();
        }

        public async Task<Domain.Scheme.Scheme> GetSchemeBasedOnId(Guid id)
        {
            return await context.Schemes.Where(s => s.Id == id).FirstOrDefaultAsync();
        }
    }
}
