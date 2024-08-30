namespace EA.Weee.Core.Organisations
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Core.DataStandards;
    using EA.Weee.Core.Validation;

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

        public Shared.AddressPostcodeRequiredData AddressData { get; set; } = new Shared.AddressPostcodeRequiredData();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return ExternalAddressValidator.Validate(AddressData.CountryId, AddressData.Postcode, "Address.CountryId", "Address.Postcode");
        }
    }
}