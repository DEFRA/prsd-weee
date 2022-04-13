namespace EA.Weee.Requests.AatfEvidence
{
    using System;
    using System.Collections.Generic;
    using Core.AatfEvidence;
    using Prsd.Core.Mediator;

    [Serializable]
    public class EvidenceNoteFilterBaseRequest : IRequest<List<EvidenceNoteData>>
    {
        public List<NoteStatus> AllowedStatuses { get; set; }

        public Guid OrganisationId { get; set; }
    }
}
