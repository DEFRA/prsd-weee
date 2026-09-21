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

    public class GetSchemeDataByNameAndComplianceYearDataAccess : IGetSchemeDataByNameAndComplianceYearDataAccess
    {
        private readonly WeeeContext context;

        public GetSchemeDataByNameAndComplianceYearDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<List<SchemeDataExceedingRetentionPeriod>> GetItemsAsync(int? complianceYear, string schemeName)
        {
            return await context.StoredProcedures.SpgSchemeDataByNameAndComplianceYear(complianceYear, schemeName);
        }
    }
}
