namespace EA.Weee.Domain.Obligation
{
    using System;
    using System.ComponentModel.DataAnnotations;

    [Flags]
    public enum ObligationType
    {
        None = 0,

        [Display(Name = "B2B")]
        B2B = 1,

        [Display(Name = "B2C")]
        B2C = 2,

        [Display(Name = "Both")]
        Both = 3
    }
}
