namespace EA.Weee.Core.DirectRegistrant
{
    using System.ComponentModel.DataAnnotations;

    public enum SellingTechniqueType
    {
        [Display(Name = "Direct Selling to End User")]
        DirectSellingToEndUser = 0,

        [Display(Name = "Indirect Selling to End User")]
        IndirectSellingToEndUser = 1,

        [Display(Name = "Both Direct and Indirect Selling to End User")]
        Both = 2,

        [Display(Name = "Online marketplace")]
        OnlineMarketplace = 3,
    }
}
