namespace EA.Weee.Requests.AatfEvidence
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfEvidence;
    using System;
    using System.Collections.Generic;

    public class GetEvidenceNotesByOrganisationRequest : EvidenceNoteFilterBaseRequest
    {
        public GetEvidenceNotesByOrganisationRequest(Guid organisationId, List<NoteStatus> allowedStatuses)
        {
            Guard.ArgumentNotDefaultValue(() => organisationId, organisationId);
            Guard.ArgumentNotNull(() => allowedStatuses, allowedStatuses);

            OrganisationId = organisationId;
            AllowedStatuses = allowedStatuses;
        }
    }
}
