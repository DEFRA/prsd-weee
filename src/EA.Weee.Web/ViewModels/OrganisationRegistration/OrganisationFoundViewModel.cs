namespace EA.Weee.Web.ViewModels.OrganisationRegistration
{
    using EA.Weee.Core.Organisations;
    using Shared;
    using System.ComponentModel.DataAnnotations;

    public class OrganisationFoundViewModel
    {
        public string OrganisationName { get; set; }
        public string CompanyRegistrationName { get; set; }
    }
}