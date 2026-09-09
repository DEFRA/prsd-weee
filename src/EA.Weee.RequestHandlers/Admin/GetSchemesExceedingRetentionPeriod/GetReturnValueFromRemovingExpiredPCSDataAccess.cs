namespace EA.Weee.RequestHandlers.Admin.GetSchemesExceedingRetentionPeriod
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using EA.Weee.Core.Constants;
    using EA.Weee.DataAccess;
    using EA.Weee.DataAccess.StoredProcedure;
    using EA.Weee.RequestHandlers.Admin.GetSchemesExceedingRetentionPeriod;

    public class GetReturnValueFromRemovingPCSRecordsDataAccess : IGetReturnValueFromRemovingPCSRecordsDataAccess
    {
        private readonly WeeeContext context;

        public GetReturnValueFromRemovingPCSRecordsDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<int> GetItemsAsync(Guid schemeId, int complianceYear)
        {
            return await context.StoredProcedures.SpgRemovePCSRecords(schemeId, complianceYear);
        }
    }
}
