namespace EA.Weee.Requests.Admin
{
    using System;
    using System.Collections.Generic;
    using AatfEvidence;
    using Core.AatfEvidence;
    using CuttingEdge.Conditions;
    using EA.Prsd.Core;

    public class GetAllNotesInternal : EvidenceNoteFilterBaseRequest
    {
        public Guid? RecipientIdFilter { get; set; }

        public NoteStatus? NoteStatusFilter { get; set; }

        public WasteType? ObligationTypeFilter { get; set; }

        public Guid? AatfOrganisationId { get; set; }
        
        public Guid? TransferOrganisationId { get; set; }

        public GetAllNotesInternal(List<NoteType> noteTypeFilterList, 
            List<NoteStatus> allowedStatuses, 
            int complianceYear,
            int pageNumber,
            int pageSize,
            DateTime? startDateSubmitted,
            DateTime? endDateSubmitted,
            Guid? recipientId,
            NoteStatus? noteStatus,
            WasteType? obligationType,
            Guid? aatfOrganisationId, 
            string searchRef,
            Guid? transferOrganisationId)
        {
            Guard.ArgumentNotNull(() => allowedStatuses, allowedStatuses);
            Condition.Requires(allowedStatuses).IsNotEmpty();
            Condition.Requires(noteTypeFilterList);
            Condition.Requires(complianceYear).IsGreaterThan(0);
            Condition.Requires(pageNumber).IsGreaterThan(0);
            Condition.Requires(pageSize).IsGreaterThan(0);

            AllowedStatuses = allowedStatuses;
            NoteTypeFilterList = noteTypeFilterList;
            ComplianceYear = complianceYear;
            PageNumber = pageNumber;
            PageSize = pageSize;
            StartDateSubmittedFilter = startDateSubmitted;
            EndDateSubmittedFilter = endDateSubmitted;
            RecipientIdFilter = recipientId;
            NoteStatusFilter = noteStatus;
            ObligationTypeFilter = obligationType;
            AatfOrganisationId = aatfOrganisationId;
            SearchRef = searchRef;
            TransferOrganisationId = transferOrganisationId;
        }
    }
}