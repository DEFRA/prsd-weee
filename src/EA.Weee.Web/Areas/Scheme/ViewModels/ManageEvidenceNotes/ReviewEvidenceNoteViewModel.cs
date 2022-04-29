namespace EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Web.ViewModels.Shared;

    public class ReviewEvidenceNoteViewModel : RadioButtonStringCollectionViewModel, IRadioButtonHint
    {
        public ViewEvidenceNoteViewModel ViewEvidenceNoteViewModel { get; set; }

        [Required(ErrorMessage = "Select whether you want to approve, reject or return the evidence note")]
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

        public Dictionary<string, string> HintItems
        {
            get
            {
                return new Dictionary<string, string>();
                //{
                //    { "Approved", "HintText" }
                //};
            }
        }
    }
}