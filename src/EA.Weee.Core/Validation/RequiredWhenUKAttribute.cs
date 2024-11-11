namespace EA.Weee.Core.Validation
{
    using EA.Weee.Core.Constants;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Organisations.Base;
    using System.ComponentModel.DataAnnotations;

    public class RequiredWhenUKAttribute : ValidationAttribute
    {
        private readonly string fieldName;

        public RequiredWhenUKAttribute(string fieldName)
        {
            this.fieldName = fieldName;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (OrganisationViewModel)validationContext.ObjectInstance;

            if (model.OrganisationType == ExternalOrganisationType.RegisteredCompany && UkCountry.ValidIds.Contains(model.Address.CountryId))
            {
                var val = value as string;
                return string.IsNullOrWhiteSpace(val)
                    ? new ValidationResult($"{fieldName} is required.")
                    : ValidationResult.Success;
            }

            return ValidationResult.Success;
        }
    }
}
