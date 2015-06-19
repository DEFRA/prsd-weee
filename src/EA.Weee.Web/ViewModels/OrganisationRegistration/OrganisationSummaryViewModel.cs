namespace EA.Weee.Web.ViewModels.OrganisationRegistration
{
    using System;
    using Weee.Requests.Organisations;

    public class OrganisationSummaryViewModel
    {
        public OrganisationSummaryViewModel()
        {
            OrganisationData = new OrganisationData();
        }

        public OrganisationData OrganisationData { get; set; }
    }
}