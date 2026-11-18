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

    public class GetSchemeComplianceYearsExceedingRetentionPeriodDataAccess : IGetSchemeComplianceYearsExceedingRetentionPeriodDataAccess
    {
        private readonly WeeeContext context;

        public GetSchemeComplianceYearsExceedingRetentionPeriodDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<List<int>> GetItemsAsync()
        {
            return await context.StoredProcedures.SpgSchemeComplianceYearsExceedingRetentionPeriod();
        }
    }
}
