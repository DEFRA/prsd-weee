namespace EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Web.ViewModels.Shared;

    public class ReviewEvidenceNoteViewModel : RadioButtonStringCollectionViewModel
    {
        public ViewEvidenceNoteViewModel ViewEvidenceNoteViewModel { get; set; }

        [Required(ErrorMessage = "You must select an option")]
        public override string SelectedValue { get; set; }

        public Core.AatfEvidence.NoteStatus SelectedEnumValue
        {
            get
            {
                return (Core.AatfEvidence.NoteStatus)System.Enum.Parse(typeof(Core.AatfEvidence.NoteStatus), SelectedValue, true);
            }
        }

        public ReviewEvidenceNoteViewModel() : base(new List<string> { "Approved" })
        {
        }
    }
}