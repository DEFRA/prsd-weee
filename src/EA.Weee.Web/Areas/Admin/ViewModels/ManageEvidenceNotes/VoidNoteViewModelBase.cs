namespace EA.Weee.Web.Areas.Admin.ViewModels.ManageEvidenceNotes
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public abstract class VoidNoteViewModelBase
    {
        [DisplayName("Reason")]
        [Required(ErrorMessage = "Enter a reason for voiding the note")]
        [StringLength(200)]
        public string VoidedReason { get; set; }
    }
}