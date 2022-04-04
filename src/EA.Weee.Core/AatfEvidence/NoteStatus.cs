namespace EA.Weee.Core.AatfEvidence
{
    using System.ComponentModel.DataAnnotations;

    public enum NoteStatus
    {
        [Display(Name = "Required")]
        Draft = 1,
        [Display(Name = "Submitted")]
        Submitted = 2
    }
}
