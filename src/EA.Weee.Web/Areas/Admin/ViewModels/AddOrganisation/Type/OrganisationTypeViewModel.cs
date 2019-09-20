namespace EA.Weee.Web.Areas.Admin.ViewModels.AddOrganisation.Type
{
    using System.ComponentModel.DataAnnotations;
    using Core.Organisations;
    using Core.Shared;
    using Web.ViewModels.Shared;

    public class OrganisationTypeViewModel : RadioButtonStringCollectionViewModel
    {
        public string SearchedText { get; set; }

        [Required(ErrorMessage = "Select organisation type")]
        public override string SelectedValue { get; set; }

        public EntityType EntityType { get; set; }

        public OrganisationTypeViewModel()
            : base(CreateFromEnum<OrganisationType>().PossibleValues)
        {
        }

        public OrganisationTypeViewModel(string searchText, EntityType entityType) : this()
        {
            SearchedText = searchText;
            EntityType = entityType;
        }
    }
}