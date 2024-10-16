﻿namespace EA.Weee.Core.Organisations
{
    using EA.Weee.Core.Constants;
    using EA.Weee.Core.DataStandards;
    using EA.Weee.Core.Validation;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

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

        public Shared.AddressPostcodeRequiredData AddressData { get; set; } = new Shared.AddressPostcodeRequiredData() { CountryId = UkCountry.Ids.England };

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return ExternalAddressValidator.Validate(AddressData.CountryId, AddressData.Postcode, "AddressData.CountryId", "AddressData.Postcode");
        }
    }
}