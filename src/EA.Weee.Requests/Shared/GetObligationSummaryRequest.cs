namespace EA.Weee.Requests.Shared
{
    using CuttingEdge.Conditions;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Admin.Obligation;
    using System;

    public class GetObligationSummaryRequest : IRequest<ObligationEvidenceSummaryData>
    {
        public Guid? SchemeId { get; private set; }

        public int ComplianceYear { get; private set; }

        public Guid? OrganisationId { get; private set; }

        public GetObligationSummaryRequest(Guid? schemeId, Guid? organisationId, int complianceYear)
        {
            Condition.Requires(complianceYear).IsGreaterThan(0);

            SchemeId = schemeId;
            OrganisationId = organisationId;
            ComplianceYear = complianceYear;
        }
    }
}
