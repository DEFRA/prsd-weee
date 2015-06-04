namespace EA.Weee.Web.ViewModels.Organisation.Type
{
    using Shared;

    public class OrganisationTypeViewModel
    {
        public RadioButtonStringCollectionViewModel OrganisationTypes { get; set; }

        public OrganisationTypeViewModel()
        {
            OrganisationTypes = RadioButtonStringCollectionViewModel.CreateFromEnum<OrganisationTypeEnum>();
        }
    }
}