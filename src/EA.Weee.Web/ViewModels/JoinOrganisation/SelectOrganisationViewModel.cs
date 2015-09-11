namespace EA.Weee.Web.ViewModels.JoinOrganisation
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Core.Organisations;

    public class SelectOrganisationViewModel
    {
        public string Name { get; set; }

        public string TradingName { get; set; }

        public string CompaniesRegistrationNumber { get; set; }

        public string SearchedText { get; set; }

        public OrganisationType Type { get; set; }

        public Guid? OrganisationId { get; set; }

        [Required]
        public SelectOrganisationRadioButtons Organisations { get; set; }
    }
}