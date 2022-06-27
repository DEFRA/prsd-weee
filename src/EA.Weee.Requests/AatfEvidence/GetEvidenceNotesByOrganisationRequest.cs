namespace EA.Weee.Requests.AatfEvidence
{
    using System;
    using System.Collections.Generic;
    using CuttingEdge.Conditions;
    using EA.Prsd.Core;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Core.Shared;

    public class GetEvidenceNotesByOrganisationRequest : EvidenceNoteFilterBaseRequest
    {
        public bool TransferredOut { get; private set; }

        public GetEvidenceNotesByOrganisationRequest(Guid organisationId, 
            List<Core.AatfEvidence.NoteStatus> allowedStatuses,
            int complianceYear, 
            NoteType noteTypeFilter,
            bool transferredOut)
        {
            Guard.ArgumentNotDefaultValue(() => organisationId, organisationId);
            Guard.ArgumentNotNull(() => allowedStatuses, allowedStatuses);
            Condition.Requires(allowedStatuses).IsNotEmpty();
            Condition.Requires(complianceYear).IsGreaterThan(0);
            Condition.Requires(noteTypeFilter);

            OrganisationId = organisationId;
            AllowedStatuses = allowedStatuses;
            ComplianceYear = complianceYear;
            NoteTypeFilter = noteTypeFilter;
            TransferredOut = transferredOut;
        }
    }
}
