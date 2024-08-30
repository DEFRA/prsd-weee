namespace EA.Weee.Core.Organisations
{
    using System;

    public class OrganisationTransactionData
    {
        public Guid? Id {get; set; }

        public string SearchTerm { get; set; }

        public TonnageType? TonnageType { get; set; }

        public ExternalOrganisationType? OrganisationType { get; set; }

        public YesNoType? PreviousRegistration { get; set; }

        public YesNoType? AuthorisedRepresentative { get; set; }

        public RegisteredCompanyDetailsViewModel RegisteredCompanyDetailsViewModel { get; set; }

        public RepresentingCompanyDetailsViewModel RepresentingCompanyDetailsViewModel { get; set; }

        public PartnershipDetailsViewModel PartnershipDetailsViewModel { get; set; }

        public SoleTraderDetailsViewModel SoleTraderDetailsViewModel { get; set; }
    }
}
