namespace EA.Weee.Requests.Tests.Unit.MemberRegistration.XmlValidation.BusinessValidation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Domain;
    using Domain.PCS;
    using Domain.Producer;
    using FluentValidation;
    using FluentValidation.Internal;
    using RequestHandlers;
    using RequestHandlers.PCS.MemberRegistration.XmlValidation.BusinessValidation;
    using Xunit;
    using ValidationContext = XmlValidation.ValidationContext;

    public class ProducerTypeValidatorTests
    {
        [Theory]
        [InlineData(null, "TestCompany")]
        [InlineData("", "TestCompany")]
        public void Amendment_RegistrationNumberIsNullOrEmpty_FailsValidation_AndIncludesTradingNameInMessage_AndErrorLevelIsError(string registrationNumber, string tradingName)
        {
            var validationResult = ProducerTypeValidator().Validate(new producerType
            {
                tradingName = tradingName, 
                status = statusType.A, 
                registrationNo = registrationNumber
            }, new RulesetValidatorSelector(BusinessValidator.RegistrationNoRuleSet));

            Assert.False(validationResult.IsValid);
            Assert.Contains(tradingName, validationResult.Errors.Single().ErrorMessage);
            Assert.Equal(ErrorLevel.Error, validationResult.Errors.Single().CustomState);
        }

        [Fact]
        public void Amendment_RegistrationNumberIsNotNullOrEmpty_PassesValidation()
        {
            const string validRegistrationNumber = "ABC12345";
            const string validTradingName = "MyCompany";

            var validationResult = ProducerTypeValidator().Validate(new producerType
            {
                tradingName = validTradingName, 
                status = statusType.A, 
                registrationNo = validRegistrationNumber
            }, new RulesetValidatorSelector(BusinessValidator.RegistrationNoRuleSet));

            Assert.True(validationResult.IsValid);
        }

        [Fact]
        public void Insert_RegistrationNumberIsNotNullOrEmpty_FailsValidation_AndIncludesTradingNameInMessage_AndErrorLevelIsError()
        {
            const string validRegistrationNumber = "ABC12345";
            const string validTradingName = "MyCompany";

            var validationResult = ProducerTypeValidator().Validate(new producerType
            {
                tradingName = validTradingName, 
                status = statusType.I, 
                registrationNo = validRegistrationNumber
            }, new RulesetValidatorSelector(BusinessValidator.RegistrationNoRuleSet));

            Assert.False(validationResult.IsValid);
            Assert.Contains(validTradingName, validationResult.Errors.Single().ErrorMessage);
            Assert.Equal(ErrorLevel.Error, validationResult.Errors.Single().CustomState);
        }

        [Theory]
        [InlineData(null, "TestCompany")]
        [InlineData("", "TestCompany")]
        public void Insert_RegistrationNumberIsNullOrEmpty_PassesValidation(string registrationNumber, string tradingName)
        {
            var validationResult = ProducerTypeValidator().Validate(new producerType
            {
                tradingName = tradingName, 
                status = statusType.I, 
                registrationNo = registrationNumber
            }, new RulesetValidatorSelector(BusinessValidator.RegistrationNoRuleSet));

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
        public void AuthorisedRepresentativeOfficeCountryIsNotInUnitedKingdom_FailsValidation_AndIncludesTradingNameInMessage_AndErrorLevelIsError()
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
            Assert.Contains(ValidTradingName, validationResult.Errors.Single().ErrorMessage);
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
        public void ProducerHasPrnNumberThatDoesNotExistAndProducerIsPartnership_ReturnsPartnerNameInErrorMessage()
        {
            const string prn = "ABC12345";
            const string tradingName = "My Trading Name";
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
                tradingName = tradingName
            };

            var result = ProducerTypeValidator(new List<Producer> { Producer("ABC12346") }, new List<MigratedProducer>())
                .Validate(producer, new RulesetValidatorSelector(BusinessValidator.DataValidationRuleSet));

            Assert.Contains(partnershipName, result.Errors.Single().ErrorMessage);
        }

        [Fact]
        public void ProducerHasPrnNumberThatDoesNotExistAndProducerIsCompany_ReturnsCompanyNameInErrorMessage()
        {
            const string prn = "ABC12345";
            const string tradingName = "My Trading Name";
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
                tradingName = tradingName
            };

            var result = ProducerTypeValidator(new List<Producer> { Producer("ABC12346") }, new List<MigratedProducer>())
                .Validate(producer, new RulesetValidatorSelector(BusinessValidator.DataValidationRuleSet));

            Assert.Contains(companyName, result.Errors.Single().ErrorMessage);
        }

        private ProducerTypeValidator ProducerTypeValidator()
        {
            return new ProducerTypeValidator(ValidationContext.Create(new List<Producer>(), new List<MigratedProducer>()));
        }

        private ProducerTypeValidator ProducerTypeValidator(IEnumerable<Producer> producers,
            IEnumerable<MigratedProducer> migratedProducers)
        {
            return new ProducerTypeValidator(ValidationContext.Create(producers, migratedProducers));
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
                new List<SICCode>());
        }
    }
}
