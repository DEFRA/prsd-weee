namespace EA.Weee.RequestHandlers.Admin.Reports.GetSchemeObligationDataCsv
{
    using DataAccess.StoredProcedure;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IGetSchemeObligationCsvDataProcessor
    {
        Task<List<SchemeObligationCsvData>> FetchObligationsForComplianceYearAsync(int complianceYear);
    }
}