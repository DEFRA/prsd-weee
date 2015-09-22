//namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme.MemberRegistration.XmlValidation.BusinessValidation.RuleEvaluators
//{
//    using System;
//    using Domain;
//    using FakeItEasy;
//    using RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.QuerySets;
//    using RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.RuleEvaluators;
//    using RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.Rules;
//    using Xunit;

//    public class ProducerAlreadyRegisteredEvaluatorTests
//    {
//        private readonly IProducerQuerySet querySet;

//        public ProducerAlreadyRegisteredEvaluatorTests()
//        {
//            querySet = A.Fake<IProducerQuerySet>();

//            // By default, nulls returned from queries
//            A.CallTo(() => querySet.GetLatestProducerForComplianceYearAndScheme(A<string>._, A<string>._, A<Guid>._))
//                .Returns(null);
//            A.CallTo(() => querySet.GetLatestProducerFromPreviousComplianceYears(A<string>._))
//                .Returns(null);
//            A.CallTo(() => querySet.GetMigratedProducer(A<string>._))
//                .Returns(null);

//            A.CallTo(() => querySet.GetProducerForOtherSchemeAndObligationType(A<string>._, A<string>._, A<Guid>._, A<int>._))
//                .Returns(null);
//        }

//        [Theory]
//        [InlineData(obligationTypeType.B2B, obligationTypeType.B2B)]
//        [InlineData(obligationTypeType.B2C, obligationTypeType.B2C)]
//        [InlineData(obligationTypeType.Both, obligationTypeType.B2C)]
//        [InlineData(obligationTypeType.Both, obligationTypeType.B2B)]
//        [InlineData(obligationTypeType.B2B, obligationTypeType.Both)]
//        [InlineData(obligationTypeType.B2C, obligationTypeType.Both)]
//        public void
//            ProducerAlreadyRegisteredForSameComplianceYearAndObligationType_ValidationFails_AndMessageIncludesPrnAndObligationType_AndErrorLevelIsError
//            (obligationTypeType existingObligationType, obligationTypeType xmlObligationType)
//        {
//            A.CallTo(() => querySet.GetProducerForOtherSchemeAndObligationType(A<string>._, A<string>._, A<Guid>._, A<int>._))
//                .Returns(FakeProducer.Create(ObligationType.Both, "ABC12345"));

//            const string complianceYear = "2016";
//            const string registrationNumber = "ABC12345";
        
//            producerType producer = new producerType()
//            {
//                tradingName = "Test Trader",
//                obligationType = xmlObligationType,
//                registrationNo = registrationNumber,
//                status = statusType.A,
//                producerBusiness = new producerBusinessType
//                {
//                    Item = new partnershipType
//                    {
//                        partnershipName = "New Producer Name"
//                    }
//                }
//            };

//            schemeType scheme = new schemeType()
//            {
//                complianceYear = complianceYear,
//                producerList = new[]
//                    {
//                       producer
//                    }
//            };
//            var result = ProducerAlreadyRegisteredError().Evaluate(new ProducerAlreadyRegisteredError(scheme, producer, A<Guid>._));

//            Assert.False(result.IsValid);
//            Assert.Contains(registrationNumber, result.Message);
//            Assert.Contains(xmlObligationType.ToString(), result.Message);
//            Assert.Equal(Core.Shared.ErrorLevel.Error, result.ErrorLevel);
//        }

//        [Theory]
//        [InlineData(obligationTypeType.B2B, obligationTypeType.B2C)]
//        [InlineData(obligationTypeType.B2C, obligationTypeType.B2B)]
//        public void ProducerAlreadyRegisteredForSameComplianceYearButObligationTypeDiffers_ValidationSucceeds(
//            obligationTypeType existingObligationType, obligationTypeType xmlObligationType)
//        {
//             A.CallTo(() => querySet.GetProducerForOtherSchemeAndObligationType(A<string>._, A<string>._, A<Guid>._, A<int>._))
//                .Returns(FakeProducer.Create(MapObligationType(existingObligationType), "ABC12345"));

//            const string complianceYear = "2016";
//            const string registrationNumber = "ABC12345";
        
//            producerType producer = new producerType()
//            {
//                tradingName = "Test Trader",
//                obligationType = xmlObligationType,
//                registrationNo = registrationNumber,
//                status = statusType.I,
//                producerBusiness = new producerBusinessType
//                {
//                    Item = new partnershipType
//                    {
//                        partnershipName = "New Producer Name"
//                    }
//                }
//            };

//            schemeType scheme = new schemeType()
//            {
//                complianceYear = complianceYear,
//                producerList = new[]
//                    {
//                       producer
//                    }
//            };
//            var result = ProducerAlreadyRegisteredError().Evaluate(new ProducerAlreadyRegisteredError(scheme, producer, A<Guid>._));

//            Assert.True(result.IsValid);
//        }

//        [Theory]
//        [InlineData(obligationTypeType.B2B, obligationTypeType.B2B)]
//        [InlineData(obligationTypeType.B2C, obligationTypeType.B2C)]
//        [InlineData(obligationTypeType.Both, obligationTypeType.B2C)]
//        [InlineData(obligationTypeType.Both, obligationTypeType.B2B)]
//        [InlineData(obligationTypeType.B2B, obligationTypeType.Both)]
//        [InlineData(obligationTypeType.B2C, obligationTypeType.Both)]
//        public void ProducerRegisteredForDifferentComplianceYearButObligationTypeMatches_ValidationSucceeds(
//            obligationTypeType existingObligationType, obligationTypeType xmlObligationType)
//        {
//            A.CallTo(() => querySet.GetProducerForOtherSchemeAndObligationType(A<string>._, A<string>._, A<Guid>._, A<int>._))
//                .Returns(FakeProducer.Create(MapObligationType(existingObligationType), "ABC12345"));

//            const string complianceYear = "2015";
//            const string registrationNumber = "ABC12345";

//            producerType producer = new producerType()
//            {
//                tradingName = "Test Trader",
//                obligationType = xmlObligationType,
//                registrationNo = registrationNumber,
//                status = statusType.I,
//                producerBusiness = new producerBusinessType
//                {
//                    Item = new partnershipType
//                    {
//                        partnershipName = "New Producer Name"
//                    }
//                }
//            };

//            schemeType scheme = new schemeType()
//            {
//                complianceYear = complianceYear,
//                producerList = new[]
//                    {
//                       producer
//                    }
//            };
//            var result = ProducerAlreadyRegisteredError().Evaluate(new ProducerAlreadyRegisteredError(scheme, producer, A<Guid>._));
//            Assert.True(result.IsValid);
//        }

//        [Fact]
//        public void ProducerRegisteredForSameComplianceYearAndObligationTypeButPartOfSameScheme_ValidationSucceeds()
//        {
//            const string complianceYear = "2016";
//            const string registrationNumber = "ABC12345";
//            var organisationId = Guid.NewGuid();
//            const obligationTypeType obligationType = obligationTypeType.B2B;

//            A.CallTo(() => querySet.GetProducerForOtherSchemeAndObligationType(A<string>._, A<string>._, A<Guid>._, A<int>._))
//              .Returns(FakeProducer.Create(MapObligationType(obligationType), "ABC12345", organisationId));

//            producerType producer = new producerType()
//            {
//                tradingName = "Test Trader",
//                obligationType = obligationType,
//                registrationNo = registrationNumber,
//                status = statusType.I,
//                producerBusiness = new producerBusinessType
//                {
//                    Item = new partnershipType
//                    {
//                        partnershipName = "New Producer Name"
//                    }
//                }
//            };

//            schemeType scheme = new schemeType()
//            {
//                complianceYear = complianceYear,
//                approvalNo = "Test approval number 1",
//                producerList = new[]
//                    {
//                       producer
//                    }
//            };
//            var result = ProducerAlreadyRegisteredError().Evaluate(new ProducerAlreadyRegisteredError(scheme, producer, A<Guid>._));
           
//            Assert.True(result.IsValid);
//        }

//        [Theory]
//        [InlineData(null)]
//        [InlineData("")]
//        public void
//            ProducerRegisteredMatchesComplianceYearAndObligationType_ButRegistrationNumberIsNullOrEmpty_ValidationSucceeds
//            (string registrationNumber)
//        {
//            const string complianceYear = "2016";
//            const obligationTypeType obligationType = obligationTypeType.B2B;

//            A.CallTo(() => querySet.GetProducerForOtherSchemeAndObligationType(A<string>._, A<string>._, A<Guid>._, A<int>._))
//              .Returns(FakeProducer.Create(MapObligationType(obligationType), registrationNumber));

//            producerType producer = new producerType()
//            {
//                tradingName = "Test Trader",
//                obligationType = obligationType,
//                producerBusiness = new producerBusinessType
//                {
//                    Item = new partnershipType
//                    {
//                        partnershipName = "New Producer Name"
//                    }
//                }
//            };

//            schemeType scheme = new schemeType()
//            {
//                complianceYear = complianceYear,
//                producerList = new[]
//                    {
//                       producer
//                    }
//            };
//            var result = ProducerAlreadyRegisteredError().Evaluate(new ProducerAlreadyRegisteredError(scheme, producer, A<Guid>._));
           
//            Assert.True(result.IsValid);
//        }

//        private ProducerAlreadyRegisteredErrorEvaluator ProducerAlreadyRegisteredError()
//        {
//            return new ProducerAlreadyRegisteredErrorEvaluator(querySet);
//        }

//        private ObligationType MapObligationType(obligationTypeType obligationType)
//        {
//            switch (obligationType)
//            {
//                case obligationTypeType.B2B:
//                    return ObligationType.B2B;
//                case obligationTypeType.B2C:
//                    return ObligationType.B2C;

//                default:
//                    return ObligationType.Both;
//            }
//        }
//    }
//}
