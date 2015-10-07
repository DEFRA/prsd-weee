namespace EA.Weee.Core.Shared
{
    using System.ComponentModel.DataAnnotations;

    public enum ChargeBandType
    {
        [Display(Name = "A")]
        A = 0,

        [Display(Name = "B")]
        B = 1,

        [Display(Name = "C")]
        C = 2,

        [Display(Name = "D")]
        D = 3,

        [Display(Name = "E")]
        E = 4,
    }
}
