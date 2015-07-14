namespace EA.Weee.Web.ViewModels.OrganisationRegistration
{
    using Core.Organisations;

    public class OrganisationSummaryViewModel
    {
        public OrganisationSummaryViewModel()
        {
            OrganisationData = new OrganisationData();
        }

        public OrganisationData OrganisationData { get; set; }
    }
}