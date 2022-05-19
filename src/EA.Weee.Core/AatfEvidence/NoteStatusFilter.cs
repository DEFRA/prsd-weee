namespace EA.Weee.Core.AatfEvidence
{
    using System;
    using System.ComponentModel.DataAnnotations;

    [Serializable]
    public enum NoteStatusFilter
    {
        [Display(Name = "Submitted")]
        Submitted = 2,
        [Display(Name = "Approved")]
        Approved = 3,
        [Display(Name = "Rejected")]
        Rejected = 4,
        [Display(Name = "Void")]
        Void = 5
    }
}
