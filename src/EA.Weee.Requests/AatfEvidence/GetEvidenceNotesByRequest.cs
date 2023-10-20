namespace EA.Weee.Requests.AatfEvidence
{
    using EA.Prsd.Core;
    using EA.Weee.Core.AatfEvidence;
    using System;
    using System.Collections.Generic;
    using CuttingEdge.Conditions;

    public class GetEvidenceNotesByRequest : EvidenceNoteFilterBaseRequest
    {
        public GetEvidenceNotesByRequest(Guid organisationId, List<NoteStatus> allowedStatuses, int complianceYear, List<NoteType> noteTypeFilterList, bool transferredOut, List<WasteType> obligationTypeList)
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
            TransferredOut = transferredOut;
            ObligationTypeFilterList = obligationTypeList;
        }
    }
}
