namespace EA.Weee.Core.AatfEvidence
{
    using Scheme;
    using System;
    using System.Collections.Generic;
    using Organisations;

    public class TransferEvidenceNoteData
    {
        public Guid Id { get; set; }

        public NoteType Type { get; set; }

        public NoteStatus Status { get; set; }

        public int Reference { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public int ComplianceYear { get; set; }

        public List<TransferEvidenceNoteTonnageData> TransferEvidenceNoteTonnageData { get; set; }

        public SchemeData RecipientSchemeData { get; set; }

        public OrganisationData TransferredOrganisation { get; set; }

        public OrganisationData RecipientOrganisationData { get; set; }

        public TransferEvidenceNoteData()
        {
        }
    }
}
