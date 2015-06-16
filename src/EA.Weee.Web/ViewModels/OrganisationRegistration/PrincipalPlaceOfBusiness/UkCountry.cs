namespace EA.Weee.Web.ViewModels.OrganisationRegistration.PrincipalPlaceOfBusiness
{
    using System.ComponentModel.DataAnnotations;

    public enum UkCountry
    {
        [Display(Name = "England")]
        England = 1,
        [Display(Name = "Northern Ireland")]
        NorthernIreland = 2,
        [Display(Name = "Scotland")]
        Scotland = 3,
        [Display(Name = "Wales")]
        Wales = 4,
    }
}