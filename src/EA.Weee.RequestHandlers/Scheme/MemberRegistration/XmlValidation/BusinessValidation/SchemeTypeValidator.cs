namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Helpers;
    using Core.XmlBusinessValidation;
    using DataAccess;
    using Domain;
    using Extensions;
    using FluentValidation;
    using Rules;

    public class SchemeTypeValidator : AbstractValidator<schemeType>
    {
        public const string NonDataValidation = "NonDataValidation";
        public const string DataValidation = "DataValidation";

        public SchemeTypeValidator(WeeeContext context, Guid organisationId, IRuleSelector ruleSelector)
        {
            RuleResult ruleResult = null;
            
            //Non data validation
            RuleSet(NonDataValidation, () =>
            {
                RuleFor(st => st.producerList)
                    .SetCollectionValidator(new ProducerTypeValidator(context, ruleSelector));

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

            //data validation
            RuleSet(DataValidation, () =>
            {
                //approval number validation
                var scheme = context.Schemes.FirstOrDefault(s => s.OrganisationId == organisationId);
                if (scheme != null)
                {
                    RuleFor(st => st.approvalNo)
                        .NotEmpty()
                        .Matches(scheme.ApprovalNumber)
                        .WithState(st => ErrorLevel.Error)
                        .WithMessage("The PCS approval number in your XML file {0} doesn’t match with the PCS that you’re uploading for. Please make sure that you’re using the right PCS approval number.",
                        st => st.approvalNo);
                }
            });

            RuleSet(BusinessValidator.CustomRules, () =>
            {
                //Producer already registered with another scheme for obligation type
                var producerAlreadyRegisteredResult = RuleResult.Pass();
                RuleForEach(st => st.producerList)
                    .Must((st, producer) =>
                    {
                        producerAlreadyRegisteredResult = ruleSelector.EvaluateRule(new ProducerAlreadyRegisteredError(st, producer, organisationId));
                        return producerAlreadyRegisteredResult.IsValid;
                    })
                    .WithState(st => producerAlreadyRegisteredResult.ErrorLevel.ToDomainEnumeration<ErrorLevel>())
                    .WithMessage("{0}", (st, producer) => producerAlreadyRegisteredResult.Message);

                //Producer Name change warning
                var producerNameChangeWarningResult = RuleResult.Pass();
                RuleForEach(st => st.producerList)
                    .Must((st, producer) =>
                    {
                        producerNameChangeWarningResult = ruleSelector.EvaluateRule(new ProducerNameWarning(st, producer, organisationId));
                        return producerNameChangeWarningResult.IsValid;
                    })
                    .WithState(st => producerNameChangeWarningResult.ErrorLevel.ToDomainEnumeration<ErrorLevel>())
                    .WithMessage("{0}", (st, producer) => producerNameChangeWarningResult.Message);

                // Producer name has already been registered
                var producerNameAlreadyRegisteredBeforeResult = RuleResult.Pass();
                RuleForEach(st => st.producerList)
                    .Must((st, producer) =>
                    {
                        producerNameAlreadyRegisteredBeforeResult =
                            ruleSelector.EvaluateRule(new ProducerNameRegisteredBefore(producer, st.complianceYear));
                        return producerNameAlreadyRegisteredBeforeResult.IsValid;
                    })
                    .WithState(
                        st => producerNameAlreadyRegisteredBeforeResult.ErrorLevel.ToDomainEnumeration<ErrorLevel>())
                    .WithMessage("{0}", (st, producer) => producerNameAlreadyRegisteredBeforeResult.Message);
            });
        }
    }
}
