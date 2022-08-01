namespace EA.Weee.Core.Admin.Obligation
{
    using CuttingEdge.Conditions;
    using System.Collections.Generic;

    public class ObligationEvidenceSummaryData
    {
        public List<ObligationEvidenceTonnageData> ObligationEvidenceValues { get; protected set; }

        public ObligationEvidenceSummaryData(List<ObligationEvidenceTonnageData> obligationEvidenceValues)
        {
            Condition.Requires(obligationEvidenceValues).IsNotNull();

            ObligationEvidenceValues = obligationEvidenceValues;
        }
    }
}
