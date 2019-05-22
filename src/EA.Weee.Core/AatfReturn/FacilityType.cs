namespace EA.Weee.Core.AatfReturn
{
    using System.ComponentModel.DataAnnotations;

    public enum FacilityType
    {
        [Display(Name = "AATF")]
        Aatf = 1,
        [Display(Name = "AE")]
        Ae = 2
    }
}
