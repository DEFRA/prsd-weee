namespace EA.Weee.RequestHandlers.Admin.GetAatfs
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Admin.Aatf;

    public class GetAatfsDataAccess : IGetAatfsDataAccess
    {
        private readonly WeeeContext context;
        public GetAatfsDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<Aatf> GetAatfById(Guid id)
        {
            return await context.Aatfs.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Aatf>> GetAatfs()
        {
            return await context.Aatfs.ToListAsync();
        }
    }
}