﻿namespace EA.Weee.Core.DirectRegistrant
{
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using System;
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

        public string TradingName { get; set; }

        public SellingTechniqueType? SellingTechnique { get; set; }

        public IList<AdditionalCompanyDetailsData> AdditionalCompanyDetailsData { get; set; }

        public AddressData ContactAddressData { get; set; }

        public ContactData ContactData { get; set; }

        public AuthorisedRepresentitiveData AuthorisedRepresentitiveData { get; set; }

        public AddressData ServiceOfNoticeData { get; set; }

        public IList<Eee> TonnageData { get; set; }

        public bool HasPaid {get; set; }

        public SubmissionStatus Status { get; set; }

        public DateTime? RegistrationDate { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public string PaymentReference { get; set; }

        public string ProducerRegistrationNumber { get; set; }

        public Guid RegisteredProducerId { get; set; }

        public Guid DirectProducerSubmissionId { get; set; }
    }
}
