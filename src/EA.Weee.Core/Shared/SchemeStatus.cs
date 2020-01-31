namespace EA.Weee.Core.Shared
{
    using System;
    using System.ComponentModel.DataAnnotations;

    [Serializable]
    public enum SchemeStatus
    {
        [Display(Name = "Pending")]
        Pending = 1,
        [Display(Name = "Approved")]
        Approved = 2,
        [Display(Name = "Rejected")]
        Rejected = 3,
        [Display(Name = "Withdrawn")]
        Withdrawn = 4
    }
}