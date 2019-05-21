namespace EA.Weee.Web.Areas.Admin.ViewModels.AddAatf.Type
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Core.Organisations;
    using EA.Weee.Web.ViewModels.Shared;

    public class OrganisationTypeViewModel : RadioButtonStringCollectionViewModel
    {
        public string SearchedText { get; set; }

        [Required(ErrorMessage = "Select the type of organisation")]
        public override string SelectedValue { get; set; }

        public OrganisationTypeViewModel()
            : base(CreateFromEnum<OrganisationType>().PossibleValues)
        {
        }

        public OrganisationTypeViewModel(string searchText) : this()
        {
            SearchedText = searchText;
        }
    }
}