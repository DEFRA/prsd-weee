﻿namespace EA.Weee.Requests.AatfEvidence
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfEvidence;
    using System;
    using System.Collections.Generic;
 
    [Serializable]
    public class GetAatfNotesRequest : IRequest<List<EvidenceNoteData>>
    {
        public Guid OrganisationId { get; set; }

        public Guid AatfId { get; set; }

        public List<NoteStatus> AllowedStatuses { get; set; }

        public GetAatfNotesRequest(Guid organisationId, Guid aatfId, List<NoteStatus> allowedStatuses)
        {
            Guard.ArgumentNotDefaultValue(() => organisationId, organisationId);
            Guard.ArgumentNotDefaultValue(() => aatfId, aatfId);
            Guard.ArgumentNotNull(() => allowedStatuses, allowedStatuses);

            OrganisationId = organisationId;
            AatfId = aatfId;

            AllowedStatuses = allowedStatuses;
        }
    }
}
