namespace EA.Weee.Web.Areas.Admin.ViewModels.AddAatf
{
    using Core.DataStandards;
    using EA.Weee.Core.AatfReturn;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class SearchViewModel
    {
        [Required]
        [DisplayName("Search term")]
        [MaxLength(CommonMaxFieldLengths.DefaultString)]
        public string SearchTerm { get; set; }

        public FacilityType FacilityType { get; set; }
    }
}