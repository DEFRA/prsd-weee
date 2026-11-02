namespace EA.Weee.RequestHandlers.Admin.GetSchemesExceedingRetentionPeriod
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EA.Weee.DataAccess;

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
