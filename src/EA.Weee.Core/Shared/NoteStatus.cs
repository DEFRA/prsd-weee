namespace EA.Weee.Core.Shared
{
    using System;
    using System.ComponentModel.DataAnnotations;

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
        Returned = 6
    }
}
