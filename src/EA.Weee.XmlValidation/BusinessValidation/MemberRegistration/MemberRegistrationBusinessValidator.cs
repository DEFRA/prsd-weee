namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Rules.Producer;
    using Rules.Scheme;
    using Xml.MemberRegistration;

    public class MemberRegistrationBusinessValidator : IMemberRegistrationBusinessValidator
    {
        private readonly IProducerNameChange producerNameWarning;
        private readonly IAnnualTurnoverMismatch annualTurnoverMismatch;
        private readonly IProducerAlreadyRegistered producerAlreadyRegistered;
        private readonly IProducerNameAlreadyRegistered producerNameAlreadyRegistered;
        private readonly IDuplicateProducerRegistrationNumbers duplicateProducerRegistrationNumbers;
        private readonly IDuplicateProducerNames duplicateProducerNames;
        private readonly ICorrectSchemeApprovalNumber correctSchemeApprovalNumber;
        private readonly IAmendmentHasNoProducerRegistrationNumber amendmentHasNoProducerRegistrationNumber;
        private readonly IInsertHasProducerRegistrationNumber insertHasProducerRegistrationNumber;
        private readonly IUkBasedAuthorisedRepresentative ukBasedAuthorisedRepresentative;
        private readonly IProducerRegistrationNumberValidity producerRegistrationNumberValidity;
        private readonly IEnsureAnOverseasProducerIsNotBasedInTheUK ensureAnOverseasProducerIsNotBasedInTheUK;
        private readonly IProducerChargeBandChange producerChargeBandChangeWarning;
        private readonly ICompanyAlreadyRegistered companyAlreadyRegistered;
        private readonly ICompanyRegistrationNumberChange companyRegistrationNumberChange;

        public MemberRegistrationBusinessValidator(IProducerNameChange producerNameWarning, 
            IAnnualTurnoverMismatch annualTurnoverMismatch, 
            IProducerAlreadyRegistered producerAlreadyRegistered, 
            IProducerNameAlreadyRegistered producerNameAlreadyRegistered,
            IDuplicateProducerRegistrationNumbers duplicateProducerRegistrationNumbers,
            IDuplicateProducerNames duplicateProducerNames,
            ICorrectSchemeApprovalNumber correctSchemeApprovalNumber,
            IAmendmentHasNoProducerRegistrationNumber amendmentHasNoProducerRegistrationNumber,
            IInsertHasProducerRegistrationNumber insertHasProducerRegistrationNumber,
            IUkBasedAuthorisedRepresentative ukBasedAuthorisedRepresentative,
            IProducerRegistrationNumberValidity producerRegistrationNumberValidity,
            IEnsureAnOverseasProducerIsNotBasedInTheUK ensureAnOverseasProducerIsNotBasedInTheUK,
            IProducerChargeBandChange producerChargeBandChangeWarning,
            ICompanyAlreadyRegistered companyAlreadyRegistered,
            ICompanyRegistrationNumberChange companyRegistrationNumberChange)
        {
            this.producerNameWarning = producerNameWarning;
            this.annualTurnoverMismatch = annualTurnoverMismatch;
            this.producerAlreadyRegistered = producerAlreadyRegistered;
            this.producerNameAlreadyRegistered = producerNameAlreadyRegistered;
            this.duplicateProducerRegistrationNumbers = duplicateProducerRegistrationNumbers;
            this.duplicateProducerNames = duplicateProducerNames;
            this.correctSchemeApprovalNumber = correctSchemeApprovalNumber;
            this.amendmentHasNoProducerRegistrationNumber = amendmentHasNoProducerRegistrationNumber;
            this.insertHasProducerRegistrationNumber = insertHasProducerRegistrationNumber;
            this.ukBasedAuthorisedRepresentative = ukBasedAuthorisedRepresentative;
            this.producerRegistrationNumberValidity = producerRegistrationNumberValidity;
            this.ensureAnOverseasProducerIsNotBasedInTheUK = ensureAnOverseasProducerIsNotBasedInTheUK;
            this.producerChargeBandChangeWarning = producerChargeBandChangeWarning;
            this.companyAlreadyRegistered = companyAlreadyRegistered;
            this.companyRegistrationNumberChange = companyRegistrationNumberChange;
        }

        public IEnumerable<RuleResult> Validate(schemeType scheme, Guid organisationId)
        {
            var result = new List<RuleResult>();

            // Xml validation only
            result.AddRange(duplicateProducerRegistrationNumbers.Evaluate(scheme));
            result.AddRange(duplicateProducerNames.Evaluate(scheme));

            // Now comparing against existing data...
            result.Add(correctSchemeApprovalNumber.Evaluate(scheme, organisationId));

            // Validate producers
            foreach (var producer in scheme.producerList)
            {
                // Xml validation only
                result.Add(amendmentHasNoProducerRegistrationNumber.Evaluate(producer));
                result.Add(insertHasProducerRegistrationNumber.Evaluate(producer));
                result.Add(ukBasedAuthorisedRepresentative.Evaluate(producer));
                result.Add(producerNameWarning.Evaluate(scheme, producer, organisationId));
                result.Add(annualTurnoverMismatch.Evaluate(producer));
                result.Add(ensureAnOverseasProducerIsNotBasedInTheUK.Evaluate(producer));

                // Now comparing against existing data...
                result.Add(producerRegistrationNumberValidity.Evaluate(producer));
                result.Add(producerAlreadyRegistered.Evaluate(scheme, producer, organisationId));
                result.Add(producerNameAlreadyRegistered.Evaluate());
                result.Add(producerChargeBandChangeWarning.Evaluate(scheme, producer, organisationId));
                result.Add(companyAlreadyRegistered.Evaluate(producer));
                result.Add(companyRegistrationNumberChange.Evaluate(producer));
            }

            return result.Where(r => r != null && !r.IsValid);
        }
    }
}
