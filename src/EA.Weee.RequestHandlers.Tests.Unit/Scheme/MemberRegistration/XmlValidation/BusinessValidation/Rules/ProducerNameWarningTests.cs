namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme.MemberRegistration.XmlValidation.BusinessValidation.Rules
{
    using System;
    using Domain;
    using Domain.Producer;
    using FakeItEasy;
    using RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.QuerySets;
    using RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.RuleEvaluators;
    using RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.Rules;
    using Xunit;

    public class ProducerNameWarningTests
    {
        private readonly IProducerNameWarningQuerySet querySet;

        public ProducerNameWarningTests()
        {
            querySet = A.Fake<IProducerNameWarningQuerySet>();
        }

        [Fact]
        public void ShouldNotWarnOfProducerNameChange_Insert_AndProducerExistsWithMatchingPrnInComplianceYearAndScheme_ReturnsTrue()
        {
            A.CallTo(() => querySet.GetLatestProducerForComplianceYearAndScheme(A<string>._, A<string>._, A<Guid>._))
                .Returns(FakeProducer.Create(ObligationType.Both, "ABC12345"));

            var producerName = "Test Producer Name";

            var result = ProducerNameWarning().Evaluate(new ProducerNameWarningData(new schemeType(), new producerType
            {
                status = statusType.I,
                producerBusiness = new producerBusinessType
                {
                    Item = new partnershipType
                    {
                        partnershipName = "New Producer Name"
                    }
                }
            }, A<Guid>._));

            Assert.True(result);
        }

        [Fact]
        public void ShouldNotWarnOfProducerNameChange_Amendment_AndProducerExistsWithMatchingPrnInComplianceYearAndScheme_ReturnsFalse()
        {
            A.CallTo(() => querySet.GetLatestProducerForComplianceYearAndScheme(A<string>._, A<string>._, A<Guid>._))
                .Returns(FakeProducer.Create(ObligationType.Both, "ABC12345"));

            var producerName = "Test Producer Name";

            var result = ProducerNameWarning().Evaluate(new ProducerNameWarningData(new schemeType(), new producerType
            {
                status = statusType.A,
                producerBusiness = new producerBusinessType
                {
                    Item = new partnershipType
                    {
                        partnershipName = "New Producer Name"
                    }
                }
            }, A<Guid>._));

            Assert.False(result);
        }

        [Fact]
        public void
            ShouldNotWarnOfProducerNameChange_Amendment_AndProducerExistsWithMatchingPrnInPreviousComplianceYear_ReturnsFalse
            ()
        {
            A.CallTo(() => querySet.GetLatestProducerFromPreviousComplianceYears(A<string>._))
                .Returns(FakeProducer.Create(ObligationType.Both, "ABC12345"));

            var producerName = "Test Producer Name";

            var result = ProducerNameWarning().Evaluate(new ProducerNameWarningData(new schemeType(), new producerType
            {
                status = statusType.A,
                producerBusiness = new producerBusinessType
                {
                    Item = new partnershipType
                    {
                        partnershipName = "New Producer Name"
                    }
                }
            }, A<Guid>._));

            Assert.False(result);
        }

        [Fact]
        public void ShouldNotWarnOfProducerNameChange_Amendment_MigratedProducerExistsWithPrn_ReturnsFalse()
        {
            A.CallTo(() => querySet.GetMigratedProducer(A<string>._))
                .Returns(new FakeMigratedProducer());

            var producerName = "Test Producer Name";

            var result = ProducerNameWarning().Evaluate(new ProducerNameWarningData(new schemeType(), new producerType
            {
                status = statusType.A,
                producerBusiness = new producerBusinessType
                {
                    Item = new partnershipType
                    {
                        partnershipName = "New Producer Name"
                    }
                }
            }, A<Guid>._));

            Assert.False(result);
        }

        private ProducerNameWarningEvaluator ProducerNameWarning()
        {
            return new ProducerNameWarningEvaluator(querySet);
        }

        private class FakeMigratedProducer : MigratedProducer
        {
        }
    }
}
