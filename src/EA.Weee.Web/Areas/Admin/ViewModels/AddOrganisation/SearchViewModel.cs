namespace EA.Weee.Web.Areas.Admin.ViewModels.AddOrganisation
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using Core.AatfReturn;
    using Core.DataStandards;
    using Core.Shared;

    public class SearchViewModel
    {
        [Required]
        [DisplayName("Search term")]
        [MaxLength(CommonMaxFieldLengths.DefaultString)]
        public string SearchTerm { get; set; }

        public EntityType EntityType { get; set; }

        public bool IsAeOrAatf => EntityType == EntityType.Aatf || EntityType == EntityType.Ae;
    }
}