namespace EA.Weee.Core.Shared
{
    using System.ComponentModel.DataAnnotations;

    public enum UserStatus
    {
        [Display(Name = "Pending")]
        Pending = 1,

        [Display(Name = "Active")]
        Active = 2,

        [Display(Name = "Rejected")]
        Rejected = 3,

        [Display(Name = "Inactive")]
        Inactive = 4
    }
}
