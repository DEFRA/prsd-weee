namespace EA.Weee.Core.Shared
{
    using System.ComponentModel.DataAnnotations;

    public enum PcsStatus
    {
        [Display(Name = "Incomplete")]
        Incomplete = 1, 
        [Display(Name = "Pending")]
        Pending = 2, 
        [Display(Name = "Approved")]
        Approved = 3, 
        [Display(Name = "Refused")]
        Refused = 4, 
        [Display(Name = "Withdrawn")]
        Withdrawn = 5
    }
}