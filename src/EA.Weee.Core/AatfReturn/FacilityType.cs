namespace EA.Weee.Core.AatfReturn
{
    using System;
    using System.ComponentModel.DataAnnotations;

    [Serializable]
    public enum FacilityType
    {
        [Display(Name = "AATF")]
        Aatf = 1,
        [Display(Name = "AE")]
        Ae = 2
    }
}
