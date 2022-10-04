namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Web.ViewModels.Shared;
    using ManageEvidenceNotes;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class ReviewTransferNoteViewModel : RadioButtonStringCollectionViewModel, IRadioButtonHint, IValidatableObject
    {
        private const string ApproveEvidenceNote = "Approve evidence note transfer";
        private const string RejectEvidenceNote = "Reject evidence note transfer";
        private const string ReturnEvidenceNote = "Return evidence note transfer";

        public virtual Guid OrganisationId { get; set; }

        public ViewTransferNoteViewModel ViewTransferNoteViewModel { get; set; }

        [Required(ErrorMessage = "Select whether you want to approve, reject or return the evidence note transfer")]
        public override string SelectedValue { get; set; }

        [DisplayName("What is the reason you are rejecting or returning the evidence note transfer?")]
        public virtual string Reason { get; set; }

        public string RedirectTabName { get; set; }

        public NoteStatus SelectedEnumValue
        {
            get
            {
                switch (SelectedValue)
                {
                    case ApproveEvidenceNote:
                        return NoteStatus.Approved;
                    case RejectEvidenceNote:
                        return NoteStatus.Rejected;
                    case ReturnEvidenceNote:
                        return NoteStatus.Returned;
                }

                throw new InvalidOperationException($"Unknown selected value {SelectedValue}");
            }
        }

        public ReviewTransferNoteViewModel() : base(new List<string> { ApproveEvidenceNote, RejectEvidenceNote, ReturnEvidenceNote })
        {
        }

        public Dictionary<string, string> HintItems =>
            new Dictionary<string, string>
            {
                { ApproveEvidenceNote, null },
                { RejectEvidenceNote, "Reject an evidence note transfer to have it replaced. If the note has been sent to you by mistake, it must be rejected." },
                { ReturnEvidenceNote, "Return an evidence note transfer to have amendments made to it." }
            };

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.SelectedEnumValue == NoteStatus.Approved && !string.IsNullOrEmpty(this.Reason))
            {
                yield return new ValidationResult("A reason can only be entered if you are rejecting or returning the evidence note transfer. Delete any text you have entered.", new List<string>() { "reason"});
            }
        }
    }
}