namespace EA.Weee.Web.Areas.Admin.ViewModels.AddAatf.Type
{
    using System.ComponentModel.DataAnnotations;
    using Core.Organisations;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.ViewModels.Shared;

    public class OrganisationTypeViewModel : RadioButtonStringCollectionViewModel
    {
        public string SearchedText { get; set; }

        [Required(ErrorMessage = "Select organisation type")]
        public override string SelectedValue { get; set; }

        public FacilityType FacilityType { get; set; }

        public OrganisationTypeViewModel()
            : base(CreateFromEnum<OrganisationType>().PossibleValues)
        {
        }

        public OrganisationTypeViewModel(string searchText, FacilityType facilityType) : this()
        {
            SearchedText = searchText;
            FacilityType = facilityType;
        }
    }
}