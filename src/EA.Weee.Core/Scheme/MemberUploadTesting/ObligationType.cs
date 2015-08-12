namespace EA.Weee.Core.Scheme.MemberUploadTesting
{
    using System.ComponentModel.DataAnnotations;

    public enum ObligationType
    {  
        [Display(Name = "B2B")]
        B2B = 1,

        [Display(Name = "B2C")]
        B2C = 2,

        [Display(Name = "Both")]
        Both = 3
    }
}
