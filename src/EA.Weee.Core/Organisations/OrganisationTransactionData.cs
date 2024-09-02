namespace EA.Weee.Core.Organisations
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class OrganisationTransactionData : IValidatableObject
    {
        public Guid? Id { get; set; }

        public string SearchTerm { get; set; }

        [Required] public TonnageType? TonnageType { get; set; }

        [Required] public ExternalOrganisationType? OrganisationType { get; set; }

        [Required] public YesNoType? PreviousRegistration { get; set; }

        [Required] public YesNoType? AuthorisedRepresentative { get; set; }

        public RegisteredCompanyDetailsViewModel RegisteredCompanyDetailsViewModel { get; set; }

        public PartnershipDetailsViewModel PartnershipDetailsViewModel { get; set; }

        public SoleTraderDetailsViewModel SoleTraderDetailsViewModel { get; set; }

        public RepresentingCompanyDetailsViewModel RepresentingCompanyDetailsViewModel { get; set; }

        public ContactDetailsViewModel ContactDetailsViewModel { get; set; }

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

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            switch (OrganisationType)
            {
                case ExternalOrganisationType.Partnership:
                    if (PartnershipDetailsViewModel == null)
                    {
                        return new List<ValidationResult>() { new ValidationResult("Partnership details are required") };
                    }
                    break;
                case ExternalOrganisationType.RegisteredCompany:
                    if (RegisteredCompanyDetailsViewModel == null)
                    {
                        return new List<ValidationResult>() { new ValidationResult("Registered company details are required") };
                    }
                    break;

                case ExternalOrganisationType.SoleTrader:
                    if (SoleTraderDetailsViewModel == null)
                    {
                        return new List<ValidationResult>() { new ValidationResult("Sole trader details are required") };
                    }
                    break;

                default:
                    throw new InvalidOperationException("Invalid organisation type.");
            }

            return new List<ValidationResult>();
        }
        public ContactDetailsViewModel ContactDetails { get; set; }
    }
}
