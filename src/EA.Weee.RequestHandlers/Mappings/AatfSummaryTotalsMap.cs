namespace EA.Weee.RequestHandlers.Mappings
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.AatfEvidence;
    using Core.DataReturns;
    using CuttingEdge.Conditions;
    using DataAccess.StoredProcedure;
    using Prsd.Core.Mapper;

    internal class AatfSummaryTotalsMap : IMap<List<AatfEvidenceSummaryTotalsData>, List<EvidenceTonnageData>>
    {
        public List<EvidenceTonnageData> Map(List<AatfEvidenceSummaryTotalsData> source)
        {
            Condition.Requires(source).IsNotNull();

            return source.Select(e => new EvidenceTonnageData((WeeeCategory)e.CategoryId, e.Received, e.Reused)).ToList();
        }
    }
}
