﻿namespace EA.Weee.Requests.AatfEvidence
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

        public int PageNumber { get; private set; }

        /// <summary>
        /// Defines how many users are returned for the requested page of results.
        /// </summary>
        public int PageSize { get; private set; }
    }
}
