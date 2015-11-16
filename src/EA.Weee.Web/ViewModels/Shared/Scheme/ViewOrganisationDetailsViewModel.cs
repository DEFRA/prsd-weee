namespace EA.Weee.Web.ViewModels.Shared.Scheme
{
    using Core.Organisations;

    public class ViewOrganisationDetailsViewModel
    {
        public ViewOrganisationDetailsViewModel()
        {
            OrganisationData = new OrganisationData();
        }

        public OrganisationData OrganisationData { get; set; }
    }
}