namespace EA.Weee.Requests.AatfEvidence
{
    using EA.Prsd.Core;
    using EA.Weee.Core.AatfEvidence;
    using System;
    using System.Collections.Generic;
    using CuttingEdge.Conditions;

    public class GetEvidenceNotesByOrganisationRequest : EvidenceNoteFilterBaseRequest
    {
        public GetEvidenceNotesByOrganisationRequest(Guid organisationId, 
            List<NoteStatus> allowedStatuses,
            int complianceYear)
        {
            Guard.ArgumentNotDefaultValue(() => organisationId, organisationId);
            Guard.ArgumentNotNull(() => allowedStatuses, allowedStatuses);
            Condition.Requires(allowedStatuses).IsNotEmpty();
            Condition.Requires(complianceYear).IsGreaterThan(0);

            OrganisationId = organisationId;
            AllowedStatuses = allowedStatuses;
            ComplianceYear = complianceYear;
        }
    }
}
