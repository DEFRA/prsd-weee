namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme.MemberRegistration.XmlValidation.BusinessValidation.Rules
{
    using System;
    using Domain;
    using Domain.Producer;
    using FakeItEasy;
    using RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.Queries;
    using RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.Rules;
    using Xunit;

    public class RulesTests
    {
        private readonly IQueries queries;

        public RulesTests()
        {
            queries = A.Fake<IQueries>();
        }

        [Fact]
        public void ShouldNotWarnOfProducerNameChange_Insert_AndProducerExistsWithMatchingPrnInComplianceYearAndScheme_ReturnsTrue()
        {
            A.CallTo(() => queries.GetLatestProducerForComplianceYearAndScheme(A<string>._, A<string>._, A<Guid>._))
                .Returns(FakeProducer.Create(ObligationType.Both, "ABC12345"));

            var producerName = "Test Producer Name";

            var result = Rules().ShouldNotWarnOfProducerNameChange(new schemeType(), new producerType
            {
                status = statusType.I,
                producerBusiness = new producerBusinessType
                {
                    Item = new partnershipType
                    {
                        partnershipName = "New Producer Name"
                    }
                }
            }, A<Guid>._);

            Assert.True(result);
        }

        [Fact]
        public void ShouldNotWarnOfProducerNameChange_Amendment_AndProducerExistsWithMatchingPrnInComplianceYearAndScheme_ReturnsFalse()
        {
            A.CallTo(() => queries.GetLatestProducerForComplianceYearAndScheme(A<string>._, A<string>._, A<Guid>._))
                .Returns(FakeProducer.Create(ObligationType.Both, "ABC12345"));

            var producerName = "Test Producer Name";

            var result = Rules().ShouldNotWarnOfProducerNameChange(new schemeType(), new producerType
            {
                status = statusType.A,
                producerBusiness = new producerBusinessType
                {
                    Item = new partnershipType
                    {
                        partnershipName = "New Producer Name"
                    }
                }
            }, A<Guid>._);

            Assert.False(result);
        }

        [Fact]
        public void
            ShouldNotWarnOfProducerNameChange_Amendment_AndProducerExistsWithMatchingPrnInPreviousComplianceYear_ReturnsFalse
            ()
        {
            A.CallTo(() => queries.GetLatestProducerFromPreviousComplianceYears(A<string>._))
                .Returns(FakeProducer.Create(ObligationType.Both, "ABC12345"));

            var producerName = "Test Producer Name";

            var result = Rules().ShouldNotWarnOfProducerNameChange(new schemeType(), new producerType
            {
                status = statusType.A,
                producerBusiness = new producerBusinessType
                {
                    Item = new partnershipType
                    {
                        partnershipName = "New Producer Name"
                    }
                }
            }, A<Guid>._);

            Assert.False(result);
        }

        [Fact]
        public void ShouldNotWarnOfProducerNameChange_Amendment_MigratedProducerExistsWithPrn_ReturnsFalse()
        {
            A.CallTo(() => queries.GetMigratedProducer(A<string>._))
                .Returns(new FakeMigratedProducer());

            var producerName = "Test Producer Name";

            var result = Rules().ShouldNotWarnOfProducerNameChange(new schemeType(), new producerType
            {
                status = statusType.A,
                producerBusiness = new producerBusinessType
                {
                    Item = new partnershipType
                    {
                        partnershipName = "New Producer Name"
                    }
                }
            }, A<Guid>._);

            Assert.False(result);
        }

        private Rules Rules()
        {
            return new Rules(queries);
        }

        private class FakeMigratedProducer : MigratedProducer
        {
        }
    }
}
