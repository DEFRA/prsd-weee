namespace EA.Weee.RequestHandlers.Mappings
{
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Admin.Obligation;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Core.Helpers;
    using EA.Weee.DataAccess.StoredProcedure;
    using System.Collections.Generic;
    using System.Linq;

    public class ObligationSummaryTotalsMap : IMap<List<ObligationEvidenceSummaryTotalsData>, ObligationEvidenceSummaryData>
    {
        public ObligationEvidenceSummaryData Map(List<ObligationEvidenceSummaryTotalsData> source)
        {
            Condition.Requires(source).IsNotNull();

            var obligationEvidenceValues = new List<ObligationEvidenceTonnageData>();

            foreach (var total in source)
            {
                obligationEvidenceValues.Add(new ObligationEvidenceTonnageData((WeeeCategory)total.CategoryId,
                    total.Obligation,
                    total.Evidence,
                    total.Reuse,
                    total.TransferredOut,
                    total.TransferredIn,
                    total.ObligationDifference));
            }

            var data = new ObligationEvidenceSummaryData(obligationEvidenceValues);

            return data;
        }
    }
}
