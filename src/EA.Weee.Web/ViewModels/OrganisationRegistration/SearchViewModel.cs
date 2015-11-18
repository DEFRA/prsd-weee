namespace EA.Weee.Web.ViewModels.OrganisationRegistration
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class SearchViewModel
    {
        [Required]
        [DisplayName("Search term")]
        [MaxLength(100, ErrorMessage = "The field Search term must be a string with a maximum length of 100")]
        public string SearchTerm { get; set; }

        public Guid? SelectedOrganisationId { get; set; }

        public bool ShowPerformAnotherActivityLink { get; set; }
    }
}