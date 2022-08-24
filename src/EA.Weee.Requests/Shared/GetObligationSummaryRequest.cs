namespace EA.Weee.Requests.Shared
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

        public bool InternalAccess { get; set; }

        public Guid OrganisationId { get; private set; }

        public GetObligationSummaryRequest(Guid schemeId, int complianceYear, bool internalAccess, Guid organisationId = default(Guid))
        {
            Guard.ArgumentNotDefaultValue(() => schemeId, schemeId);

            Condition.Requires(complianceYear).IsGreaterThan(0);
            Condition.Requires(internalAccess);

            SchemeId = schemeId;
            OrganisationId = organisationId;
            ComplianceYear = complianceYear;
            InternalAccess = internalAccess;
        }
    }
}
