namespace EA.Weee.Requests.Admin.Obligations
{
    using CuttingEdge.Conditions;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Admin.Obligation;
    using System;

    public class GetObligationSummaryRequest : IRequest<ObligationEvidenceSummaryData>
    {
        public Guid PcsId { get; private set; }
        public Guid OrganisationId { get; private set; }
        public int ComplianceYear { get; private set; }

        public GetObligationSummaryRequest(Guid pcsId, Guid organisationId, int complianceYear)
        {
            Guard.ArgumentNotDefaultValue(() => pcsId, pcsId);
            Guard.ArgumentNotDefaultValue(() => organisationId, organisationId);
            Condition.Requires(complianceYear).IsGreaterThan(0);

            PcsId = pcsId;
            OrganisationId = organisationId;
            ComplianceYear = complianceYear;
        }
    }
}
