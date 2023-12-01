namespace EA.Weee.Core.AatfEvidence
{
    using System;
    using System.ComponentModel.DataAnnotations;

    [Serializable]
    public enum NoteStatus
    {
        [Display(Name = "Draft")]
        Draft = 1,
        [Display(Name = "Submitted")]
        Submitted = 2,
        [Display(Name = "Approved")]
        Approved = 3,
        [Display(Name = "Rejected")]
        Rejected = 4,
        [Display(Name = "Void")]
        Void = 5,
        [Display(Name = "Returned")]
        Returned = 6,
        [Display(Name = "Cancelled")]
        Cancelled = 7
    }
}
