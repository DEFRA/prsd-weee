namespace EA.Weee.RequestHandlers.PCS.MemberRegistration.XmlValidation.BusinessValidation
{
    using System.Collections.Generic;
    using System.Linq;
    using DataAccess;
    using Domain;
    using FluentValidation;
    using Prsd.Core.Domain;

    public class SchemeTypeValidator : AbstractValidator<schemeType>
    {
        public const string NonDataValidation = "NonDataValidation";
        public const string DataValidation = "DataValidation";

        public SchemeTypeValidator(WeeeContext context)
        {
            RuleSet(NonDataValidation, () =>
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
            });

            RuleSet(DataValidation, () =>
            {
                var producers = context.Producers
                    .Where(p => p.MemberUpload != null)
                    .ToList();

                RuleForEach(st => st.producerList)
                    .Must((st, producer) =>
                    {
                        var existingProducer = producers
                            .FirstOrDefault(p => p.MemberUpload.ComplianceYear == int.Parse(st.complianceYear)
                                                 && p.RegistrationNumber == producer.registrationNo
                                                 &&
                                                 (Enumeration.FromValue<ObligationType>(p.ObligationType) ==
                                                  MapObligationType(producer.obligationType)
                                                  ||
                                                  Enumeration.FromValue<ObligationType>(p.ObligationType) ==
                                                  ObligationType.Both
                                                  || producer.obligationType == obligationTypeType.Both));

                        if (existingProducer != null)
                        {
                            // Map the existing obligation type to the producer so we can use it in the error message
                            producer.obligationType =
                                MapObligationType(Enumeration.FromValue<ObligationType>(existingProducer.ObligationType));
                            return false;
                        }

                        return true;
                    })
                    .WithState(st => ErrorLevel.Error)
                    .WithMessage(
                        "{0} {1} is already registered with another scheme with obligation type: {2}.",
                        (st, producer) =>
                        {
                            var producerName = producer.producerBrandNames != null
                                ? producer.producerBrandNames.FirstOrDefault()
                                : null;
                            producerName = producerName ?? producer.tradingName;
                            return producerName;
                        },
                        (st, producer) => producer.registrationNo,
                        (st, producer) => producer.obligationType.ToString());
            });
        }

        private ObligationType MapObligationType(obligationTypeType obligationType)
        {
            switch (obligationType)
            {
                case obligationTypeType.B2B:
                    return ObligationType.B2B;
                case obligationTypeType.B2C:
                    return ObligationType.B2C;

                default:
                    return ObligationType.Both;
            }
        }

        private obligationTypeType MapObligationType(ObligationType obligationType)
        {
            if (obligationType == ObligationType.B2B)
            {
                return obligationTypeType.B2B;
            }
            else if (obligationType == ObligationType.B2C)
            {
                return obligationTypeType.B2C;
            }

            return obligationTypeType.Both;       
        }
    }
}
