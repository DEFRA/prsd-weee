namespace EA.Weee.Requests.Admin.Obligations
{
    using CuttingEdge.Conditions;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Admin.Obligation;
    using System;

    public class GetObligationSummaryRequest : IRequest<ObligationEvidenceSummaryData>
    {
        public Guid SchemeId { get; private set; }

        public int ComplianceYear { get; private set; }

        public GetObligationSummaryRequest(Guid schemeId, int complianceYear)
        {
            Guard.ArgumentNotDefaultValue(() => schemeId, schemeId);
            Condition.Requires(complianceYear).IsGreaterThan(0);

            SchemeId = schemeId;
            ComplianceYear = complianceYear;
        }
    }
}
