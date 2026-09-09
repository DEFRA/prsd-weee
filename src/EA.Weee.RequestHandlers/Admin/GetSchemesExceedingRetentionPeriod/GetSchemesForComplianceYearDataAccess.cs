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

    public class GetSchemesForComplianceYearDataAccess : IGetSchemesForComplianceYearDataAccess
    {
        private readonly WeeeContext context;

        public GetSchemesForComplianceYearDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<List<string>> GetItemsAsync(int? complianceYear)
        {
            return await context.StoredProcedures.SpgSchemeNamesForComplianceYear(complianceYear);
        }
    }
}
