namespace EA.Weee.Web.ViewModels.Organisation.Type
{
    using System.ComponentModel.DataAnnotations;

    public enum OrganisationTypeEnum
    {
        [Display(Name = "Sole trader or individual")]
        SoleTrader,

        [Display(Name = "Partnership")]
        Partnership,

        [Display(Name = "Registered company")]
        RegisteredCompany
    }
}