namespace EA.Weee.RequestHandlers.AatfReturn
{
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    public class ReturnSchemeDataAccess : IReturnSchemeDataAccess
    {
        private readonly WeeeContext context;

        public ReturnSchemeDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<Guid> Submit(ReturnScheme scheme)
        {
            context.ReturnScheme.Add(scheme);

            await context.SaveChangesAsync();

            return scheme.Id;
        }

        public async Task<List<Guid>> GetSelectedSchemesByReturnId(Guid returnId)
        {
            return await context.ReturnScheme.Where(now => now.ReturnId == returnId).Select(now => now.SchemeId).ToListAsync();
        }
    }
}
