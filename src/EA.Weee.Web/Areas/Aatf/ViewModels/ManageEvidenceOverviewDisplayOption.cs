namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public enum ManageEvidenceOverviewDisplayOption
    {
        [Display(Name = "evidence-summary")]
        EvidenceSummary = 0,
        [Display(Name = "edit-draft-and-returned-notes")]
        EditDraftAndReturnedNotes = 1,
        [Display(Name = "view-all-other-evidence-notes")]
        ViewAllOtherEvidenceNotes = 2
    }
}
