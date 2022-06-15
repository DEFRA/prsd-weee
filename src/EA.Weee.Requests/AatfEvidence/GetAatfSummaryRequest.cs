namespace EA.Weee.Requests.AatfEvidence
{
    using System;
    using Core.AatfEvidence;
    using CuttingEdge.Conditions;
    using EA.Prsd.Core;
    using Prsd.Core.Mediator;

    [Serializable]
    public class GetAatfSummaryRequest : IRequest<AatfEvidenceSummaryData>
    {
        public Guid AatfId { get; private set; }
        public int ComplianceYear { get; private set; }

        public GetAatfSummaryRequest(Guid aatfId, int complianceYear)
        {
            Guard.ArgumentNotDefaultValue(() => aatfId, aatfId);
            Condition.Requires(complianceYear).IsGreaterThan(0);

            AatfId = aatfId;
            ComplianceYear = complianceYear;
        }
    }
}
