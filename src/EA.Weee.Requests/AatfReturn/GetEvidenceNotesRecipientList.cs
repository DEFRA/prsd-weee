namespace EA.Weee.Requests.AatfReturn
{
    using CuttingEdge.Conditions;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Requests.AatfEvidence;
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class GetEvidenceNotesRecipientList : EvidenceEntityIdDisplayNameDataBase
    {
        public Guid? OrganisationId { get; }

        public Guid? AatfId { get; }

        public RecipientOrTransfer RecipientOrTransfer { get; }

        public List<NoteType> AllowedNoteTypes { get; }

        public GetEvidenceNotesRecipientList(Guid? organisationId, Guid? aatfId, int complianceYear, List<NoteStatus> allowedStatuses, List<NoteType> allowedNoteTypes) : base(complianceYear, allowedStatuses)
        {
            Condition.Requires(organisationId).IsNotNull();
            Condition.Requires(aatfId).IsNotNull();
            Condition.Requires(allowedNoteTypes).IsNotNull();
            Condition.Requires(allowedNoteTypes).IsNotEmpty();

            OrganisationId = organisationId;
            AatfId = aatfId;
            AllowedNoteTypes = allowedNoteTypes;
        }
    }
}
