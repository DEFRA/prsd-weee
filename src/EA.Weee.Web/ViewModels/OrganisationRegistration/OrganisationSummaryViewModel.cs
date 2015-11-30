namespace EA.Weee.Web.ViewModels.OrganisationRegistration
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using Core.Organisations;
    using Prsd.Core.Validation;

    public class OrganisationSummaryViewModel
    {
        public OrganisationSummaryViewModel()
        {
            OrganisationData = new OrganisationData();
        }

        public OrganisationData OrganisationData { get; set; }

        [MustBeTrue(ErrorMessage = "Please confirm that you have read the privacy policy")]
        public bool PrivacyPolicy { get; set; }
    }
}