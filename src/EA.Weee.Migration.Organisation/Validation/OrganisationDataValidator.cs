namespace EA.Weee.Migration.Organisation.Validation
{
    using FluentValidation;
    using Model;

    public class OrganisationDataValidator : AbstractValidator<Organisation>
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