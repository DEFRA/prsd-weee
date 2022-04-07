namespace EA.Weee.Requests.AatfEvidence
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfEvidence;
    using System;
    using System.Collections.Generic;

    public class GetSubmittedEvidenceNotesByOrganisationIdRequest : IRequest<List<EvidenceNoteData>>
    {
        public Guid OrganisationId { get; private set; }

        public GetSubmittedEvidenceNotesByOrganisationIdRequest(Guid organisationId)
        {
            Guard.ArgumentNotDefaultValue(() => organisationId, organisationId);

            OrganisationId = organisationId;
        }
    }
}
