namespace EA.Weee.RequestHandlers.PCS.MemberRegistration.XmlValidation.BusinessValidation
{
    using System.Collections.Generic;
    using System.Linq;
    using Domain;
    using FluentValidation;

    public class SchemeTypeValidator : AbstractValidator<schemeType>
    {
        public SchemeTypeValidator(IValidationContext context)
        {
            RuleFor(st => st.producerList)
                .SetCollectionValidator(new ProducerTypeValidator(context));

            var duplicateRegistrationNumbers = new List<string>();
            RuleForEach(st => st.producerList)
                .Must((st, producer) =>
                {
                    if (!string.IsNullOrEmpty(producer.registrationNo)) // Duplicate empty registration numbers should not be validated
                    {
                        var isDuplicate = st.producerList
                            .Any(p => p != producer && p.registrationNo == producer.registrationNo);

                        if (isDuplicate && !duplicateRegistrationNumbers.Contains(producer.registrationNo))
                        {
                            duplicateRegistrationNumbers.Add(producer.registrationNo);
                            return false;
                        }
                    }

                    return true;
                })
                .WithState(st => ErrorLevel.Error)
                .WithMessage("The Registration Number '{0}' appears more than once in the uploaded XML file",
                    (st, producer) => producer.registrationNo);
        }
    }
}
