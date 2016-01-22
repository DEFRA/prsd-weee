namespace EA.Weee.DataAccess.StoredProcedure
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IStoredProcedures
    {
        Task<List<ProducerCSVData>> SpgCSVDataByOrganisationIdAndComplianceYear(Guid organisationId, int complianceYear);

        Task<List<MembersDetailsCSVData>> SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(int complianceYear);
        Task<List<MembersDetailsCSVData>> SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(int complianceYear, bool includeRemovedProducer, Guid? schemeId, Guid? competentAuthorityId);

        Task<int> SpgNextIbisTransactionNumber();
        
        Task<List<ProducerPublicRegisterCSVData>> SpgProducerPublicRegisterCSVDataByComplianceYear(int complianceYear);

        Task<List<ProducerAmendmentsHistoryCSVData>> SpgProducerAmendmentsCSVDataByPRN(string prn);

        Task<List<PCSChargesCSVData>> SpgInvoiceRunChargeBreakdown(Guid invoiceRunId);

        Task<SpgSchemeWeeeCsvResult> SpgSchemeWeeeCsvAsync(int complianceYear, string obligationType);
        Task<List<ProducerEEECSVData>> SpgProducerEEECSVDataByComplianceYearAndObligationType(int complianceYear, string obligationtype);
    }
}
