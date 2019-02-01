namespace EA.Weee.RequestHandlers.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.AatfReturn;
    
    public class ReturnDataAccess : IReturnDataAccess
    {
        private readonly WeeeContext context;

        public ReturnDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<Guid> Submit(Return aatfReturn)
        {
            context.Returns.Add(aatfReturn);

            await context.SaveChangesAsync();

            return aatfReturn.Id;
        }

        public async Task<Return> GetById(Guid id)
        {
            return await context.Returns.FindAsync(id);
        }
    }
}
