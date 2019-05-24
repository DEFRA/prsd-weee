namespace EA.Weee.Migration.Organisation.Model
{
    using System.ComponentModel.DataAnnotations;

    public enum OrganisationType
    {
        [Display(Name = "Registered company")]
        RegisteredCompany = 1,

        [Display(Name = "Partnership")]
        Partnership = 2,

        [Display(Name = "Sole trader or individual")]
        SoleTraderOrIndividual = 3
    }
}
