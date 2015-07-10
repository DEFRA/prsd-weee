namespace EA.Weee.Web.ViewModels.OrganisationRegistration.Type
{
    using Core.Organisations;
    using Shared;

    public class OrganisationTypeViewModel
    {
        public RadioButtonStringCollectionViewModel OrganisationTypes { get; set; }

        public OrganisationTypeViewModel()
        {
            OrganisationTypes = RadioButtonStringCollectionViewModel.CreateFromEnum<OrganisationType>();
        }
    }
}