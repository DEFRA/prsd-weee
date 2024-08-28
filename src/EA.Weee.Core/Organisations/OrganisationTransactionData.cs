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

        public PartnershipDetailsViewModel PartnershipDetailsViewModel { get; set; }

        public SoleTraderDetailsViewModel SoleTraderDetailsViewModel { get; set; }

        public ExternalAddressData GetAddressData()
        {
            switch (OrganisationType)
            {
                case ExternalOrganisationType.Partnership:
                    return PartnershipDetailsViewModel?.Address;

                case ExternalOrganisationType.RegisteredCompany:
                    return RegisteredCompanyDetailsViewModel?.Address;

                case ExternalOrganisationType.SoleTrader:
                    return SoleTraderDetailsViewModel?.Address;

                default:
                    throw new InvalidOperationException("Invalid organisation type.");
            }
        }

        public string GetBrandNames()
        {
            switch (OrganisationType)
            {
                case ExternalOrganisationType.Partnership:
                    return PartnershipDetailsViewModel?.EEEBrandNames;

                case ExternalOrganisationType.RegisteredCompany:
                    return RegisteredCompanyDetailsViewModel?.EEEBrandNames;

                case ExternalOrganisationType.SoleTrader:
                    return SoleTraderDetailsViewModel?.EEEBrandNames;

                default:
                    throw new InvalidOperationException("Invalid organisation type.");
            }
        }
    }
}
