namespace EA.Weee.Core.DirectRegistrant
{
    using System.ComponentModel.DataAnnotations;
    public enum SellingTechniqueType
    {
        [Display(Name = "Direct selling to end user (mail, order, internet etc)")]
        DirectSellingToEndUser = 0,

        [Display(Name = "Indirect selling (other)")]
        IndirectSellingToEndUser = 1,

        [Display(Name = "Both Direct and Indirect Selling to End User")]
        Both = 2,

        [Display(Name = "Online marketplace")]
        OnlineMarketplace = 3,
    }
}
