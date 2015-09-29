namespace EA.Weee.Web.ViewModels.OrganisationRegistration
{
    using System.ComponentModel.DataAnnotations;

    public class SearchOrganisationViewModel
    {
        [Required(ErrorMessage = "No organisation name supplied, unable to perform search")]
        [Display(Name = "Enter the organisation name and search for a matching organisation")]
        public string SearchedText { get; set; }
    }
}