namespace EA.Weee.RequestHandlers.Scheme
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.Organisation;

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

        public async Task<IList<Domain.Scheme.Scheme>> GetCompleteSchemes(Guid id)
        {
            return await context.Schemes
                .Where(s => s.Organisation.Id == id)
                .ToListAsync();
        }

        public async Task<IList<Domain.Scheme.Scheme>> GetAllSchemes()
        {
            return await context.Schemes.ToListAsync();
        }
    }
}
