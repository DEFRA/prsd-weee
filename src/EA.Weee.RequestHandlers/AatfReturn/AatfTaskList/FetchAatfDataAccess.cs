namespace EA.Weee.RequestHandlers.AatfReturn.AatfTaskList
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.DataReturns;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.AatfReturn;

    public class FetchAatfDataAccess : IFetchAatfDataAccess
    {
        private readonly WeeeContext context;

        public FetchAatfDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<List<Aatf>> FetchAatfByOrganisationIdAndQuarter(Guid organisationId, int complianceYear, DateTime windowStartDate)
        {
            return await context.Aatfs.Where(a => a.Organisation.Id == organisationId 
                                                  && a.FacilityType.Value == FacilityType.Aatf.Value
                                                  && a.ComplianceYear == complianceYear
                                                  && a.ApprovalDate.HasValue
                                                  && a.ApprovalDate.Value < windowStartDate.Date).Select(a => a).ToListAsync();
        }

        public async Task<List<Aatf>> FetchAatfByReturnId(Guid returnId)
        {
            var receivedAatf = await context.WeeeReceived.Where(w => w.Return.Id == returnId).Select(w => w.Aatf).ToListAsync();

            var sentOnAatf = await context.WeeeSentOn.Where(w => w.Return.Id == returnId).Select(w => w.Aatf).ToListAsync();

            var reusedAatf = await context.WeeeReused.Where(w => w.Return.Id == returnId).Select(w => w.Aatf).ToListAsync();

            return receivedAatf.Union(sentOnAatf).Union(reusedAatf).Distinct().ToList();
        }
    }
}
