namespace EA.Weee.Core.Organisations
{
    using EA.Weee.Core.Organisations.Base;
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

        public RepresentingCompanyDetailsViewModel RepresentingCompanyDetailsViewModel { get; set; }

        public OrganisationViewModel OrganisationViewModel { get; set; }

        public ContactDetailsViewModel ContactDetailsViewModel { get; set; }

        public List<PartnerModel> PartnerModels { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (OrganisationViewModel == null)
            {
                return new List<ValidationResult>() { new ValidationResult("Company details are required") };
            }

            return new List<ValidationResult>();
        }
    }
}
