namespace EA.Weee.Core.DirectRegistrant
{
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using System.Collections.Generic;

    public class SmallProducerSubmissionHistoryData
    {
        public int ComplianceYear { get; set; }

        public bool OrganisationDetailsComplete { get; set; }
        public bool ContactDetailsComplete { get; set; }
        public bool ServiceOfNoticeComplete { get; set; }
        public bool RepresentingCompanyDetailsComplete { get; set; }
        public bool EEEDetailsComplete { get; set; }

        public AddressData BusinessAddressData { get; set; }

        public string EEEBrandNames { get; set; }

        public string CompanyName { get; set; }

        public string CompanyRegistrationNumber { get; set; }

        public string TradingName { get; set; }

        public IList<AdditionalCompanyDetailsData> AdditionalCompanyDetailsData { get; set; }

        public AddressData ContactAddressData { get; set; }

        public ContactData ContactData { get; set; }

        public AuthorisedRepresentitiveData AuthorisedRepresentitiveData { get; set; }
    }
}
