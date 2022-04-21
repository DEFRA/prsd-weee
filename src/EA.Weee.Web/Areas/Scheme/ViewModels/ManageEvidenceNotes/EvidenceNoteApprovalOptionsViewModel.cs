namespace EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes
{
    using EA.Weee.Web.Areas.Aatf.Attributes;
    using EA.Weee.Web.ViewModels.Shared;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class EvidenceNoteApprovalOptionsViewModel : RadioButtonStringCollectionViewModel
    {
        [Required(ErrorMessage = "You must select an option")]
        public override string SelectedValue { get; set; }

        public Core.AatfEvidence.NoteStatus SelectedEnumValue
        { 
            get
            {
                return (Core.AatfEvidence.NoteStatus)System.Enum.Parse(typeof(Core.AatfEvidence.NoteStatus), SelectedValue, true);
            }
        }   

        public EvidenceNoteApprovalOptionsViewModel() : base(new List<string> { "Approved" })
        {
        }
    }
}