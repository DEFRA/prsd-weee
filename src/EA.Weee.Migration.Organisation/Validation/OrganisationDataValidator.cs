namespace EA.Weee.Migration.Organisation.Validation
{
    using FluentValidation;

    public class OrganisationDataValidator : AbstractValidator<OrganisationData>
    {
        public OrganisationDataValidator()
        {
            RuleSet("Organisation", () =>
            {
                RuleFor(a => a.Name).NotEmpty().MaximumLength(256);
                RuleFor(a => a.TradingName).MaximumLength(256);
                RuleFor(a => a.RegistrationNumber).MaximumLength(15);
            });
        }
    }
}