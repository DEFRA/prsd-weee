﻿namespace EA.Weee.Requests.AatfEvidence
{
    using System;
    using System.Collections.Generic;
    using Core.AatfEvidence;
    using EA.Weee.Core.Shared;
    using Prsd.Core.Mediator;

    [Serializable]
    public class EvidenceNoteFilterBaseRequest : IRequest<List<EvidenceNoteData>>
    {
        public List<Core.AatfEvidence.NoteStatus> AllowedStatuses { get; protected set; }

        public Guid OrganisationId { get; protected set; }

        public string SearchRef { get; protected set; }

        public int ComplianceYear { get; protected set; }

        public NoteType NoteTypeFilter { get; protected set; }
    }
}
