namespace EA.Weee.RequestHandlers.Admin.GetSchemes
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.Scheme;

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
