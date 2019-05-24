namespace EA.Weee.Migration.Organisation.Validation
{
    using FluentValidation;
    using Model;

    public class OrganisationDataValidator : AbstractValidator<Organisation>
    {
        public OrganisationDataValidator()
        {
            RuleFor(a => a.Name).NotEmpty().MaximumLength(256);
            RuleFor(a => a.TradingName).MaximumLength(256);
            RuleFor(a => a.RegistrationNumber).MaximumLength(15).MinimumLength(7);
        }
    }
}