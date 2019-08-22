namespace EA.Weee.RequestHandlers.Admin.GetSchemes
{
    using DataAccess;
    using Domain.Scheme;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Threading.Tasks;

    public class GetSchemesDataAccess : IGetSchemesDataAccess
    {
        private readonly WeeeContext context;

        public GetSchemesDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<List<Scheme>> GetSchemes()
        {
            return await context.Schemes.ToListAsync();
        }
    }
}
