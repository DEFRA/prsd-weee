namespace EA.Weee.Requests.AatfEvidence
{
    using EA.Prsd.Core;
    using EA.Weee.Core.AatfEvidence;
    using System;
    using System.Collections.Generic;
    using CuttingEdge.Conditions;

    [Serializable]
    public class GetAatfNotesRequest : EvidenceNoteFilterBaseRequest
    {
        public Guid AatfId { get; set; }

        public Guid? RecipientId { get; set; }

        public WasteType? WasteTypeId { get; set; }

        public NoteStatus? NoteStatusFilter { get; set; }

        public DateTime? StartDateSubmitted { get; set; }

        public DateTime? EndDateSubmitted { get; set; }

        public int SelectedComplianceYear { get; set; }

        public GetAatfNotesRequest(Guid organisationId, Guid aatfId, List<NoteStatus> allowedStatuses, string searchRef, int selectedComplianceYear, 
            Guid? recipient, WasteType? wasteType, NoteStatus? noteStatusFilter,
            DateTime? startDateSubmitted, DateTime? endDateSubmitted)
        {
            Guard.ArgumentNotDefaultValue(() => organisationId, organisationId);
            Guard.ArgumentNotDefaultValue(() => aatfId, aatfId);
            Guard.ArgumentNotNull(() => allowedStatuses, allowedStatuses);
            Condition.Requires(allowedStatuses).IsNotEmpty();
            Condition.Requires(selectedComplianceYear).IsGreaterThan(0);

            OrganisationId = organisationId;
            AatfId = aatfId;
            SearchRef = searchRef;
            AllowedStatuses = allowedStatuses;
            RecipientId = recipient;
            WasteTypeId = wasteType;
            NoteStatusFilter = noteStatusFilter;
            StartDateSubmitted = startDateSubmitted;
            EndDateSubmitted = endDateSubmitted;
            SelectedComplianceYear = selectedComplianceYear;
        }
    }
}
