﻿namespace EA.Weee.DataAccess.StoredProcedure
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IEvidenceStoredProcedures
    {
        Task<List<AatfEvidenceSummaryTotalsData>> GetAatfEvidenceSummaryTotals(Guid aatfId, int complianceYear);
        
        Task<List<ObligationEvidenceSummaryTotalsData>> GetObligationEvidenceSummaryTotals(Guid? schemeId, Guid? organisationId, int complianceYear);

        Task<List<EvidenceNoteReportData>> GetEvidenceNoteOriginalTonnagesReport(int complianceYear, Guid? originatingOrganisationId, Guid? recipientOrganisationId, Guid? aatfId, bool netTonnage);

        Task<List<InternalObligationAndEvidenceSummaryTotalsData>> GetSchemeObligationAndEvidenceTotals(Guid? pcsId,
            int complianceYear);
    }
}