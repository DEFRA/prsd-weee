﻿namespace EA.Weee.RequestHandlers.Admin.AatfReports
{
    using DataAccess;
    using Domain.AatfReturn;
    using Domain.Admin.AatfReports;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class GetAatfSubmissionHistoryDataAccess : IGetAatfSubmissionHistoryDataAccess
    {
        private readonly WeeeContext context;

        public GetAatfSubmissionHistoryDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<List<AatfSubmissionHistory>> GetItemsAsync(Guid aatfId)
        {
            var aatf = context.Aatfs.FirstOrDefault(a => a.Id.Equals(aatfId));

            if (aatf == null)
            {
                throw new ArgumentNullException(nameof(aatfId));
            }

            if (aatf.FacilityType.Equals(FacilityType.Aatf))
            {
                return await context.StoredProcedures.GetAatfSubmissions(aatfId, aatf.ComplianceYear);
            }

            return await context.StoredProcedures.GetAeSubmissions(aatfId, aatf.ComplianceYear);
        }
    }
}
