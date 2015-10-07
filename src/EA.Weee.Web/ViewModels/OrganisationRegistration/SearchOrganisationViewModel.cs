namespace EA.Weee.Web.ViewModels.OrganisationRegistration
{
    using System.ComponentModel.DataAnnotations;

    public class SearchOrganisationViewModel
    {
        [Required(ErrorMessage = "No organisation name supplied, unable to perform search")]
        [Display(Name = "Search for your organisation on our system. If your organisation is not listed, create a new organisation.")]
        public string SearchedText { get; set; }

        public bool ShowPerformAnotherActivityLink { get; set; }
    }
}