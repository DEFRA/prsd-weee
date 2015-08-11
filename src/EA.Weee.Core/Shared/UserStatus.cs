namespace EA.Weee.Core.Shared
{
    using System.ComponentModel.DataAnnotations;

    public enum UserStatus
    {
        [Display(Name = "Pending")]
        Pending = 1,

        [Display(Name = "Approved")]
        Approved = 2,

        [Display(Name = "Refused")]
        Refused = 3,

        [Display(Name = "Inactive")]
        Inactive = 4
    }
}
