namespace EA.Weee.RequestHandlers.Admin.GetSchemesExceedingRetentionPeriod
{
    using EA.Weee.DataAccess;
    using EA.Weee.DataAccess.StoredProcedure;
    using EA.Weee.Requests;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class GetSchemeDataByNameAndComplianceYearDataAccess : IGetSchemeDataByNameAndComplianceYearDataAccess
    {
        private readonly WeeeContext context;

        public GetSchemeDataByNameAndComplianceYearDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<List<SchemeDataExceedingRetentionPeriod>> GetItemsAsync(GetSchemeDataExceedingRetentionPeriodRequest request)
        {
            return await context.StoredProcedures.SpgSchemeDataByNameAndComplianceYear(request.ComplianceYear, request.SchemeName, request.UserId);
        }
    }
}
