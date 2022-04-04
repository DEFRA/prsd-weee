namespace EA.Weee.Requests.AatfEvidence
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mediator;
    using System;
    using System.Collections.Generic;
    using Core.AatfEvidence;

    [Serializable]
    public class GetAatfNotesRequest : IRequest<List<EvidenceNoteData>>
    {
        public Guid OrganisationId { get; set; }

        public Guid AatfId { get; set; }

        public GetAatfNotesRequest(Guid organisationId, Guid aatfId)
        {
            Guard.ArgumentNotDefaultValue(() => organisationId, organisationId);
            Guard.ArgumentNotDefaultValue(() => aatfId, aatfId);

            OrganisationId = organisationId;
            AatfId = aatfId;
        }
    }
}
