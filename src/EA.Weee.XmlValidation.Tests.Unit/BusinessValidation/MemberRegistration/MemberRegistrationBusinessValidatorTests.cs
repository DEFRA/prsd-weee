namespace EA.Weee.XmlValidation.Tests.Unit.BusinessValidation.MemberRegistration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Shared;
    using FakeItEasy;
    using Xml.MemberRegistration;
    using XmlValidation.BusinessValidation;
    using XmlValidation.BusinessValidation.MemberRegistration;
    using XmlValidation.BusinessValidation.MemberRegistration.Rules.Producer;
    using XmlValidation.BusinessValidation.MemberRegistration.Rules.Scheme;
    using Xunit;

    public class MemberRegistrationBusinessValidatorTests
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

        public MemberRegistrationBusinessValidatorTests()
        {
            producerNameWarning = A.Fake<IProducerNameChange>();
            annualTurnoverMismatch = A.Fake<IAnnualTurnoverMismatch>();
            producerAlreadyRegistered = A.Fake<IProducerAlreadyRegistered>();
            producerNameAlreadyRegistered = A.Fake<IProducerNameAlreadyRegistered>();
            duplicateProducerRegistrationNumbers = A.Fake<IDuplicateProducerRegistrationNumbers>();
            duplicateProducerNames = A.Fake<IDuplicateProducerNames>();
            correctSchemeApprovalNumber = A.Fake<ICorrectSchemeApprovalNumber>();
            amendmentHasNoProducerRegistrationNumber = A.Fake<IAmendmentHasNoProducerRegistrationNumber>();
            insertHasProducerRegistrationNumber = A.Fake<IInsertHasProducerRegistrationNumber>();
            ukBasedAuthorisedRepresentative = A.Fake<IUkBasedAuthorisedRepresentative>();
            producerRegistrationNumberValidity = A.Fake<IProducerRegistrationNumberValidity>();
            ensureAnOverseasProducerIsNotBasedInTheUK = A.Fake<IEnsureAnOverseasProducerIsNotBasedInTheUK>();
            producerChargeBandChangeWarning = A.Fake<IProducerChargeBandChange>();
            companyAlreadyRegistered = A.Fake<ICompanyAlreadyRegistered>();
            companyRegistrationNumberChange = A.Fake<ICompanyRegistrationNumberChange>();
        }

        [Fact]
        public void DuplicateRegistrationNumbers_ShouldReturnRuleResult()
        {
            var scheme = SchemeWithXProducers(1);
            var schemeId = Guid.NewGuid();
            var errors = new List<RuleResult> { RuleResult.Fail("oops") };

            A.CallTo(() => duplicateProducerRegistrationNumbers.Evaluate(scheme)).Returns(errors);

            var result = XmlBusinessValidator().Validate(scheme, schemeId);
            Assert.Equal(errors, result);
        }

        [Fact]
        public void DuplicateProducerNames_ShouldReturnRuleResult()
        {
            var scheme = SchemeWithXProducers(1);
            var schemeId = Guid.NewGuid();
            var errors = new List<RuleResult> { RuleResult.Fail("oops") };

            A.CallTo(() => duplicateProducerNames.Evaluate(scheme)).Returns(errors);

            var result = XmlBusinessValidator().Validate(scheme, schemeId);
            Assert.Equal(errors, result);
        }

        [Fact]
        public void SchemeApprovalNumberIsInvalid_ShouldReturnRuleResult()
        {
            var scheme = SchemeWithXProducers(1);
            var schemeId = Guid.NewGuid();
            var error = RuleResult.Fail("oops");

            A.CallTo(() => correctSchemeApprovalNumber.Evaluate(scheme, schemeId)).Returns(error);

            var result = XmlBusinessValidator().Validate(scheme, schemeId);

            Assert.Single(result);
            Assert.Equal(error, result.Single());
        }

        [Fact]
        public void ProducerAmendmentHasNoRegistrationNumber_ShouldReturnRuleResult()
        {
            var scheme = SchemeWithXProducers(1);
            var schemeId = Guid.NewGuid();
            var error = RuleResult.Fail("oops");

            A.CallTo(() => amendmentHasNoProducerRegistrationNumber.Evaluate(scheme.producerList.Single())).Returns(error);

            var result = XmlBusinessValidator().Validate(scheme, schemeId);

            Assert.Single(result);
            Assert.Equal(error, result.Single());
        }

        [Fact]
        public void ProducerInsertHasRegistrationNumber_ShouldReturnRuleResult()
        {
            var scheme = SchemeWithXProducers(1);
            var schemeId = Guid.NewGuid();
            var error = RuleResult.Fail("oops");

            A.CallTo(() => insertHasProducerRegistrationNumber.Evaluate(scheme.producerList.Single())).Returns(error);

            var result = XmlBusinessValidator().Validate(scheme, schemeId);

            Assert.Single(result);
            Assert.Equal(error, result.Single());
        }

        [Fact]
        public void ProducerHasNoUkBasedAuthorisedRepresentative_ShouldReturnRuleResult()
        {
            var scheme = SchemeWithXProducers(1);
            var schemeId = Guid.NewGuid();
            var error = RuleResult.Fail("oops");

            A.CallTo(() => ukBasedAuthorisedRepresentative.Evaluate(scheme.producerList.Single())).Returns(error);

            var result = XmlBusinessValidator().Validate(scheme, schemeId);

            Assert.Single(result);
            Assert.Equal(error, result.Single());
        }

        [Fact]
        public void ProducerNameIsChanging_ShouldReturnRuleResult()
        {
            var scheme = SchemeWithXProducers(1);
            var schemeId = Guid.NewGuid();
            var error = RuleResult.Fail("oops", ErrorLevel.Warning);

            A.CallTo(() => producerNameWarning.Evaluate(scheme, scheme.producerList.Single(), schemeId)).Returns(error);

            var result = XmlBusinessValidator().Validate(scheme, schemeId);

            Assert.Single(result);
            Assert.Equal(error, result.Single());
        }

        [Fact]
        public void AnnualTurnoverHasMismatch_ShouldReturnRuleResult()
        {
            var scheme = SchemeWithXProducers(1);
            var schemeId = Guid.NewGuid();
            var error = RuleResult.Fail("oops");

            A.CallTo(() => annualTurnoverMismatch.Evaluate(scheme.producerList.Single())).Returns(error);

            var result = XmlBusinessValidator().Validate(scheme, schemeId);

            Assert.Single(result);
            Assert.Equal(error, result.Single());
        }

        [Fact]
        public void ProducerRegistrationNumberIsInvalid_ShouldReturnRuleResult()
        {
            var scheme = SchemeWithXProducers(1);
            var schemeId = Guid.NewGuid();
            var error = RuleResult.Fail("oops");

            A.CallTo(() => producerRegistrationNumberValidity.Evaluate(scheme.producerList.Single())).Returns(error);

            var result = XmlBusinessValidator().Validate(scheme, schemeId);

            Assert.Single(result);
            Assert.Equal(error, result.Single());
        }

        [Fact]
        public void ProducerAlreadyRegistered_ShouldReturnRuleResult()
        {
            var scheme = SchemeWithXProducers(1);
            var schemeId = Guid.NewGuid();
            var error = RuleResult.Fail("oops", ErrorLevel.Warning);

            A.CallTo(() => producerAlreadyRegistered.Evaluate(scheme, scheme.producerList.Single(), schemeId)).Returns(error);

            var result = XmlBusinessValidator().Validate(scheme, schemeId);

            Assert.Single(result);
            Assert.Equal(error, result.Single());
        }

        [Fact]
        public void ProducerNameAlreadyRegistered_ShouldReturnRuleResult()
        {
            var scheme = SchemeWithXProducers(1);
            var schemeId = Guid.NewGuid();
            var error = RuleResult.Fail("oops", ErrorLevel.Warning);

            A.CallTo(() => producerNameAlreadyRegistered.Evaluate()).Returns(error);

            var result = XmlBusinessValidator().Validate(scheme, schemeId);

            Assert.Single(result);
            Assert.Equal(error, result.Single());
        }

        [Fact]
        public void ProducerChargeBandChanged_ShouldReturnRuleResult()
        {
            var scheme = SchemeWithXProducers(1);
            var schemeId = Guid.NewGuid();
            var error = RuleResult.Fail("oops", ErrorLevel.Warning);

            A.CallTo(() => producerChargeBandChangeWarning.Evaluate(scheme, scheme.producerList.Single(), schemeId)).Returns(error);

            var result = XmlBusinessValidator().Validate(scheme, schemeId);

            Assert.Single(result);
            Assert.Equal(error, result.Single());
        }

        [Fact]
        public void CompanyAlreadyRegistered_ShouldReturnRuleResult()
        {
            var scheme = SchemeWithXProducers(1);
            var schemeId = Guid.NewGuid();
            var error = RuleResult.Fail("oops", ErrorLevel.Warning);

            A.CallTo(() => companyAlreadyRegistered.Evaluate(scheme.producerList.Single())).Returns(error);

            var result = XmlBusinessValidator().Validate(scheme, schemeId);

            Assert.Single(result);
            Assert.Equal(error, result.Single());
        }

        [Fact]
        public void CompanyRegistrationNumberChange_ShouldReturnRuleResult()
        {
            var scheme = SchemeWithXProducers(1);
            var schemeId = Guid.NewGuid();
            var error = RuleResult.Fail("oops", ErrorLevel.Warning);

            A.CallTo(() => companyRegistrationNumberChange.Evaluate(scheme.producerList.Single())).Returns(error);

            var result = XmlBusinessValidator().Validate(scheme, schemeId);

            Assert.Single(result);
            Assert.Equal(error, result.Single());
        }

        [Fact]
        public void WhereAllRulesPass_NoRuleResultsShouldBeReturned()
        {
            A.CallTo(() => producerNameWarning.Evaluate(A<schemeType>._, A<producerType>._, A<Guid>._)).Returns(RuleResult.Pass());
            A.CallTo(() => annualTurnoverMismatch.Evaluate(A<producerType>._)).Returns(RuleResult.Pass());
            A.CallTo(() => producerAlreadyRegistered.Evaluate(A<schemeType>._, A<producerType>._, A<Guid>._)).Returns(RuleResult.Pass());
            A.CallTo(() => producerNameAlreadyRegistered.Evaluate()).Returns(RuleResult.Pass());
            A.CallTo(() => duplicateProducerRegistrationNumbers.Evaluate(A<schemeType>._)).Returns(new List<RuleResult> { RuleResult.Pass() });
            A.CallTo(() => duplicateProducerNames.Evaluate(A<schemeType>._)).Returns(new List<RuleResult> { RuleResult.Pass() });
            A.CallTo(() => correctSchemeApprovalNumber.Evaluate(A<schemeType>._, A<Guid>._)).Returns(RuleResult.Pass());
            A.CallTo(() => amendmentHasNoProducerRegistrationNumber.Evaluate(A<producerType>._)).Returns(RuleResult.Pass());
            A.CallTo(() => insertHasProducerRegistrationNumber.Evaluate(A<producerType>._)).Returns(RuleResult.Pass());
            A.CallTo(() => ukBasedAuthorisedRepresentative.Evaluate(A<producerType>._)).Returns(RuleResult.Pass());
            A.CallTo(() => producerRegistrationNumberValidity.Evaluate(A<producerType>._)).Returns(RuleResult.Pass());
            A.CallTo(() => ensureAnOverseasProducerIsNotBasedInTheUK.Evaluate(A<producerType>._)).Returns(RuleResult.Pass());
            A.CallTo(() => producerChargeBandChangeWarning.Evaluate(A<schemeType>._, A<producerType>._, A<Guid>._)).Returns(RuleResult.Pass());
            A.CallTo(() => companyAlreadyRegistered.Evaluate(A<producerType>._)).Returns(RuleResult.Pass());
            A.CallTo(() => companyRegistrationNumberChange.Evaluate(A<producerType>._)).Returns(RuleResult.Pass());

            var scheme = new schemeType
            {
                producerList = new[]
                {
                    new producerType()
                }
            };

            var schemeId = Guid.NewGuid();

            var result = XmlBusinessValidator().Validate(scheme, schemeId);

            Assert.Empty(result);
        }

        private MemberRegistrationBusinessValidator XmlBusinessValidator()
        {
            return new MemberRegistrationBusinessValidator(
                producerNameWarning,
                annualTurnoverMismatch,
                producerAlreadyRegistered,
                producerNameAlreadyRegistered,
                duplicateProducerRegistrationNumbers,
                duplicateProducerNames,
                correctSchemeApprovalNumber,
                amendmentHasNoProducerRegistrationNumber,
                insertHasProducerRegistrationNumber,
                ukBasedAuthorisedRepresentative,
                producerRegistrationNumberValidity,
                ensureAnOverseasProducerIsNotBasedInTheUK,
                producerChargeBandChangeWarning,
                companyAlreadyRegistered,
                companyRegistrationNumberChange);
        }

        private schemeType SchemeWithXProducers(int numberOfProducers)
        {
            var producers = new List<producerType>();
            for (int i = 0; i < numberOfProducers; i++)
            {
                producers.Add(new producerType());
            }

            var scheme = new schemeType
            {
                producerList = producers.ToArray()
            };

            return scheme;
        }
    }
}
