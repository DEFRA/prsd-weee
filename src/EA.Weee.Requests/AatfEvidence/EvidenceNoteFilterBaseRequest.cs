namespace EA.Weee.Requests.AatfEvidence
{
    using System;
    using System.Collections.Generic;
    using Core.AatfEvidence;
    using Prsd.Core.Mediator;

    [Serializable]
    public class EvidenceNoteFilterBaseRequest : IRequest<EvidenceNoteSearchDataResult>
    {
        public List<NoteStatus> AllowedStatuses { get; protected set; }

        public Guid OrganisationId { get; protected set; }

        public string SearchRef { get; protected set; }

        public int ComplianceYear { get; protected set; }

        public int PageNumber { get; protected set; }

        public int PageSize { get; protected set; }

        public NoteType NoteTypeFilter { get; protected set; }

        public List<NoteType> NoteTypeFilterList { get; protected set; }

        public bool TransferredOut { get; protected set; }
    }
}
