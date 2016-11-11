namespace EA.Weee.RequestHandlers.Admin.Reports.GetSchemeObligationDataCsv
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EA.Weee.DataAccess;
    using EA.Weee.DataAccess.StoredProcedure;

    public class GetSchemeObligationCsvDataAccess : IGetSchemeObligationCsvDataAccess
    {
        private readonly WeeeContext context;

        public GetSchemeObligationCsvDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<List<SchemeObligationCsvData>> FetchObligationsForComplianceYearAsync(int complianceYear)
        {
            return await context.StoredProcedures.SpgSchemeObligationDataCsv(complianceYear);
        }
    }
}