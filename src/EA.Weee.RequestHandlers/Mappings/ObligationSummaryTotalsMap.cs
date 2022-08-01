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

            var data = new ObligationEvidenceSummaryData(obligationEvidenceValues)
            {
                //ObligationTotal = source.Sum(x => x.Obligation).ToTonnageDisplay(),
                //Obligation210Total = source.Where(x => !excludedCategories.Contains((WeeeCategory)x.CategoryId)).Sum(x => x.Obligation).ToTonnageDisplay(),
                //EvidenceTotal = source.Sum(x => x.Evidence).ToTonnageEditDisplay(),
                //Evidence210Total = source.Where(x => !excludedCategories.Contains((WeeeCategory)x.CategoryId)).Sum(x => x.Evidence).ToTonnageDisplay(),
                //ReuseTotal = source.Sum(x => x.Reuse).ToTonnageEditDisplay(),
                //Reuse210Total = source.Where(x => !excludedCategories.Contains((WeeeCategory)x.CategoryId)).Sum(x => x.Reuse).ToTonnageDisplay(),
                //TransferredInTotal = source.Sum(x => x.TransferredIn).ToTonnageEditDisplay(),
                //TransferredIn210Total = source.Where(x => !excludedCategories.Contains((WeeeCategory)x.CategoryId)).Sum(x => x.TransferredIn).ToTonnageDisplay(),
                //TransferredOutTotal = source.Sum(x => x.TransferredOut).ToTonnageEditDisplay(),
                //TransferredOut210Total = source.Where(x => !excludedCategories.Contains((WeeeCategory)x.CategoryId)).Sum(x => x.TransferredOut).ToTonnageDisplay(),
                //DifferenceTotal = source.Sum(x => x.ObligationDifference).ToTonnageEditDisplay(),
                //Difference210Total = source.Where(x => !excludedCategories.Contains((WeeeCategory)x.CategoryId)).Sum(x => x.ObligationDifference).ToTonnageDisplay()
            };

            return data;
        }
    }
}
