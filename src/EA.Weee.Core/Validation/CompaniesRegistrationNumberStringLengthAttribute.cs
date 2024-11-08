namespace EA.Weee.Core.Validation
{
    using EA.Weee.Core.Constants;
    using EA.Weee.Core.DataStandards;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Organisations.Base;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class CompaniesRegistrationNumberStringLengthAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (OrganisationViewModel)validationContext.ObjectInstance;

            var min = 7;
            var max = EnvironmentAgencyMaxFieldLengths.CompanyRegistrationNumber;

            if (model.OrganisationType == ExternalOrganisationType.RegisteredCompany && UkCountry.ValidIds.Contains(model.Address.CountryId))
            {
                max = 8;
            }

            var val = value as string;

            var count = val.Count();

            if (count < min || count > max)
            {
                return new ValidationResult($"The company registration number should be {min} to {max} characters long");
            }

            return ValidationResult.Success;
        }
    }
}
