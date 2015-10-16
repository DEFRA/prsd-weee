namespace EA.Weee.Web.ViewModels.OrganisationRegistration
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class SearchViewModel
    {
        [Required]
        [DisplayName("Search term")]
        public string SearchTerm { get; set; }

        public Guid? SelectedOrganisationId { get; set; }

        public bool ShowPerformAnotherActivityLink { get; set; }
    }
}