namespace EA.Weee.Core.Admin
{
    using System;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Scheme;

    public class AdminEvidenceNoteData
    {
        public Guid Id { get; set; }

        public int ReferenceId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public NoteType Type { get; set; }

        public NoteStatus Status { get; set; }

        public Guid RecipientId { get; set; }

        public SchemeData SchemeData { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public int ComplianceYear { get; set; }

        public AatfData AatfData { get; set; }

        public AdminEvidenceNoteData()
        {
        }
    }
}