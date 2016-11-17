namespace EA.Weee.DataAccess.StoredProcedure
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IStoredProcedures
    {
        Task<List<ProducerCsvData>> SpgCSVDataByOrganisationIdAndComplianceYear(Guid organisationId, int complianceYear);

        Task<List<MembersDetailsCsvData>> SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(int complianceYear, bool includeRemovedProducer,
            bool includeBrandNames, Guid? schemeId = null, Guid? competentAuthorityId = null);

        Task<int> SpgNextIbisTransactionNumber();
        
        Task<List<ProducerPublicRegisterCSVData>> SpgProducerPublicRegisterCSVDataByComplianceYear(int complianceYear);

        Task<List<ProducerAmendmentsHistoryCSVData>> SpgProducerAmendmentsCSVDataByPRN(string prn);

        Task<List<PCSChargesCSVData>> SpgInvoiceRunChargeBreakdown(Guid invoiceRunId);

        Task<SpgSchemeWeeeCsvResult> SpgSchemeWeeeCsvAsync(int complianceYear, Guid? schemeId, string obligationType);

        Task<List<ProducerEeeCsvData>> SpgProducerEeeCsvData(int complianceYear, Guid? schemeId, string obligationtype);

        Task<List<UkEeeCsvData>> SpgUKEEEDataByComplianceYear(int complianceYear);

        Task<ProducerEeeHistoryCsvData> SpgProducerEeeHistoryCsvData(string prn);

        Task<List<DataReturnSummaryCsvData>> SpgDataReturnSummaryCsv(Guid schemeId, int complianceYear);

        Task<List<SchemeObligationCsvData>> SpgSchemeObligationDataCsv(int complianceYear);

        Task<List<MissingProducerDataCsvData>> SpgMissingProducerDataCsvData(int complianceYear, string obligationType, int? quarter,
            Guid? schemeId);
    }
}
