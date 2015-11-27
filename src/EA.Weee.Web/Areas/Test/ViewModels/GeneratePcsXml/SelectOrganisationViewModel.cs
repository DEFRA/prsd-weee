namespace EA.Weee.Web.Areas.Test.ViewModels.GeneratePcsXml
{
    using Core.Organisations;
    using Core.Shared.Paging;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class SelectOrganisationViewModel
    {
        [Required]
        [DisplayName("Organisation Name")]
        public string OrganisationName { get; set; }

        public IPagedList<PublicOrganisationData> MatchingOrganisations { get; set; }
    }
}