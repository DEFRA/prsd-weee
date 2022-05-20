namespace EA.Weee.RequestHandlers.AatfReturn
{
    using DataAccess;
    using Domain.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

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
                    .Include(r => r.Organisation)
                    .SingleOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IList<Return>> GetByOrganisationId(Guid id)
        {
            return await context.Returns
                .Include(r => r.Organisation)
                .Where(r => r.Organisation.Id == id).ToListAsync();
        }

        public async Task<IList<Return>> GetByComplianceYearAndQuarter(Return @return)
        {
            return await context.Returns.Where(r => r.Quarter.Year == @return.Quarter.Year && (int)r.Quarter.Q == (int)@return.Quarter.Q && r.Organisation.Id == @return.Organisation.Id && r.FacilityType.Value == @return.FacilityType.Value).ToListAsync();
        }

        public async Task<Return> GetByYearAndQuarter(Guid organisationId, int year, int quarter)
        {
            return await context.Returns
                    .Include(o => o.Organisation)
                    .OrderByDescending(o => o.CreatedDate)
                    .FirstOrDefaultAsync(o => o.Organisation.Id == organisationId && o.Quarter.Year == year && (int)o.Quarter.Q == quarter);
        }
    }
}
