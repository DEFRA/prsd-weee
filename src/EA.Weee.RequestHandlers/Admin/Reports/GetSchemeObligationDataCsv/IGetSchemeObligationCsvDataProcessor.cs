namespace EA.Weee.RequestHandlers.Admin.Reports.GetSchemeObligationDataCsv
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DataAccess.StoredProcedure;

    public interface IGetSchemeObligationCsvDataProcessor
    {
        Task<List<SchemeObligationCsvData>> FetchObligationsForComplianceYearAsync(int complianceYear);
    }
}