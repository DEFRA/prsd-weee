namespace EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes
{
    using System.ComponentModel.DataAnnotations;

    public enum ManageEvidenceNotesDisplayOptions
    {
        [Display(Name = "evidence-summary")]
        Summary = 0,
        [Display(Name = "review-submitted-evidence")]
        ReviewSubmittedEvidence = 1,
        [Display(Name = "view-and-transfer-evidence")]
        ViewAndTransferEvidence = 2,
        [Display(Name = "outgoing-transfers")]
        OutgoingTransfers = 3,
    }
}