namespace EA.Weee.RequestHandlers.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.AatfReturn;
    using DataAccess;
    using Domain.AatfReturn;
    using Domain.DataReturns;

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
            return await context.Returns
                    .Include(r => r.Operator)
                    .Include(r => r.Operator.Organisation)
                    .SingleOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IList<Return>> GetByOrganisationId(Guid id)
        {
            return await context.Returns
                .Include(r => r.Operator)
                .Include(r => r.Operator.Organisation)
                .Where(r => r.Operator.Organisation.Id == id).ToListAsync();
        }

        public async Task<IList<Return>> GetByComplianceYearAndQuarter(Return @return)
        {
            return await context.Returns.Where(r => r.Quarter.Year == @return.Quarter.Year && (int)r.Quarter.Q == (int)@return.Quarter.Q && r.Operator.Id == @return.Operator.Id).ToListAsync();
        }
    }
}
