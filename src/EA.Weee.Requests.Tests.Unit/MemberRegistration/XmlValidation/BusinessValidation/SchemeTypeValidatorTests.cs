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

    public class SchemeTypeValidatorTests
    {
        [Fact]
        public void SetOfDuplicateRegistrationNumbers_ValidationFails_IncludesRegistraionNumberInMessage_AndErrorLevelIsError()
        {
            const string registrationNumber = "ABC12345";
            var xml = new schemeType
            {
                producerList = Producers(registrationNumber, registrationNumber)
            };

            var result = SchemeTypeValidator().Validate(xml, new RulesetValidatorSelector(RequestHandlers.PCS.MemberRegistration.XmlValidation.BusinessValidation.SchemeTypeValidator.NonDataValidation));

            Assert.False(result.IsValid);
            Assert.Contains(registrationNumber, result.Errors.Single().ErrorMessage);
            Assert.Equal(ErrorLevel.Error, result.Errors.Single().CustomState);
        }

        [Fact]
        public void SetOfEmptyRegistrationNumbers_ValidationSucceeds()
        {
            var xml = new schemeType
            {
                producerList = Producers(string.Empty, string.Empty)
            };

            var result = SchemeTypeValidator().Validate(xml);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void TwoSetsOfDuplicateRegistrationNumbers_ValidationFails_IncludesBothRegistrationNumbersInMessages()
        {
            const string firstRegistrationNumber = "ABC12345";
            const string secondRegistrationNumber = "XYZ54321";
            var xml = new schemeType
            {
                producerList = Producers(firstRegistrationNumber, firstRegistrationNumber, secondRegistrationNumber, secondRegistrationNumber)
            };

            var result = SchemeTypeValidator().Validate(xml, new RulesetValidatorSelector(RequestHandlers.PCS.MemberRegistration.XmlValidation.BusinessValidation.SchemeTypeValidator.NonDataValidation));

            Assert.False(result.IsValid);

            var aggregatedErrorMessages = result.Errors.Select(err => err.ErrorMessage).Aggregate((curr, next) => curr + ", " + next);

            Assert.Contains(firstRegistrationNumber, aggregatedErrorMessages);
            Assert.Contains(secondRegistrationNumber, aggregatedErrorMessages);
        }

        [Fact]
        public void TwoProducersWithDifferentRegistrationNumbers_ValidationSucceeds()
        {
            var xml = new schemeType
            {
                producerList = Producers("ABC12345", "XYZ54321").ToArray()
            };

            var result = SchemeTypeValidator().Validate(xml, new RulesetValidatorSelector(RequestHandlers.PCS.MemberRegistration.XmlValidation.BusinessValidation.SchemeTypeValidator.NonDataValidation));

            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(obligationTypeType.B2B, obligationTypeType.B2B)]
        [InlineData(obligationTypeType.B2C, obligationTypeType.B2C)]
        [InlineData(obligationTypeType.Both, obligationTypeType.B2C)]
        [InlineData(obligationTypeType.Both, obligationTypeType.B2B)]
        [InlineData(obligationTypeType.B2B, obligationTypeType.Both)]
        [InlineData(obligationTypeType.B2C, obligationTypeType.Both)]
        public void ProducerAlreadyRegisteredForSameComplianceYearAndObligationType_ValidationFails_AndMessageIncludesPrnAndObligationType_AndErrorLevelIsError(obligationTypeType existingObligationType, obligationTypeType xmlObligationType)
        {
            const string complianceYear = "2016";
            const string registrationNumber = "ABC12345";
            var xml = new schemeType
            {
                complianceYear = complianceYear,
                producerList = new[]
                {
                    new producerType
                    {
                        tradingName = "Test Trader",
                        obligationType = xmlObligationType,
                        registrationNo = registrationNumber
                    }
                }
            };

            var existingProducer = FakeProducer.Create(MapObligationType(existingObligationType), registrationNumber);
            var existingScheme = new Scheme(Guid.NewGuid());
            existingScheme.Producers.Add(existingProducer);

            var result = SchemeTypeValidator(existingScheme)
                .Validate(xml, new RulesetValidatorSelector(RequestHandlers.PCS.MemberRegistration.XmlValidation.BusinessValidation.SchemeTypeValidator.DataValidation));

            Assert.False(result.IsValid);
            Assert.Contains(registrationNumber, result.Errors.Single().ErrorMessage);
            Assert.Contains(existingObligationType.ToString(), result.Errors.Single().ErrorMessage);
            Assert.Equal(ErrorLevel.Error, result.Errors.Single().CustomState);
        }

        [Theory]
        [InlineData(obligationTypeType.B2B, obligationTypeType.B2C)]
        [InlineData(obligationTypeType.B2C, obligationTypeType.B2B)]
        public void ProducerAlreadyRegisteredForSameComplianceYearButObligationTypeDiffers_ValidationSucceeds(obligationTypeType existingObligationType, obligationTypeType xmlObligationType)
        {
            const string complianceYear = "2016";
            const string registrationNumber = "ABC12345";
            var xml = new schemeType()
            {
                complianceYear = complianceYear,
                producerList = new[]
                {
                    new producerType
                    {
                        obligationType = xmlObligationType,
                        registrationNo = registrationNumber
                    }
                }
            };

            var existingProducer = FakeProducer.Create(MapObligationType(existingObligationType), registrationNumber);
            var existingScheme = new Scheme(Guid.NewGuid());
            existingScheme.Producers.Add(existingProducer);

            var result = SchemeTypeValidator(existingScheme)
                .Validate(xml, new RulesetValidatorSelector(RequestHandlers.PCS.MemberRegistration.XmlValidation.BusinessValidation.SchemeTypeValidator.DataValidation));

            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(obligationTypeType.B2B, obligationTypeType.B2B)]
        [InlineData(obligationTypeType.B2C, obligationTypeType.B2C)]
        [InlineData(obligationTypeType.Both, obligationTypeType.B2C)]
        [InlineData(obligationTypeType.Both, obligationTypeType.B2B)]
        [InlineData(obligationTypeType.B2B, obligationTypeType.Both)]
        [InlineData(obligationTypeType.B2C, obligationTypeType.Both)]
        public void ProducerRegisteredForDifferentComplianceYearButObligationTypeMatches_ValidationSucceeds(obligationTypeType existingObligationType, obligationTypeType xmlObligationType)
        {
            const string complianceYear = "2015";
            const string registrationNumber = "ABC12345";
            var xml = new schemeType()
            {
                complianceYear = complianceYear,
                producerList = new[]
                {
                    new producerType
                    {
                        obligationType = xmlObligationType,
                        registrationNo = registrationNumber
                    }
                }
            };

            var existingProducer = FakeProducer.Create(MapObligationType(existingObligationType), registrationNumber);
            var existingScheme = new Scheme(Guid.NewGuid());
            existingScheme.Producers.Add(existingProducer);

            var result = SchemeTypeValidator(existingScheme)
                .Validate(xml, new RulesetValidatorSelector(RequestHandlers.PCS.MemberRegistration.XmlValidation.BusinessValidation.SchemeTypeValidator.DataValidation));

            Assert.True(result.IsValid);
        }

        [Fact]
        public void ProducerRegisteredForSameComplianceYearAndObligationTypeButPartOfSameScheme_ValidationSucceeds()
        {
            const string complianceYear = "2016";
            const string registrationNumber = "ABC12345";
            var organisationId = Guid.NewGuid();
            const obligationTypeType obligationType = obligationTypeType.B2B;
            var xml = new schemeType()
            {
                complianceYear = complianceYear,
                producerList = new[]
                {
                    new producerType
                    {
                        obligationType = obligationType,
                        registrationNo = registrationNumber
                    }
                }
            };

            var existingProducer = FakeProducer.Create(MapObligationType(obligationType), registrationNumber, organisationId);
            var existingScheme = new Scheme(organisationId);
            existingScheme.Producers.Add(existingProducer);

            var result = SchemeTypeValidator(existingScheme, organisationId)
                .Validate(xml, new RulesetValidatorSelector(RequestHandlers.PCS.MemberRegistration.XmlValidation.BusinessValidation.SchemeTypeValidator.DataValidation));

            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ProducerRegisteredMatchesComplianceYearAndObligationType_ButRegistrationNumberIsNullOrEmpty_ValidationSucceeds(string registrationNumber)
        {
            const string complianceYear = "2016";
            var xml = new schemeType()
            {
                complianceYear = complianceYear,
                producerList = new[]
                {
                    new producerType
                    {
                        obligationType = obligationTypeType.B2B,
                        registrationNo = registrationNumber
                    }
                }
            };

            var existingProducer = FakeProducer.Create(MapObligationType(obligationTypeType.B2B), registrationNumber);
            var existingScheme = new Scheme(Guid.NewGuid());
            existingScheme.Producers.Add(existingProducer);

            var result = SchemeTypeValidator(existingScheme)
                .Validate(xml, new RulesetValidatorSelector(RequestHandlers.PCS.MemberRegistration.XmlValidation.BusinessValidation.SchemeTypeValidator.DataValidation));

            Assert.True(result.IsValid);
        }

        private IValidator<schemeType> SchemeTypeValidator(Guid? existingOrganisationId = null, Guid? organisationId = null)
        {
            return SchemeTypeValidator(new Scheme(existingOrganisationId ?? Guid.NewGuid()), organisationId);
        }

        private IValidator<schemeType> SchemeTypeValidator(Scheme scheme, Guid? organisationId = null)
        {
            return new SchemeTypeValidator(ValidationContext.Create(scheme), organisationId ?? Guid.NewGuid());
        }

        private producerType[] Producers(params string[] regstrationNumbers)
        {
            return regstrationNumbers.Select(r => new producerType
            {
                status = statusType.A,
                registrationNo = r
            }).ToArray();
        }

        private Producer Producer(ObligationType obligationType, string prn, params string[] brandNames)
        {
            return new Producer(Guid.NewGuid(),
                new MemberUpload(Guid.NewGuid(), "<xml>SomeData</xml>", new List<MemberUploadError>(), 0),
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
                obligationType,
                AnnualTurnOverBandType.Greaterthanonemillionpounds,
                brandNames.Select(bn => new BrandName(bn)).ToList(),
                new List<SICCode>(),
                true,
                ChargeBandType.A);
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
    }
}
