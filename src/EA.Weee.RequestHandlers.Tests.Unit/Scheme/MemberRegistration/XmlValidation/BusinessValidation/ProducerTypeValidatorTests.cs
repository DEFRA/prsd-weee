namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme.MemberRegistration.XmlValidation.BusinessValidation
{
    using Core.XmlBusinessValidation;
    using Domain;
    using Domain.Producer;
    using Domain.Scheme;
    using FakeItEasy;
    using FluentValidation;
    using FluentValidation.Internal;
    using RequestHandlers;
    using RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation;
    using RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.Rules;
    using RequestHandlers.Scheme.MemberRegistration.XmlValidation.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Helpers;
    using Xunit;
    using ValidationContext = ValidationContext;

    public class ProducerTypeValidatorTests
    {
        private readonly IRuleSelector ruleSelector;

        public ProducerTypeValidatorTests()
        {
            ruleSelector = A.Fake<IRuleSelector>();

            // By default, all rules pass
            A.CallTo(() => ruleSelector.EvaluateRule(A<AnnualTurnoverMismatch>._))
                .Returns(RuleResult.Pass());
            A.CallTo(() => ruleSelector.EvaluateRule(A<ProducerNameRegisteredBefore>._))
                .Returns(RuleResult.Pass());
        }

        [Theory]
        [InlineData(null, "TestCompany")]
        [InlineData("", "TestCompany")]
        public void Amendment_RegistrationNumberIsNullOrEmpty_FailsValidation_AndIncludesProducerNameInMessage_AndErrorLevelIsError(string registrationNumber, string tradingName)
        {
            var producer = new producerType
            {
                tradingName = tradingName,
                status = statusType.A,
                registrationNo = registrationNumber
            };

            var validationResult = ProducerTypeValidator().Validate(producer, new RulesetValidatorSelector(BusinessValidator.RegistrationNoRuleSet));

            Assert.False(validationResult.IsValid);
            Assert.Contains(producer.GetProducerName(), validationResult.Errors.Single().ErrorMessage);
            Assert.Equal(ErrorLevel.Error, validationResult.Errors.Single().CustomState);
        }

        [Fact]
        public void Amendment_RegistrationNumberIsNotNullOrEmpty_PassesValidation()
        {
            const string validRegistrationNumber = "ABC12345";
            const string validTradingName = "MyCompany";

            var producer = new producerType
            {
                tradingName = validTradingName,
                status = statusType.A,
                registrationNo = validRegistrationNumber
            };

            var validationResult = ProducerTypeValidator().Validate(producer, new RulesetValidatorSelector(BusinessValidator.RegistrationNoRuleSet));

            Assert.True(validationResult.IsValid);
        }

        [Fact]
        public void Insert_RegistrationNumberIsNotNullOrEmpty_FailsValidation_AndIncludesProducerNameInMessage_AndErrorLevelIsError()
        {
            const string validRegistrationNumber = "ABC12345";
            const string validTradingName = "MyCompany";

            var producer = new producerType
            {
                tradingName = validTradingName, 
                status = statusType.I, 
                registrationNo = validRegistrationNumber
            };

            var validationResult = ProducerTypeValidator().Validate(producer, new RulesetValidatorSelector(BusinessValidator.RegistrationNoRuleSet));

            Assert.False(validationResult.IsValid);
            Assert.Contains(producer.GetProducerName(), validationResult.Errors.Single().ErrorMessage);
            Assert.Equal(ErrorLevel.Error, validationResult.Errors.Single().CustomState);
        }

        [Theory]
        [InlineData(null, "TestCompany")]
        [InlineData("", "TestCompany")]
        public void Insert_RegistrationNumberIsNullOrEmpty_PassesValidation(string registrationNumber, string tradingName)
        {
            var producer = new producerType
            {
                tradingName = tradingName, 
                status = statusType.I, 
                registrationNo = registrationNumber
            };

            var validationResult = ProducerTypeValidator().Validate(producer, new RulesetValidatorSelector(BusinessValidator.RegistrationNoRuleSet));

            Assert.True(validationResult.IsValid);
        }

        [Theory]
        [InlineData(countryType.UKENGLAND)]
        [InlineData(countryType.UKNORTHERNIRELAND)]
        [InlineData(countryType.UKSCOTLAND)]
        [InlineData(countryType.UKWALES)]
        public void AuthorisedRepresentativeOfficeCountryIsInUnitedKingdom_PassesValidation(countryType someUkCountry)
        {
            var producer = new producerType
            {
                authorisedRepresentative = new authorisedRepresentativeType
                {
                    overseasProducer = new overseasProducerType()
                },
                producerBusiness = MakeProducerBusinessTypeInCountry(someUkCountry)
            };

            var validationResult = ProducerTypeValidator()
                .Validate(producer, new RulesetValidatorSelector(BusinessValidator.AuthorisedRepresentativeMustBeInUkRuleset));

            Assert.True(validationResult.IsValid);
        }

        [Fact]
        public void AuthorisedRepresentativeOfficeCountryIsNotInUnitedKingdom_FailsValidation_AndIncludesProducerNameInMessage_AndErrorLevelIsError()
        {
            const string ValidTradingName = "MyCompany";

            const countryType SomeNonUkCountry = countryType.TURKEY;

            var producer = new producerType
            {
                tradingName = ValidTradingName,
                authorisedRepresentative = new authorisedRepresentativeType
                {
                    overseasProducer = new overseasProducerType()
                },
                producerBusiness = MakeProducerBusinessTypeInCountry(SomeNonUkCountry)
            };

            var validationResult = ProducerTypeValidator()
                .Validate(producer, new RulesetValidatorSelector(BusinessValidator.AuthorisedRepresentativeMustBeInUkRuleset));

            Assert.False(validationResult.IsValid);
            Assert.Contains(producer.GetProducerName(), validationResult.Errors.Single().ErrorMessage);
            Assert.Equal(ErrorLevel.Error, validationResult.Errors.Single().CustomState);
        }

        [Fact]
        public void NotAnAuthorisedRepresentativeButIsInNonUkCountry_PassesValidation()
        {
            const countryType SomeNonUkCountry = countryType.TURKEY;

            var producer = new producerType
            {
                authorisedRepresentative = new authorisedRepresentativeType { overseasProducer = null },
                producerBusiness = MakeProducerBusinessTypeInCountry(SomeNonUkCountry)
            };

            var validationResult = ProducerTypeValidator()
                .Validate(producer, new RulesetValidatorSelector(BusinessValidator.AuthorisedRepresentativeMustBeInUkRuleset));

            Assert.True(validationResult.IsValid);
        }

        [Fact]
        public void AuthorisedRepresentativeIsNotAValidBusinessType_ThrowsArgumentException()
        {
            var producer = new producerType
            {
                authorisedRepresentative =
                    new authorisedRepresentativeType { overseasProducer = new overseasProducerType() },
                producerBusiness = new producerBusinessType { Item = new object() }
            };

            Assert.Throws<ArgumentException>(() => 
                 ProducerTypeValidator().Validate(
                    producer,
                    new RulesetValidatorSelector(BusinessValidator.AuthorisedRepresentativeMustBeInUkRuleset)));
        }

        [Fact]
        public void ProducerHasPrnNumberThatDoesExist_ValidationSucceeds()
        {
            const string prn = "ABC12345";
            var producer = new producerType()
            {
                registrationNo = prn,
                status = statusType.A
            };

            var result = ProducerTypeValidator(new List<Producer> { Producer(prn) }, new List<MigratedProducer>())
                .Validate(producer, new RulesetValidatorSelector(BusinessValidator.DataValidationRuleSet));

            Assert.True(result.IsValid);
        }

        [Fact]
        public void ProducerHasPrnNumberThatDoesNotExist_IsInvalid_IncludesPrnInMessage_AndErrorLevelIsError()
        {
            const string prn = "ABC12345";
            var producer = new producerType()
            {
                registrationNo = prn,
                status = statusType.A
            };

            var result = ProducerTypeValidator(new List<Producer> { Producer("ABC12346") }, new List<MigratedProducer>())
                .Validate(producer, new RulesetValidatorSelector(BusinessValidator.DataValidationRuleSet));

            Assert.False(result.IsValid);

            Assert.Contains(prn, result.Errors.Single().ErrorMessage);
            Assert.Equal(ErrorLevel.Error, result.Errors.Single().CustomState);
        }

        [Fact]
        public void ProducerHasPrnNumberThatDoesNotExistAndProducerIsPartnership_ReturnsProducerNameInErrorMessage()
        {
            const string prn = "ABC12345";
            const string partnershipName = "Partnership Name";

            var producer = new producerType()
            {
                registrationNo = prn,
                status = statusType.A,
                producerBusiness = new producerBusinessType
                {
                    Item = new partnershipType
                    {
                        partnershipName = partnershipName
                    }
                },
            };

            var result = ProducerTypeValidator(new List<Producer> { Producer("ABC12346") }, new List<MigratedProducer>())
                .Validate(producer, new RulesetValidatorSelector(BusinessValidator.DataValidationRuleSet));

            Assert.Contains(producer.GetProducerName(), result.Errors.Single().ErrorMessage);
        }

        [Fact]
        public void ProducerHasPrnNumberThatDoesNotExistAndProducerIsCompany_ReturnsProducerNameInErrorMessage()
        {
            const string prn = "ABC12345";
            const string companyName = "Company Name";
            var producer = new producerType
            {
                registrationNo = prn,
                status = statusType.A,
                producerBusiness = new producerBusinessType
                {
                    Item = new companyType
                    {
                        companyName = companyName
                    }
                },
            };

            var result = ProducerTypeValidator(new List<Producer> { Producer("ABC12346") }, new List<MigratedProducer>())
                .Validate(producer, new RulesetValidatorSelector(BusinessValidator.DataValidationRuleSet));

            Assert.Contains(producer.GetProducerName(), result.Errors.Single().ErrorMessage);
        }

        [Fact]
        public void ProducerHasEmptyPrnNumberDuringAmendment_DataValidationSucceeds()
        {
            // amendments /should/ have PRNs, but this is handled in other rules with more relevant description
            // throwing this validation error here will just confuse the user

            var producer = new producerType
            {
                registrationNo = string.Empty,
                status = statusType.A
            };

            var result = ProducerTypeValidator(new List<Producer> { Producer("ABC12346") }, new List<MigratedProducer>())
                .Validate(producer, new RulesetValidatorSelector(BusinessValidator.DataValidationRuleSet));

            Assert.Empty(result.Errors);
        }

        [Fact]
        public void ProducerDoesNotHaveAnnualTurnoverMismatch_ReturnsValidResult()
        {
            A.CallTo(() => ruleSelector.EvaluateRule(A<AnnualTurnoverMismatch>._))
                .Returns(RuleResult.Pass());

            var result = ProducerTypeValidator()
                .Validate(new producerType(), new RulesetValidatorSelector(BusinessValidator.CustomRules));

            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(Core.Shared.ErrorLevel.Warning)]
        [InlineData(Core.Shared.ErrorLevel.Error)]
        public void ProducerDoesHaveAnnualTurnoverMismatch_ReturnsResult_WithMappedState_AndMappedErrorMessage(Core.Shared.ErrorLevel errorLevel)
        {
            var ruleResult = RuleResult.Fail("oops", errorLevel);

            A.CallTo(() => ruleSelector.EvaluateRule(A<AnnualTurnoverMismatch>._))
                .Returns(ruleResult);

            var result = ProducerTypeValidator()
                .Validate(new producerType(), new RulesetValidatorSelector(BusinessValidator.CustomRules));

            Assert.False(result.IsValid);
            Assert.Equal(ruleResult.Message, result.Errors.Single().ErrorMessage);
            Assert.Equal(errorLevel.ToDomainEnumeration<ErrorLevel>(), result.Errors.Single().CustomState);
        }

        [Fact]
        public void ProducerNameHasNotBeenRegisteredBefore_ReturnsValidResult()
        {
            A.CallTo(() => ruleSelector.EvaluateRule(A<ProducerNameRegisteredBefore>._))
                .Returns(RuleResult.Pass());

            var result = ProducerTypeValidator()
                .Validate(new producerType(), new RulesetValidatorSelector(BusinessValidator.CustomRules));

            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(Core.Shared.ErrorLevel.Warning)]
        [InlineData(Core.Shared.ErrorLevel.Error)]
        public void ProducerNameHasBeenRegisteredBefore_ReturnsResult_WithMappedState_AndMappedErrorMessage(Core.Shared.ErrorLevel errorLevel)
        {
            var ruleResult = RuleResult.Fail("oops", errorLevel);

            A.CallTo(() => ruleSelector.EvaluateRule(A<ProducerNameRegisteredBefore>._))
                .Returns(ruleResult);

            var result = ProducerTypeValidator()
                .Validate(new producerType(), new RulesetValidatorSelector(BusinessValidator.CustomRules));

            Assert.False(result.IsValid);
            Assert.Equal(ruleResult.Message, result.Errors.Single().ErrorMessage);
            Assert.Equal(errorLevel.ToDomainEnumeration<ErrorLevel>(), result.Errors.Single().CustomState);
        }

        private ProducerTypeValidator ProducerTypeValidator()
        {
            return new ProducerTypeValidator(ValidationContext.Create(new List<Producer>(), new List<MigratedProducer>()), ruleSelector);
        }

        private ProducerTypeValidator ProducerTypeValidator(IEnumerable<Producer> producers,
            IEnumerable<MigratedProducer> migratedProducers)
        {
            return new ProducerTypeValidator(ValidationContext.Create(producers, migratedProducers), ruleSelector);
        }

        private producerBusinessType MakeProducerBusinessTypeInCountry(countryType country)
        {
            return new producerBusinessType
            {
                Item =
                    new partnershipType
                    {
                        principalPlaceOfBusiness =
                            new contactDetailsContainerType
                            {
                                contactDetails =
                                    new contactDetailsType { address = new addressType { country = country } }
                            }
                    }
            };
        }

        private Producer Producer(string prn, params string[] brandNames)
        {
            return new Producer(Guid.NewGuid(), 
                new MemberUpload(Guid.NewGuid(), "<xml>SomeData</xml>"), 
                new ProducerBusiness(), 
                new AuthorisedRepresentative("authrep"),
                DateTime.Now,
                decimal.Zero,
                true,
                prn,
                null,
                "trading name",
                EEEPlacedOnMarketBandType.Lessthan5TEEEplacedonmarket,
                SellingTechniqueType.Both,
                ObligationType.B2B,
                AnnualTurnOverBandType.Greaterthanonemillionpounds,
                brandNames.Select(bn => new BrandName(bn)).ToList(),
                new List<SICCode>(),
                true,
                ChargeBandType.A,
                (decimal)445.0);
        }
    }
}
