namespace EA.Weee.Core.Organisations
{
    using System.ComponentModel.DataAnnotations;

    public enum ExternalOrganisationType
    {
        [Display(Name = "Sole trader")]
        SoleTrader = 1,

        [Display(Name = "Partnership")]
        Partnership = 2,

        [Display(Name = "Registered company")]
        RegisteredCompany = 3
    }
}