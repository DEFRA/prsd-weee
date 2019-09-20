namespace EA.Weee.RequestHandlers.AatfReturn.AatfTaskList
{
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.AatfReturn;
    using Factories;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    public class FetchAatfDataAccess : IFetchAatfDataAccess
    {
        private readonly WeeeContext context;
        private readonly IQuarterWindowFactory quarterWindowFactory;

        public FetchAatfDataAccess(WeeeContext context,
            IQuarterWindowFactory quarterWindowFactory)
        {
            this.context = context;
            this.quarterWindowFactory = quarterWindowFactory;
        }

        public async Task<List<Aatf>> FetchAatfByReturnQuarterWindow(Return @return)
        {
            var quarterWindow = await quarterWindowFactory.GetQuarterWindow(@return.Quarter);

            return await context.Aatfs.Where(a => a.Organisation.Id == @return.Organisation.Id
                                                  && a.FacilityType.Value == @return.FacilityType.Value
                                                  && a.ComplianceYear == @return.Quarter.Year
                                                  && a.ApprovalDate.HasValue
                                                  && a.ApprovalDate.Value < quarterWindow.StartDate.Date).Select(a => a).OrderBy(a => a.Name).ToListAsync();
        }

        public async Task<List<Aatf>> FetchAatfByReturnId(Guid returnId)
        {
            return await context.ReturnAatfs.Where(r => r.Return.Id.Equals(returnId)).Select(r => r.Aatf).OrderBy(r => r.Name).ToListAsync();
        }

        public async Task<Aatf> FetchByApprovalNumber(string approvalNumber, int? year = null)
        {
            if (year == null)
            {
                return await context.Aatfs.FirstOrDefaultAsync(p => p.ApprovalNumber == approvalNumber);
            }
            else
            {
                return await context.Aatfs.FirstOrDefaultAsync(p => p.ApprovalNumber == approvalNumber && p.ComplianceYear == year);
            }
        }

        public async Task<Aatf> FetchById(Guid id)
        {
            return await context.Aatfs.FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
