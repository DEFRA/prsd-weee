namespace EA.Weee.Web.ViewModels.OrganisationRegistration
{
    using System.ComponentModel.DataAnnotations;

    public class SearchOrganisationViewModel
    {
        [Required(ErrorMessage = "No organisation name supplied, unable to perform search")]
        public string SearchedText { get; set; }

        public bool ShowPerformAnotherActivityLink { get; set; }
    }
}