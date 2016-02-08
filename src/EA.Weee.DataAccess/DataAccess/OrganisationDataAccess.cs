namespace EA.Weee.DataAccess.DataAccess
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.Organisation;

    public class OrganisationDataAccess : IOrganisationDataAccess
    {
        private readonly WeeeContext context;

        public OrganisationDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<Organisation> GetBySchemeId(Guid schemeId)
        {
            var scheme = await context.Schemes
                .Include(s => s.Organisation)
                .SingleAsync(s => s.Id == schemeId);

            return scheme.Organisation;
        }
    }
}
