namespace EA.Weee.Requests.Organisations
{
    using System.ComponentModel.DataAnnotations;

    public enum Type
    {
        [Display(Name = "Registered company")]
        RegisteretCompany = 1,
        [Display(Name = "Partnership")]
        Partnership = 2,
        [Display(Name = "Sole trader or individual")]
        SoleTraderOrIndividual = 3
    }
}