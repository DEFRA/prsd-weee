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

                //Producer already registered with another scheme for obligation type
                RuleForEach(st => st.producerList)
                    .Must((st, producer) =>
                    {
                        ProducerAlreadyRegisteredError rule = new ProducerAlreadyRegisteredError(st, producer, organisationId);
                        ruleResult = ruleSelector.EvaluateRule(rule);
                        return ruleResult.IsValid;
                    })
                    .WithState(st => ruleResult.ErrorLevel.ToDomainEnumeration<ErrorLevel>())
                    .WithMessage("{0}", (st, producer) => ruleResult.Message);

                //Producer Name change warning
                RuleForEach(st => st.producerList)
                    .Must((st, producer) =>
                    {
                        ruleResult = ruleSelector.EvaluateRule(new ProducerNameWarning(st, producer, organisationId));
                        return ruleResult.IsValid;
                    })
                    .WithState(st => ruleResult.ErrorLevel.ToDomainEnumeration<ErrorLevel>())
                    .WithMessage("{0}", (st, producer) => ruleResult.Message);
            });
        }
    }
}
