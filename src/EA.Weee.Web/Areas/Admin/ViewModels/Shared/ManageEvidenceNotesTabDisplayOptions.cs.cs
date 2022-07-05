namespace EA.Weee.Web.Areas.Admin.ViewModels.Shared
{
    using System.ComponentModel.DataAnnotations;

    public enum ManageEvidenceNotesTabDisplayOptions
    {
        [Display(Name = "view-all-evidence-notes")]
        ViewAllEvidenceNotes = 0,
        [Display(Name = "view-all-evidence-transfers")]
        ViewAllEvidenceTransfers = 1,
    }
}