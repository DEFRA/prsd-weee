namespace EA.Weee.DataAccess.StoredProcedure
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IEvidenceStoredProcedures
    {
        Task<List<AatfEvidenceSummaryTotalsData>> GetAatfEvidenceSummaryTotals(Guid aatfId, int complianceYear);
        
        Task<List<ObligationEvidenceSummaryTotalsData>> GetObligationEvidenceSummaryTotals(Guid? schemeId, Guid? organisationId, int complianceYear);

        Task<List<EvidenceNoteReportData>> GetEvidenceNoteTonnagesReport(int complianceYear, Guid? recipientOrganisationId, Guid? aatfId, bool netTonnage);

        Task<List<ObligationAndEvidenceProgressSummaryData>> GetSchemeObligationAndEvidenceProgress(Guid? pcsId, Guid? appropriateAuthorityId, Guid? organisationId,
            int complianceYear);

        Task<List<TransferNoteData>> GetTransferNoteReport(int complianceYear, Guid? organisationId);
    }
}