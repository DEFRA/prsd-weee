namespace EA.Weee.Web.ViewModels.JoinOrganisation
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Core.Organisations;
    using Core.Shared.Paging;
 
    public class SelectOrganisationViewModel
    {
        public string Name { get; set; }

        public string TradingName { get; set; }

        public string CompaniesRegistrationNumber { get; set; }

        public OrganisationType Type { get; set; }

        public IPagedList<PublicOrganisationData> MatchingOrganisations { get; set; }

        [Required]
        public Guid? Selected { get; set; }

        public Guid? OrganisationId { get; set; }
    }
}