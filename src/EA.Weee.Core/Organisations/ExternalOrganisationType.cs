namespace EA.Weee.Core.Organisations
{
    using System.ComponentModel.DataAnnotations;

    public enum ExternalOrganisationType
    {
        [Display(Name = "Sole trader")] SoleTrader = 3,

        [Display(Name = "Partnership")] Partnership = 2,

        [Display(Name = "Registered company")] RegisteredCompany = 1
    }
}