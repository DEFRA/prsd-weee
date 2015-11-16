namespace EA.Weee.RequestHandlers.Admin
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.Scheme;

    public class GetAllApprovedSchemesDataAccess : IGetAllApprovedSchemesDataAccess
    {
        private readonly WeeeContext context;

        public GetAllApprovedSchemesDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<List<Scheme>> GetAllApprovedSchemes()
        {
            return await context.Schemes.Where(s => s.SchemeStatus.Value == SchemeStatus.Approved.Value)
                                        .Distinct()
                                        .OrderBy(s => s.SchemeName)
                                        .ToListAsync();
        }
    }
}
