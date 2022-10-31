namespace EA.Weee.Requests.AatfEvidence.Reports
{
    using System;
    using Core.Admin;
    using CuttingEdge.Conditions;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfEvidence;

    [Serializable]
    public class GetAatfSummaryReportRequest : IRequest<CSVFileData>
    {
        public Guid AatfId { get; private set; }

        public int ComplianceYear { get; private set; }

        public GetAatfSummaryReportRequest(Guid aatfId, int complianceYear)
        {
            Guard.ArgumentNotDefaultValue(() => aatfId, aatfId);
            Condition.Requires(complianceYear).IsGreaterThan(0);

            AatfId = aatfId;
            ComplianceYear = complianceYear;
        }
    }
}
