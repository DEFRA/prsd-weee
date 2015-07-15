namespace EA.Weee.RequestHandlers.PCS.MemberRegistration.XmlValidation.BusinessValidation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Domain;
    using FluentValidation;
    using FluentValidation.Validators;

    public class SchemeTypeValidator : AbstractValidator<schemeType>
    {
        public SchemeTypeValidator()
        {
            RuleFor(st => st.producerList)
                .SetCollectionValidator(new ProducerTypeValidator());

            var duplicateRegistrationNumbers = new List<string>();
            RuleForEach(st => st.producerList)
                .Must((st, producer) =>
                {
                    var isDuplicate = st.producerList
                        .Any(p => p != producer && p.registrationNo == producer.registrationNo);

                    if (isDuplicate && !duplicateRegistrationNumbers.Contains(producer.registrationNo))
                    {
                        duplicateRegistrationNumbers.Add(producer.registrationNo);
                        return false;
                    }

                    return true;
                })
                .WithState(st => ErrorLevel.Error)
                .WithMessage("The Registration Number '{0}' appears more than once in the uploaded XML file",
                    (st, producer) => producer.registrationNo);
        }
    }
}
