namespace EA.Weee.Core.AatfEvidence
{
    using System;

    public abstract class EvidenceNoteDataBase
    {
        public virtual Guid Id { get; set; }

        public virtual NoteType Type { get; set; }

        public virtual NoteStatus Status { get; set; }

        public int Reference { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public int ComplianceYear { get; set; }

        public DateTime? ReturnedDate { get; set; }

        public string ReturnedReason { get; set; }

        public DateTime? RejectedDate { get; set; }

        public string RejectedReason { get; set; }

        public DateTime? VoidedDate { get; set; }

        public string VoidedReason { get; set; }

        public WasteType? WasteType { get; set; }
    }
}
