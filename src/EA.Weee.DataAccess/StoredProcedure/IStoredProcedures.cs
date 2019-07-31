﻿namespace EA.Weee.DataAccess.StoredProcedure
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;
    using Domain.Admin.AatfReports;

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

        Task<List<UkNonObligatedWeeeReceivedData>> GetUkNonObligatedWeeeReceivedByComplianceYear(int complianceYear);

        Task<List<NonObligatedWeeeReceivedAtAatfData>> GetNonObligatedWeeeReceivedAtAatf(int complianceYear, string aatf);

        Task<ProducerEeeHistoryCsvData> SpgProducerEeeHistoryCsvData(string prn);

        Task<List<DataReturnSummaryCsvData>> SpgDataReturnSummaryCsv(Guid schemeId, int complianceYear);

        Task<List<SchemeObligationCsvData>> SpgSchemeObligationDataCsv(int complianceYear);

        Task<List<MissingProducerDataCsvData>> SpgMissingProducerDataCsvData(int complianceYear, string obligationType, int? quarter,
            Guid? schemeId);

        Task<List<SubmissionChangesCsvData>> SpgSubmissionChangesCsvData(Guid memberUploadId);

        Task<List<AatfSubmissionHistory>> GetAatfSubmissions(Guid aatfId);

        Task<List<AatfAeReturnData>> GetAatfAeReturnDataCsvData(int complianceYear, int quarter, int facilityType, int? returnStatus, Guid? authority, Guid? area, Guid? panArea);

        Task<List<AatfSubmissionHistory>> GetAeSubmissions(Guid aatfId);

        Task<DataTable> GetAllAatfObligatedCsvData(int complianceYear, string aatfName, string obligationType, Guid? authority, Guid? panArea, int columnType);

        Task<DataTable> GetAatfObligatedCsvData(Guid returnId, int complianceYear, int quarter, Guid aatfId);

        Task<DataSet> GetAllAatfSentOnDataCsv(int complianceYear, string aatfName, string obligationType, Guid? authority, Guid? panArea);
        
        Task<List<AatfReuseSitesData>> GetAllAatfReuseSitesCsvData(int complianceYear, Guid? authority, Guid? panArea);
    }
}
