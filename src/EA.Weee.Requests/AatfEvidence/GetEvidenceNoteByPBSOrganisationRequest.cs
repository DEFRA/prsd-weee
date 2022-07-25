namespace EA.Weee.Requests.AatfEvidence
{
    using EA.Prsd.Core;
    using EA.Weee.Core.AatfEvidence;
    using System;
    using System.Collections.Generic;
    using CuttingEdge.Conditions;

    public class GetEvidenceNoteByPbsOrganisationRequest : EvidenceNoteFilterBaseRequest
    {
        public GetEvidenceNoteByPbsOrganisationRequest(Guid organisationId,
            List<NoteStatus> allowedStatuses,
            int complianceYear,
            List<NoteType> noteTypeFilterList,
            bool transferredOut)
        {
            Guard.ArgumentNotDefaultValue(() => organisationId, organisationId);
            Guard.ArgumentNotNull(() => allowedStatuses, allowedStatuses);
            Condition.Requires(allowedStatuses).IsNotEmpty();
            Condition.Requires(complianceYear).IsGreaterThan(0);
            Condition.Requires(noteTypeFilterList);

            OrganisationId = organisationId;
            AllowedStatuses = allowedStatuses;
            ComplianceYear = complianceYear;
            NoteTypeFilterList = noteTypeFilterList;
        }
    }
}
