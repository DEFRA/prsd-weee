namespace EA.Weee.Requests.AatfEvidence
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfEvidence;
    using System;
    using System.Collections.Generic;
    using CuttingEdge.Conditions;

    [Serializable]
    public class GetAatfNotesRequest : EvidenceNoteFilterBaseRequest
    {
        public Guid AatfId { get; set; }

        public GetAatfNotesRequest(Guid organisationId, 
            Guid aatfId, 
            List<NoteStatus> allowedStatuses,
            string searchRef)
        {
            Guard.ArgumentNotDefaultValue(() => organisationId, organisationId);
            Guard.ArgumentNotDefaultValue(() => aatfId, aatfId);
            Guard.ArgumentNotNull(() => allowedStatuses, allowedStatuses);
            Condition.Requires(allowedStatuses).IsNotEmpty();

            OrganisationId = organisationId;
            AatfId = aatfId;
            SearchRef = searchRef;
            AllowedStatuses = allowedStatuses;
        }
    }
}
