namespace EA.Weee.Core.Admin
{
    using System;
    using Shared;

    /// <summary>
    /// Provides scheme-specific details about a producer (identified by their
    /// registration number) for a specific compliance year.
    /// </summary>
    public class ProducerDetailsScheme
    {
        public Guid RegisteredProducerId { get; set; }

        public string SchemeName { get; set; }

        public string ProducerName { get; set; }

        public string TradingName { get; set; }

        public string CompanyNumber { get; set; }

        public DateTime RegistrationDate { get; set; }

        public ObligationType ObligationType { get; set; }

        public ChargeBandType ChargeBandType { get; set; }

        public DateTime? CeasedToExist { get; set; }

        public string IsAuthorisedRepresentative { get; set; }

        public int ComplianceYear { get; set; }

        public string RegistrationNumber { get; set; }

        public bool HasSubmittedEEE { get; set; }

        public ProducerEeeDetails ProducerEeeDetails { get; set; }

        public bool CanRemoveProducer { get; set; }

        public string Address { get; set; }

        public bool IsCompany { get; set; }

        public ProducerContactDetails CorrespondentForNotices { get; set; }
    }
}
