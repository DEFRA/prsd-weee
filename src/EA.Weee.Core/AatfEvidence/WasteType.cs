namespace EA.Weee.Core.AatfEvidence
{
    using System;
    using System.ComponentModel.DataAnnotations;

    [Serializable]
    public enum WasteType
    {
        [Display(Name = "Household")]
        Household = 1,
        [Display(Name = "Non-household")]
        NonHousehold = 2
    }
}
