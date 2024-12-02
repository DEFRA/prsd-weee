namespace EA.Weee.Core.Validation
{
    using EA.Weee.Core.Constants;
    using EA.Weee.Core.DataStandards;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Organisations.Base;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class CompaniesRegistrationNumberStringLengthAttribute : ValidationAttribute
    {
        private const int MinLength = 7;
        private const int UkMaxLength = 8;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!(validationContext.ObjectInstance is OrganisationViewModel model))
            {
                throw new ArgumentException("Expected OrganisationViewModel", nameof(validationContext));
            }

            var registrationNumber = value as string;

            // Skip validation if the value is null, empty or whitespace
            if (string.IsNullOrWhiteSpace(registrationNumber) || model.Action == "Edit")
            {
                return ValidationResult.Success;
            }

            var maxLength = DetermineMaxLength(model);
            var length = registrationNumber.Length;

            if (length < MinLength || length > maxLength)
            {
                return new ValidationResult(
                    $"The company registration number should be {MinLength} to {maxLength} characters long");
            }

            return ValidationResult.Success;
        }

        private static int DetermineMaxLength(OrganisationViewModel model)
        {
            var isUkRegisteredCompany = model.OrganisationType == ExternalOrganisationType.RegisteredCompany &&
                                        UkCountry.ValidIds.Contains(model.Address.CountryId);

            return isUkRegisteredCompany
                ? UkMaxLength
                : EnvironmentAgencyMaxFieldLengths.ExternallyCapturedCompanyNumber;
        }
    }
}