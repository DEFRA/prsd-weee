namespace EA.Weee.RequestHandlers.PCS.MemberRegistration.XmlValidation.BusinessValidation
{
    using Domain;
    using FluentValidation;

    public class ProducerTypeValidator : AbstractValidator<producerType>
    {
        public ProducerTypeValidator()
        {
            RuleFor(pt => pt.registrationNo)
                .NotEmpty()
                .When(pt => pt.status == statusType.A)
                .WithState(pt => ErrorLevel.Error)
                .WithMessage(
                    "The producer registration number for '{0}' has been left out of the xml file but the xml file is amending existing producer details. Check this producer's details. To amend this producer add the producer registration number or to add as a brand new producer use status \"I\" not \"A\".", 
                    pt => pt.tradingName);
        }
    }
}
