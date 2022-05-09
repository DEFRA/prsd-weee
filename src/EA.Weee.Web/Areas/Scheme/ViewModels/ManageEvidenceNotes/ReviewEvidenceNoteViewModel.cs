namespace EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using Core.AatfEvidence;
    using Microsoft.Ajax.Utilities;
    using Web.ViewModels.Shared;

    public class ReviewEvidenceNoteViewModel : RadioButtonStringCollectionViewModel, IRadioButtonHint
    {
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
                    case "Approve evidence note":
                        return NoteStatus.Approved;
                    case "Reject evidence note":
                        return NoteStatus.Rejected;
                    case "Return evidence note":
                        return NoteStatus.Returned;
                }

                return NoteStatus.Approved;
            }
        }

        public ReviewEvidenceNoteViewModel() : base(new List<string> { "Approve evidence note", "Reject evidence note", "Return evidence note" })
        {
        }

        public Dictionary<string, string> HintItems =>
            new Dictionary<string, string>
            {
                { "Approve evidence note", null },
                { "Reject evidence note", "Reject an evidence note if the evidence has been sent to you by mistake or if there is a large number of updates to make that it is quicker to create a new evidence note" },
                { "Return evidence note", "Return an evidence note if there are some minor updates to be made by the AATF" }
            };
    }
}