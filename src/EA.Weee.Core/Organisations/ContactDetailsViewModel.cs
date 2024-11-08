namespace EA.Weee.Core.Organisations
{
    using CsvHelper;
    using EA.Weee.Core.Constants;
    using EA.Weee.Core.DataStandards;
    using EA.Weee.Core.Validation;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class ContactDetailsViewModel : IValidatableObject
    {
        [Required]
        [StringLength(CommonMaxFieldLengths.FirstName)]
        [Display(Name = "First name")]
        public virtual string FirstName { get; set; }

        [Required]
        [StringLength(CommonMaxFieldLengths.LastName)]
        [Display(Name = "Last name")]
        public virtual string LastName { get; set; }

        [StringLength(CommonMaxFieldLengths.Position)]
        [Display(Name = "Position")]
        public virtual string Position { get; set; }

        public virtual bool HasAuthorisedRepresentitive { get; set; }

        public Shared.AddressPostcodeRequiredData AddressData { get; set; } = new Shared.AddressPostcodeRequiredData() { CountryId = Guid.Empty };

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            results.AddRange(ExternalAddressValidator.Validate(AddressData.CountryId, AddressData.Postcode, "AddressData.CountryId", "AddressData.Postcode"));

            var isUkCountry = UkCountry.ValidIds.Contains(AddressData.CountryId);
            if (isUkCountry == false && HasAuthorisedRepresentitive)
            {
                var validationsResult = new ValidationResult("Selected country must be a UK country.", new[] { "AddressData.CountryId" });

                results.Add(validationsResult);
            }

            return results;
        }
    }
}