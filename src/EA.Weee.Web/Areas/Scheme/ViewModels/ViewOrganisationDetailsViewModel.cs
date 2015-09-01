namespace EA.Weee.Web.Areas.Scheme.ViewModels
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