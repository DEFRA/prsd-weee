namespace EA.Weee.Core.Shared
{
    using System.ComponentModel.DataAnnotations;

    public enum EntityType
    {
        [Display(Name = "AATF")]
        Aatf = 1,
        [Display(Name = "AE")]
        Ae = 2,
        [Display(Name = "PCS")]
        Pcs = 3
    }
}
