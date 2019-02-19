namespace EA.Weee.RequestHandlers.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.AatfReturn;

    public class AatfDataAccess : IAatfDataAccess
    {
        private readonly WeeeContext context;

        public AatfDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<List<Aatf>> GetByOrganisationId(Guid id)
        {
            return await context.Aatfs.Where(c => c.Operator.Organisation.Id == id).ToListAsync();
        }
    }
}
