namespace EA.Weee.XmlValidation.Tests.Unit.BusinessValidation.Rules.Producer
{
    using System;
    using Core.Helpers;
    using Domain;
    using FakeItEasy;
    using Weee.Domain;
    using Xml;
    using Xml.MemberRegistration;
    using XmlValidation.BusinessValidation.QuerySets;
    using XmlValidation.BusinessValidation.Rules.Producer;
    using Xunit;
    using schemeType = Xml.MemberRegistration.schemeType;

    public class ProducerAlreadyRegisteredEvaluatorTests
    {
        private readonly IProducerQuerySet producerQuerySet;
        private readonly IMigratedProducerQuerySet migratedProducerQuerySet;

        public ProducerAlreadyRegisteredEvaluatorTests()
        {
            producerQuerySet = A.Fake<IProducerQuerySet>();
            migratedProducerQuerySet = A.Fake<IMigratedProducerQuerySet>();

            // By default, nulls returned from queries
            A.CallTo(() => producerQuerySet.GetLatestProducerForComplianceYearAndScheme(A<string>._, A<string>._, A<Guid>._))
                .Returns(null);
            A.CallTo(() => producerQuerySet.GetLatestProducerFromPreviousComplianceYears(A<string>._))
                .Returns(null);
            A.CallTo(() => migratedProducerQuerySet.GetMigratedProducer(A<string>._))
                .Returns(null);
            A.CallTo(() => producerQuerySet.GetProducerForOtherSchemeAndObligationType(A<string>._, A<string>._, A<Guid>._, A<int>._))
                .Returns(null);
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
            var schemeId = Guid.NewGuid();

            A.CallTo(() => producerQuerySet.GetProducerForOtherSchemeAndObligationType(A<string>._, A<string>._, schemeId, A<int>._))
                .Returns(FakeProducer.Create(existingObligationType.ToDomainObligationType(), "ABC12345"));

            const string complianceYear = "2016";
            const string registrationNumber = "ABC12345";

            var producer = new producerType()
            {
                tradingName = "Test Trader",
                obligationType = xmlObligationType,
                registrationNo = registrationNumber,
                status = statusType.A,
                producerBusiness = new producerBusinessType
                {
                    Item = new partnershipType
                    {
                        partnershipName = "New Producer Name"
                    }
                }
            };

            schemeType scheme = new schemeType()
            {
                complianceYear = complianceYear,
                producerList = new[]
                    {
                       producer
                    }
            };
            var result = Rule().Evaluate(scheme, producer, schemeId);

            Assert.False(result.IsValid);
            Assert.Contains(registrationNumber, result.Message);
            Assert.Contains(existingObligationType.ToDomainObligationType().ToString(), result.Message);
            Assert.Equal(Core.Shared.ErrorLevel.Error, result.ErrorLevel);
        }

        [Theory]
        [InlineData(obligationTypeType.B2B, obligationTypeType.B2C)]
        [InlineData(obligationTypeType.B2C, obligationTypeType.B2B)]
        public void ProducerAlreadyRegisteredForSameComplianceYearButObligationTypeDiffers_ValidationSucceeds(
            obligationTypeType existingObligationType, obligationTypeType xmlObligationType)
        {
            A.CallTo(() => producerQuerySet.GetProducerForOtherSchemeAndObligationType(A<string>._, A<string>._, A<Guid>._, A<int>._))
               .Returns(FakeProducer.Create(existingObligationType.ToDomainObligationType(), "ABC12345"));

            const string complianceYear = "2016";
            const string registrationNumber = "ABC12345";

            producerType producer = new producerType()
            {
                tradingName = "Test Trader",
                obligationType = xmlObligationType,
                registrationNo = registrationNumber,
                status = statusType.I,
                producerBusiness = new producerBusinessType
                {
                    Item = new partnershipType
                    {
                        partnershipName = "New Producer Name"
                    }
                }
            };

            schemeType scheme = new schemeType()
            {
                complianceYear = complianceYear,
                producerList = new[]
                    {
                       producer
                    }
            };
            var result = Rule().Evaluate(scheme, producer, A<Guid>._);

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
            A.CallTo(() => producerQuerySet.GetProducerForOtherSchemeAndObligationType(A<string>._, A<string>._, A<Guid>._, A<int>._))
                .Returns(FakeProducer.Create(existingObligationType.ToDomainObligationType(), "ABC12345"));

            const string complianceYear = "2015";
            const string registrationNumber = "ABC12345";

            producerType producer = new producerType()
            {
                tradingName = "Test Trader",
                obligationType = xmlObligationType,
                registrationNo = registrationNumber,
                status = statusType.I,
                producerBusiness = new producerBusinessType
                {
                    Item = new partnershipType
                    {
                        partnershipName = "New Producer Name"
                    }
                }
            };

            schemeType scheme = new schemeType()
            {
                complianceYear = complianceYear,
                producerList = new[]
                    {
                       producer
                    }
            };
            var result = Rule().Evaluate(scheme, producer, A<Guid>._);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void ProducerRegisteredForSameComplianceYearAndObligationTypeButPartOfSameScheme_ValidationSucceeds()
        {
            const string complianceYear = "2016";
            const string registrationNumber = "ABC12345";
            var organisationId = Guid.NewGuid();
            const obligationTypeType obligationType = obligationTypeType.B2B;

            A.CallTo(() => producerQuerySet.GetProducerForOtherSchemeAndObligationType(A<string>._, A<string>._, A<Guid>._, A<int>._))
              .Returns(FakeProducer.Create(obligationType.ToDomainObligationType(), "ABC12345", organisationId));

            producerType producer = new producerType()
            {
                tradingName = "Test Trader",
                obligationType = obligationType,
                registrationNo = registrationNumber,
                status = statusType.I,
                producerBusiness = new producerBusinessType
                {
                    Item = new partnershipType
                    {
                        partnershipName = "New Producer Name"
                    }
                }
            };

            schemeType scheme = new schemeType()
            {
                complianceYear = complianceYear,
                approvalNo = "Test approval number 1",
                producerList = new[]
                    {
                       producer
                    }
            };
            var result = Rule().Evaluate(scheme, producer, A<Guid>._);

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
            const obligationTypeType obligationType = obligationTypeType.B2B;

            A.CallTo(() => producerQuerySet.GetProducerForOtherSchemeAndObligationType(A<string>._, A<string>._, A<Guid>._, A<int>._))
              .Returns(FakeProducer.Create(obligationType.ToDomainObligationType(), registrationNumber));

            producerType producer = new producerType()
            {
                tradingName = "Test Trader",
                obligationType = obligationType,
                producerBusiness = new producerBusinessType
                {
                    Item = new partnershipType
                    {
                        partnershipName = "New Producer Name"
                    }
                }
            };

            schemeType scheme = new schemeType()
            {
                complianceYear = complianceYear,
                producerList = new[]
                    {
                       producer
                    }
            };
            var result = Rule().Evaluate(scheme, producer, A<Guid>._);

            Assert.True(result.IsValid);
        }

        private ProducerAlreadyRegistered Rule()
        {
            return new ProducerAlreadyRegistered(producerQuerySet);
        }
    }
}
