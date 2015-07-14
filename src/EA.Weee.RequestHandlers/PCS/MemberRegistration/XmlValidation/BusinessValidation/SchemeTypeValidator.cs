namespace EA.Weee.RequestHandlers.PCS.MemberRegistration.XmlValidation.BusinessValidation
{
    using FluentValidation;

    public class SchemeTypeValidator : AbstractValidator<schemeType>
    {
        public SchemeTypeValidator()
        {
            RuleFor(st => st.producerList)
                .SetCollectionValidator(new ProducerTypeValidator());
        }
    }
}
