namespace EA.Weee.Web.ViewModels.OrganisationRegistration
{
    using System;
    using Core.Organisations;
    using Prsd.Core.Validation;

    public class OrganisationSummaryViewModel
    {
        public Guid? ContactId { get; set; }

        public Guid? AddressId { get; set; }

        public OrganisationSummaryViewModel()
        {
            OrganisationData = new OrganisationData();
        }

        public OrganisationData OrganisationData { get; set; }

        [MustBeTrue(ErrorMessage = "Please confirm that you have read the privacy policy")]
        public bool PrivacyPolicy { get; set; }
    }
}