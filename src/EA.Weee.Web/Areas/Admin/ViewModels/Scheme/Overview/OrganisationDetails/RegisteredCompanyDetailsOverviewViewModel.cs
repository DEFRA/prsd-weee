namespace EA.Weee.Web.Areas.Admin.ViewModels.Scheme.Overview.OrganisationDetails
{
    using System;
    using Core.Organisations;

    public class RegisteredCompanyDetailsOverviewViewModel : OrganisationDetailsOverviewViewModel
    {
        public string CompanyName { get; set; }

        public string CompanyRegistrationNumber { get; set; }
    }
}