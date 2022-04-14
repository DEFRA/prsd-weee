namespace EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes
{
    using EA.Weee.Web.ViewModels.Shared;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class EvidenceNoteApprovalOptionsViewModel : RadioButtonStringCollectionViewModel
    {
        [Required(ErrorMessage = "You must select an option")]
        public override string SelectedValue { get; set; }

        public EvidenceNoteApprovalOptionsViewModel() : base(new List<string> { "Approved" })
        {
        }
    }
}