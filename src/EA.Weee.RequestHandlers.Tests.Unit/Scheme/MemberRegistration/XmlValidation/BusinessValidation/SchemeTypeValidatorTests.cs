namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme.MemberRegistration.XmlValidation.BusinessValidation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DataAccess;
    using Domain;
    using Domain.Organisation;
    using Domain.Producer;
    using Domain.Scheme;
    using FakeItEasy;
    using FluentValidation;
    using FluentValidation.Internal;
    using Helpers;
    using RequestHandlers;
    using RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation;
    using Xunit;
    using ValidationContext = XmlValidation.ValidationContext;

    public class SchemeTypeValidatorTests
    {
        [Fact]
        public void
            SetOfDuplicateRegistrationNumbers_ValidationFails_IncludesRegistraionNumberInMessage_AndErrorLevelIsError()
        {
            const string registrationNumber = "ABC12345";
            var xml = new schemeType
            {
                producerList = ProducersWithRegistrationNumbers(registrationNumber, registrationNumber)
            };

            var result = SchemeTypeValidator()
                .Validate(xml,
                    new RulesetValidatorSelector(
                        RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.SchemeTypeValidator
                            .NonDataValidation));

            Assert.False(result.IsValid);
            Assert.Contains(registrationNumber, result.Errors.Single().ErrorMessage);
            Assert.Equal(ErrorLevel.Error, result.Errors.Single().CustomState);
        }

        [Fact]
        public void SetOfEmptyRegistrationNumbers_ValidationSucceeds()
        {
            var xml = new schemeType
            {
                producerList = ProducersWithRegistrationNumbers(string.Empty, string.Empty)
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
                producerList =
                    ProducersWithRegistrationNumbers(firstRegistrationNumber, firstRegistrationNumber,
                        secondRegistrationNumber, secondRegistrationNumber)
            };

            var result = SchemeTypeValidator()
                .Validate(xml,
                    new RulesetValidatorSelector(
                        RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.SchemeTypeValidator
                            .NonDataValidation));

            Assert.False(result.IsValid);

            var aggregatedErrorMessages =
                result.Errors.Select(err => err.ErrorMessage).Aggregate((curr, next) => curr + ", " + next);

            Assert.Contains(firstRegistrationNumber, aggregatedErrorMessages);
            Assert.Contains(secondRegistrationNumber, aggregatedErrorMessages);
        }

        [Fact]
        public void TwoProducersWithDifferentRegistrationNumbers_ValidationSucceeds()
        {
            var xml = new schemeType
            {
                producerList = ProducersWithRegistrationNumbers("ABC12345", "XYZ54321").ToArray()
            };

            var result = SchemeTypeValidator()
                .Validate(xml,
                    new RulesetValidatorSelector(
                        RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.SchemeTypeValidator
                            .NonDataValidation));

            Assert.True(result.IsValid);
        }

        [Fact]
        public void ProducerWithoutProducerName_ThrowsArgumentException()
        {
            const string producerName = null;
            var xml = new schemeType
            {
                producerList = ProducersWithProducerNames(producerName)
            };

            Assert.Throws<ArgumentException>(() => SchemeTypeValidator()
                .Validate(xml,
                    new RulesetValidatorSelector(
                        RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.SchemeTypeValidator
                            .NonDataValidation)));
        }

        [Fact]
        public void ProducerWithEmptyProducerName_ThrowsArgumentException()
        {
            var producerName = string.Empty;
            var xml = new schemeType
            {
                producerList = ProducersWithProducerNames(producerName)
            };

            Assert.Throws<ArgumentException>(() => SchemeTypeValidator()
                .Validate(xml,
                    new RulesetValidatorSelector(
                        RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.SchemeTypeValidator
                            .NonDataValidation)));
        }

        [Fact]
        public void SetOfDuplicateProducerNames_ValidationFails_IncludesProducerNameInMessage_AndErrorLevelIsError()
        {
            const string producerName = "Producer Name";
            var xml = new schemeType
            {
                producerList = ProducersWithProducerNames(producerName, producerName)
            };

            var result = SchemeTypeValidator()
                .Validate(xml,
                    new RulesetValidatorSelector(
                        RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.SchemeTypeValidator
                            .NonDataValidation));

            Assert.False(result.IsValid);
            Assert.Contains(producerName, result.Errors.Single().ErrorMessage);
            Assert.Equal(ErrorLevel.Error, result.Errors.Single().CustomState);
        }

        [Fact]
        public void TwoSetsOfDuplicateProducerNames_ValidationFails_IncludesBothProducerNamesInMessages()
        {
            const string firstProducerName = "First Producer Name";
            const string secondProducerName = "Second Producer Name";
            var xml = new schemeType
            {
                producerList =
                    ProducersWithProducerNames(firstProducerName, firstProducerName, secondProducerName,
                        secondProducerName)
            };

            var result = SchemeTypeValidator()
                .Validate(xml,
                    new RulesetValidatorSelector(
                        RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.SchemeTypeValidator
                            .NonDataValidation));

            Assert.False(result.IsValid);

            var aggregatedErrorMessages =
                result.Errors.Select(err => err.ErrorMessage).Aggregate((curr, next) => curr + ", " + next);

            Assert.Contains(firstProducerName, aggregatedErrorMessages);
            Assert.Contains(secondProducerName, aggregatedErrorMessages);
        }

        [Fact]
        public void TwoProducersWithDifferentProducerNames_ValidationSucceeds()
        {
            var xml = new schemeType
            {
                producerList = ProducersWithProducerNames("First Producer Name", "Second Producer Name")
            };

            var result = SchemeTypeValidator()
                .Validate(xml,
                    new RulesetValidatorSelector(
                        RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.SchemeTypeValidator
                            .NonDataValidation));

            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(obligationTypeType.B2B, obligationTypeType.B2B)]
        [InlineData(obligationTypeType.B2C, obligationTypeType.B2C)]
        [InlineData(obligationTypeType.Both, obligationTypeType.B2C)]
        [InlineData(obligationTypeType.Both, obligationTypeType.B2B)]
        [InlineData(obligationTypeType.B2B, obligationTypeType.Both)]
        [InlineData(obligationTypeType.B2C, obligationTypeType.Both)]
        public void
            ProducerAlreadyRegisteredForSameComplianceYearAndObligationType_ValidationFails_AndMessageIncludesPrnAndObligationType_AndErrorLevelIsError
            (obligationTypeType existingObligationType, obligationTypeType xmlObligationType)
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
                .Validate(xml,
                    new RulesetValidatorSelector(
                        RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.SchemeTypeValidator
                            .DataValidation));

            Assert.False(result.IsValid);
            Assert.Contains(registrationNumber, result.Errors.Single().ErrorMessage);
            Assert.Contains(existingObligationType.ToString(), result.Errors.Single().ErrorMessage);
            Assert.Equal(ErrorLevel.Error, result.Errors.Single().CustomState);
        }

        [Theory]
        [InlineData(obligationTypeType.B2B, obligationTypeType.B2C)]
        [InlineData(obligationTypeType.B2C, obligationTypeType.B2B)]
        public void ProducerAlreadyRegisteredForSameComplianceYearButObligationTypeDiffers_ValidationSucceeds(
            obligationTypeType existingObligationType, obligationTypeType xmlObligationType)
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
                        obligationType = xmlObligationType,
                        registrationNo = registrationNumber
                    }
                }
            };

            var existingProducer = FakeProducer.Create(MapObligationType(existingObligationType), registrationNumber);
            var existingScheme = new Scheme(Guid.NewGuid());
            existingScheme.Producers.Add(existingProducer);

            var result = SchemeTypeValidator(existingScheme)
                .Validate(xml,
                    new RulesetValidatorSelector(
                        RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.SchemeTypeValidator
                            .DataValidation));

            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(obligationTypeType.B2B, obligationTypeType.B2B)]
        [InlineData(obligationTypeType.B2C, obligationTypeType.B2C)]
        [InlineData(obligationTypeType.Both, obligationTypeType.B2C)]
        [InlineData(obligationTypeType.Both, obligationTypeType.B2B)]
        [InlineData(obligationTypeType.B2B, obligationTypeType.Both)]
        [InlineData(obligationTypeType.B2C, obligationTypeType.Both)]
        public void ProducerRegisteredForDifferentComplianceYearButObligationTypeMatches_ValidationSucceeds(
            obligationTypeType existingObligationType, obligationTypeType xmlObligationType)
        {
            const string complianceYear = "2015";
            const string registrationNumber = "ABC12345";
            var xml = new schemeType
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
                .Validate(xml,
                    new RulesetValidatorSelector(
                        RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.SchemeTypeValidator
                            .DataValidation));

            Assert.True(result.IsValid);
        }

        [Fact]
        public void ProducerRegisteredForSameComplianceYearAndObligationTypeButPartOfSameScheme_ValidationSucceeds()
        {
            const string complianceYear = "2016";
            const string registrationNumber = "ABC12345";
            var organisationId = Guid.NewGuid();
            const obligationTypeType obligationType = obligationTypeType.B2B;
            var xml = new schemeType
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

            var existingProducer = FakeProducer.Create(MapObligationType(obligationType), registrationNumber,
                organisationId);
            var existingScheme = new Scheme(organisationId);
            existingScheme.Producers.Add(existingProducer);

            var result = SchemeTypeValidator(existingScheme, organisationId)
                .Validate(xml,
                    new RulesetValidatorSelector(
                        RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.SchemeTypeValidator
                            .DataValidation));

            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void
            ProducerRegisteredMatchesComplianceYearAndObligationType_ButRegistrationNumberIsNullOrEmpty_ValidationSucceeds
            (string registrationNumber)
        {
            const string complianceYear = "2016";
            var xml = new schemeType
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
                .Validate(xml,
                    new RulesetValidatorSelector(
                        RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.SchemeTypeValidator
                            .DataValidation));

            Assert.True(result.IsValid);
        }

        private IValidator<schemeType> SchemeTypeValidator(Guid? existingOrganisationId = null,
            Guid? organisationId = null)
        {
            return SchemeTypeValidator(new Scheme(existingOrganisationId ?? Guid.NewGuid()), organisationId);
        }

        private IValidator<schemeType> SchemeTypeValidator(Scheme scheme, Guid? organisationId = null)
        {
            return new SchemeTypeValidator(ValidationContext.Create(scheme), organisationId ?? Guid.NewGuid());
        }

        private producerType[] ProducersWithRegistrationNumbers(params string[] regstrationNumbers)
        {
            return regstrationNumbers.Select(r => new producerType
            {
                status = statusType.A,
                registrationNo = r,
                producerBusiness = new producerBusinessType
                {
                    Item = new partnershipType
                    {
                        partnershipName = Guid.NewGuid().ToString()
                    }
                }
            }).ToArray();
        }

        private producerType[] ProducersWithProducerNames(params string[] producerNames)
        {
            return producerNames.Select(n => new producerType
            {
                status = statusType.I,
                producerBusiness = new producerBusinessType
                {
                    Item = new partnershipType
                    {
                        partnershipName = n
                    }
                }
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

        [Theory]
        [InlineData(obligationTypeType.B2B)]
        [InlineData(obligationTypeType.B2C)]
        public void
            ProducerRegisteredforAnotherSchemeforDifferentObligationTypeForDifferentComplianceYear_ValidationSucceeds(
            obligationTypeType obligationType)
        {
            var weeeContext = CreateFakeDatabase();
            var xml = new schemeType
            {
                complianceYear = "2016",
                producerList = new[]
                {
                    new producerType
                    {
                        obligationType = obligationType,
                        registrationNo = "ABC"
                    }
                }
            };

            var orgId = new Guid("20C569E6-C4A0-40C2-9D87-120906D5434B");
            var scheme = weeeContext.Schemes.FirstOrDefault(s => s.OrganisationId == orgId);
            var result = SchemeTypeValidator(scheme, orgId)
                .Validate(xml,
                    new RulesetValidatorSelector(
                        RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.SchemeTypeValidator
                            .DataValidation));

            if (obligationType == obligationTypeType.B2B)
            {
                Assert.True(result.IsValid);
            }
            else
            {
                Assert.False(result.IsValid);
            }
        }

        private readonly DbContextHelper dbContextHelper = new DbContextHelper();

        /// <summary>
        ///     Sets up a faked WeeeContext with 2 schemes
        /// </summary>
        /// <returns></returns>
        private WeeeContext CreateFakeDatabase()
        {
            var memberUpload1 = A.Fake<MemberUpload>();
            A.CallTo(() => memberUpload1.OrganisationId).Returns(new Guid("20C569E6-C4A0-40C2-9D87-120906D5434B"));
            A.CallTo(() => memberUpload1.ComplianceYear).Returns(2016);
            A.CallTo(() => memberUpload1.IsSubmitted).Returns(true);

            var memberUpload2 = A.Fake<MemberUpload>();
            A.CallTo(() => memberUpload2.OrganisationId).Returns(new Guid("20C569E6-C4A0-40C2-9D87-120906D5434B"));
            A.CallTo(() => memberUpload2.ComplianceYear).Returns(2017);
            A.CallTo(() => memberUpload2.IsSubmitted).Returns(true);

            Producer producer1 = FakeProducer.Create(MapObligationType(obligationTypeType.B2B), "ABC", false);

            Producer producer2 = FakeProducer.Create(MapObligationType(obligationTypeType.B2C), "ABC", true);

            var organisation1 = A.Fake<Organisation>();
            A.CallTo(() => organisation1.TradingName).Returns("Test Trading Name 1");

            var organisation2 = A.Fake<Organisation>();
            A.CallTo(() => organisation2.TradingName).Returns("Test Trading Name 2");

            var scheme1 = A.Fake<Scheme>();
            A.CallTo(() => scheme1.OrganisationId).Returns(new Guid("20C569E6-C4A0-40C2-9D87-120906D5434B"));
            A.CallTo(() => scheme1.ApprovalNumber).Returns("Test Approval Number 1");

            var scheme2 = A.Fake<Scheme>();
            A.CallTo(() => scheme2.OrganisationId).Returns(new Guid("A99F0A92-E08D-47F0-9758-82F02DB816FA"));
            A.CallTo(() => scheme2.ApprovalNumber).Returns("Test Approval Number 2");

            // Wire up scheme to organisations (1-way).
            A.CallTo(() => scheme1.Organisation).Returns(organisation1);
            A.CallTo(() => scheme2.Organisation).Returns(organisation2);

            // Wire up member uploads to organisations (1-way)
            A.CallTo(() => memberUpload1.Organisation).Returns(organisation1);
            A.CallTo(() => memberUpload2.Organisation).Returns(organisation1);
            //A.CallTo(() => memberUpload3.Organisation).Returns(organisation2);

            // Wire up member uploads to schemes (1-way)
            A.CallTo(() => memberUpload1.Scheme).Returns(scheme1);
            A.CallTo(() => memberUpload2.Scheme).Returns(scheme1);
            //A.CallTo(() => memberUpload3.Scheme).Returns(scheme2);

            // Wire up producers and schemes (2-way).
            A.CallTo(() => scheme1.Producers).Returns(new List<Producer>
            {
                producer1,
                producer2
            });

            // Wire up producers and member uploads (2-way).
            A.CallTo(() => memberUpload1.Producers).Returns(new List<Producer>
            {
                producer1
            });

            A.CallTo(() => memberUpload2.Producers).Returns(new List<Producer>
            {
                producer2
            });

            // Wire up everything to the context (1-way).
            var weeeContext = A.Fake<WeeeContext>();

            var schemesDbSet = dbContextHelper.GetAsyncEnabledDbSet(new List<Scheme>
            {
                scheme1,
                scheme2
            });
            A.CallTo(() => weeeContext.Schemes).Returns(schemesDbSet);

            var producersDbSet = dbContextHelper.GetAsyncEnabledDbSet(new List<Producer>
            {
                producer1,
                producer2
            });
            A.CallTo(() => weeeContext.Producers).Returns(producersDbSet);

            var organisationDbSet = dbContextHelper.GetAsyncEnabledDbSet(new List<Organisation>
            {
                organisation1,
                organisation2
            });
            A.CallTo(() => weeeContext.Organisations).Returns(organisationDbSet);

            var memberUploadDbSet = dbContextHelper.GetAsyncEnabledDbSet(new List<MemberUpload>
            {
                memberUpload1,
                memberUpload2
            });
            A.CallTo(() => weeeContext.MemberUploads).Returns(memberUploadDbSet);

            return weeeContext;
        }
    }
}
