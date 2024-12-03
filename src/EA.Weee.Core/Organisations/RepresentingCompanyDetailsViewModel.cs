namespace EA.Weee.Core.Organisations
{
    using EA.Weee.Core.DataStandards;
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class RepresentingCompanyDetailsViewModel : IProducerSubmissionViewModel
    {
        public Guid DirectRegistrantId { get; set; }

        public Guid OrganisationId { get; set; }

        [Required]
        [DisplayName("Producer name")]
        [StringLength(EnvironmentAgencyMaxFieldLengths.ProducerName)]
        public string CompanyName { get; set; }

        [DisplayName("Trading name")]
        [StringLength(EnvironmentAgencyMaxFieldLengths.ProducerTradingName)]
        public string BusinessTradingName { get; set; }

        public RepresentingCompanyAddressData Address { get; set; } = new RepresentingCompanyAddressData();

        public bool? RedirectToCheckAnswers { get; set; }
        public bool HasAuthorisedRepresentitive { get; set; }
        public bool NpwdMigrated { get; set; }
    }
}