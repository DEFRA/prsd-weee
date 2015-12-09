namespace EA.Weee.Web.Areas.Test.ViewModels.CreatePcsMemberXmlFile
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using Core.Organisations;
    using Core.Shared.Paging;

    public class SelectOrganisationViewModel
    {
        [Required]
        [DisplayName("Organisation Name")]
        public string OrganisationName { get; set; }

        public IPagedList<PublicOrganisationData> MatchingOrganisations { get; set; }
    }
}