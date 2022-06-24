namespace EA.Weee.Core.AatfEvidence
{
    using System;

    public abstract class EvidenceNoteDataBase
    {
        public Guid Id { get; set; }

        public NoteType Type { get; set; }

        public NoteStatus Status { get; set; }

        public int Reference { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public int ComplianceYear { get; set; }

        public DateTime? ReturnedDate { get; set; }

        public string ReturnedReason { get; set; }

        public DateTime? RejectedDate { get; set; }

        public string RejectedReason { get; set; }

        public WasteType? WasteType { get; set; }
    }
}
