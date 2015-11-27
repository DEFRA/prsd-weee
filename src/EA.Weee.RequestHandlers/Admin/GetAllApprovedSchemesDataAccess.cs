namespace EA.Weee.RequestHandlers.Admin
{
    using DataAccess;
    using Domain.Scheme;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

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
