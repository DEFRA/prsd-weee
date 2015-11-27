namespace EA.Weee.Web.ViewModels.OrganisationRegistration
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using Core.DataStandards;

    public class SearchViewModel
    {
        [Required]
        [DisplayName("Search term")]
        [MaxLength(CommonMaxFieldLengths.DefaultString)]
        public string SearchTerm { get; set; }

        public Guid? SelectedOrganisationId { get; set; }

        public bool ShowPerformAnotherActivityLink { get; set; }
    }
}