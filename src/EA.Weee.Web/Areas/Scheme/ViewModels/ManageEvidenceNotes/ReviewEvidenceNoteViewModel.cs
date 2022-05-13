namespace EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes
{
    using System;
    using Core.AatfEvidence;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using Web.ViewModels.Shared;

    public class ReviewEvidenceNoteViewModel : RadioButtonStringCollectionViewModel, IRadioButtonHint, IValidatableObject
    {
        private const string ApproveEvidenceNote = "Approve evidence note";
        private const string RejectEvidenceNote = "Reject evidence note";
        private const string ReturnEvidenceNote = "Return evidence note";

        public ViewEvidenceNoteViewModel ViewEvidenceNoteViewModel { get; set; }

        [Required(ErrorMessage = "Select whether you want to approve, reject or return the evidence note")]
        public override string SelectedValue { get; set; }

        [DisplayName("What is the reason you are rejecting or returning the evidence note?")]
        public string Reason { get; set; }

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

        public ReviewEvidenceNoteViewModel() : base(new List<string> { ApproveEvidenceNote, RejectEvidenceNote, ReturnEvidenceNote })
        {
        }

        public Dictionary<string, string> HintItems =>
            new Dictionary<string, string>
            {
                { ApproveEvidenceNote, null },
                { RejectEvidenceNote, "Reject an evidence note if the evidence has been sent to you by mistake or if there is a large number of updates to make that it is quicker to create a new evidence note" },
                { ReturnEvidenceNote, "Return an evidence note if there are some minor updates to be made by the AATF" }
            };

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.SelectedEnumValue == NoteStatus.Approved && !string.IsNullOrEmpty(this.Reason))
            {
                yield return new ValidationResult("A reason can only be entered if you are rejecting or returning the evidence note. Delete any text you have entered.", new List<string>() { "reason"});
            }
        }
    }
}