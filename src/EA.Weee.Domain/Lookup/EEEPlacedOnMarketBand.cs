namespace EA.Weee.Domain.Lookup
{
    using System.ComponentModel.DataAnnotations;

    public enum EEEPlacedOnMarketBand
    {
        [Display(Name = "More than or equal to 5 tonnes EEE placed on market")]
        Morethanorequalto5TEEEplacedonmarket = 0,

        [Display(Name = "Less than 5 tonnes EEE placed on market")]
        Lessthan5TEEEplacedonmarket = 1,
    }
}
