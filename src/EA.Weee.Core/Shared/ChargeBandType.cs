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

        [Display(Name = "A2")]
        A2 = 5,

        [Display(Name = "C2")]
        C2 = 6,

        [Display(Name = "D2")]
        D2 = 7,

        [Display(Name = "D3")]
        D3 = 8,

        [Display(Name = "NA")]
        NA = 9
    }
}
