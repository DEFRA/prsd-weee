namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DataAccess;
    using Domain;
    using Domain.Producer;
    using Extensions;
    using FluentValidation;
  
    public class SchemeTypeValidator : AbstractValidator<schemeType>
    {
        public const string NonDataValidation = "NonDataValidation";
        public const string DataValidation = "DataValidation";
       
        public SchemeTypeValidator(WeeeContext context, Guid organisationId)
        {
            RuleSet(DataValidation, () =>
            {
                var scheme = context.Schemes.FirstOrDefault(s => s.OrganisationId == organisationId);
                if (scheme != null)
                {
                    RuleFor(st => st.approvalNo)
                        .NotEmpty()
                        .Matches(scheme.ApprovalNumber)
                        .WithState(st => ErrorLevel.Error)
                        .WithMessage("The approval number for your compliance scheme doesn’t match with the PCS that you selected. Please make sure that you’re entering the right compliance scheme approval number.");
                }
            });

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

                var duplicateProducerNames = new List<string>();
                RuleForEach(st => st.producerList)
                    .Must((st, producer) =>
                {
                    if (string.IsNullOrEmpty(producer.GetProducerName()))
                    {
                        throw new ArgumentException(
                            string.Format("{0}: producer must have a producer name, should have been caught in schema", producer.tradingName));
                    }

                    var isDuplicate = st.producerList
                        .Any(p => p != producer && p.GetProducerName() == producer.GetProducerName());

                    if (isDuplicate && !duplicateProducerNames.Contains(producer.GetProducerName()))
                    {
                        duplicateProducerNames.Add(producer.GetProducerName());
                        return false;
                    }

                    return true;
                })
                .WithState(st => ErrorLevel.Error)
                .WithMessage("The Producer Name '{0}' appears more than once in the uploaded XML file",
                    (st, producer) => producer.GetProducerName());
            });

            RuleSet(DataValidation, () =>
            {
                var producers = context.Producers
                    .Where(p => p.MemberUpload != null
                    && p.Scheme.OrganisationId != organisationId && p.IsCurrentForComplianceYear)
                    .ToList();

                RuleForEach(st => st.producerList)
                    .Must((st, producer) =>
                    {
                        if (!string.IsNullOrEmpty(producer.registrationNo))
                        {
                            var existingProducer = producers
                                .FirstOrDefault(p => p.MemberUpload.ComplianceYear == int.Parse(st.complianceYear)
                                                     && p.RegistrationNumber == producer.registrationNo
                                                     &&
                                                     ((Domain.ObligationType)p.ObligationType ==
                                                      producer.obligationType.ToDomainObligationType()
                                                      ||
                                                      (Domain.ObligationType)p.ObligationType ==
                                                      ObligationType.Both
                                                      || producer.obligationType == obligationTypeType.Both));

                            if (existingProducer != null)
                            {
                                // Map the existing obligation type to the producer so we can use it in the error message
                                producer.obligationType =
                                    DeserializedXmlExtensions.ToDeserializedXmlObligationType(
                                        (Domain.ObligationType)existingProducer.ObligationType);
                                return false;
                            }
                        }

                        return true;
                    })
                    .WithState(st => ErrorLevel.Error)
                    .WithMessage(
                        "{0} {1} is already registered with another scheme with obligation type: {2}.",
                        (st, producer) => producer.GetProducerName(),
                        (st, producer) => producer.registrationNo,
                        (st, producer) => producer.obligationType.ToString());

                //Producer Name change warning
                var currentProducers = context.Producers.Where(p => p.IsCurrentForComplianceYear).ToList();
                RuleForEach(st => st.producerList)
                    .Must((st, producer) =>
                    {
                        if (producer.status == statusType.A)
                        {
                            string producerName = producer.GetProducerName();
                            var matchingProducer = GetMatchingProducer(currentProducers, producer.registrationNo, st.complianceYear, organisationId);
                            if (matchingProducer == null)
                            {
                                // search in migrated producers list if not in Producers database
                                var migratedProducer = context.MigratedProducers.FirstOrDefault(p => p.ProducerRegistrationNumber == producer.registrationNo);
                                if (migratedProducer == null)
                                {
                                    throw new ArgumentNullException(string.Format("No matching producer found for PRN {0}", producer.registrationNo));
                                }

                                if (migratedProducer.ProducerName != producerName)
                                {
                                    return false;
                                }
                            }
                            
                            if (matchingProducer != null && matchingProducer.OrganisationName != producerName)
                            {
                                return false;
                            }
                        }
                        return true;
                    })
                    .WithState(st => ErrorLevel.Warning)
                    .WithMessage(
                     "The name of the producer with registration number {0} will be amended to {1}.",
                     (st, producer) => producer.registrationNo,
                     (st, procucer) => procucer.GetProducerName());
            });
        }

        private Producer GetMatchingProducer(List<Producer> currentProducers, string registrationNo, string schemeComplianceYear, Guid schemeOrgId)
        {
            var matchingProducer = currentProducers.FirstOrDefault(p =>
                                                                    p.RegistrationNumber == registrationNo
                                                                    && p.MemberUpload.ComplianceYear == int.Parse(schemeComplianceYear)
                                                                    && p.Scheme.OrganisationId == schemeOrgId);
            if (matchingProducer == null)
            {
                // reverse the order the current producers list by compliance year and then by updatedDate
                matchingProducer = currentProducers.Where(p => p.RegistrationNumber == registrationNo)
                    .OrderByDescending(p => p.MemberUpload.ComplianceYear)
                    .ThenBy(p => p.UpdatedDate)
                    .FirstOrDefault();
            }
            return matchingProducer;
        }
    }
}
