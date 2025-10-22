namespace EA.Weee.Domain.Lookup
{
    using System.ComponentModel.DataAnnotations;

    public enum CompetentAuthorityType
    {
        [Display(Name = "Non-UK")]
        NonUK = 0,

        [Display(Name = "England")]
        England = 1,

        [Display(Name = "Wales")]
        Wales = 2,

        [Display(Name = "Scotland")]
        Scotland = 3,

        [Display(Name = "Northern Ireland")]
        NorthernIreland = 4
    }
}
