namespace EA.Weee.Core.AatfEvidence
{
    using System;
    using System.Collections.Generic;
    using CuttingEdge.Conditions;

    [Serializable]
    public class AatfEvidenceSummaryData
    {
        public List<EvidenceSummaryTonnageData> EvidenceCategoryTotals { get; protected set; }

        public int NumberOfDraftNotes { get; protected set; }

        public int NumberOfSubmittedNotes { get; protected set; }

        public int NumberOfApprovedNotes { get; protected set; }

        public AatfEvidenceSummaryData(List<EvidenceSummaryTonnageData> totals,
            int numberOfDraftNotes,
            int numberOfSubmittedNotes,
            int numberOfApprovedNotes)
        {
            Condition.Requires(totals).IsNotNull();

            EvidenceCategoryTotals = totals;
            NumberOfDraftNotes = numberOfDraftNotes;
            NumberOfSubmittedNotes = numberOfSubmittedNotes;
            NumberOfApprovedNotes = numberOfApprovedNotes;
        }
    }
}
