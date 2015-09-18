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
        private readonly IProducerQuerySet querySet;

        public ProducerNameWarningTests()
        {
            querySet = A.Fake<IProducerQuerySet>();

            // By default, nulls returned from queries
            A.CallTo(() => querySet.GetLatestProducerForComplianceYearAndScheme(A<string>._, A<string>._, A<Guid>._))
                .Returns(null);
            A.CallTo(() => querySet.GetLatestProducerFromPreviousComplianceYears(A<string>._))
                .Returns(null);
            A.CallTo(() => querySet.GetMigratedProducer(A<string>._))
                .Returns(null);
        }

        [Fact]
        public void Evaluate_Insert_AndProducerExistsWithMatchingPrnInComplianceYearAndScheme_ReturnsPass()
        {
            A.CallTo(() => querySet.GetLatestProducerForComplianceYearAndScheme(A<string>._, A<string>._, A<Guid>._))
                .Returns(FakeProducer.Create(ObligationType.Both, "ABC12345"));

            var result = ProducerNameWarning().Evaluate(new ProducerNameWarning(new schemeType(), new producerType
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

            Assert.True(result.IsValid);
        }

        [Fact]
        public void Evaluate_Amendment_AndProducerExistsWithMatchingPrnInComplianceYearAndScheme_ReturnsFailAsWarning()
        {
            A.CallTo(() => querySet.GetLatestProducerForComplianceYearAndScheme(A<string>._, A<string>._, A<Guid>._))
                .Returns(FakeProducer.Create(ObligationType.Both, "ABC12345"));

            var result = ProducerNameWarning().Evaluate(new ProducerNameWarning(new schemeType(), new producerType
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

            Assert.False(result.IsValid);
            Assert.Equal(Core.Shared.ErrorLevel.Warning, result.ErrorLevel);
        }

        [Fact]
        public void Evaluate_Amendment_AndProducerExistsWithMatchingPrnInPreviousComplianceYear_ReturnsFailAsWarning_AndIncludesProducerNamesInErrorMessage()
        {
            A.CallTo(() => querySet.GetLatestProducerFromPreviousComplianceYears(A<string>._))
                .Returns(FakeProducer.Create(ObligationType.Both, "ABC12345"));

            var result = ProducerNameWarning().Evaluate(new ProducerNameWarning(new schemeType(), new producerType
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

            Assert.False(result.IsValid);
            Assert.Equal(Core.Shared.ErrorLevel.Warning, result.ErrorLevel);
        }

        [Fact]
        public void Evaluate_Amendment_MigratedProducerExistsWithPrn_ReturnsFailAsWarning_AndIncludesProducerNamesInErrorMessage()
        {
            const string existingProducerName = "existing producer name";
            const string newProducerName = "new producer Name";

            var migratedProducer = A.Fake<MigratedProducer>();
            A.CallTo(() => migratedProducer.ProducerName)
                .Returns(existingProducerName);

            A.CallTo(() => querySet.GetMigratedProducer(A<string>._))
                .Returns(migratedProducer);

            var result = ProducerNameWarning().Evaluate(new ProducerNameWarning(new schemeType(), new producerType
            {
                status = statusType.A,
                producerBusiness = new producerBusinessType
                {
                    Item = new partnershipType
                    {
                        partnershipName = newProducerName
                    }
                }
            }, A<Guid>._));

            Assert.False(result.IsValid);
            Assert.Equal(Core.Shared.ErrorLevel.Warning, result.ErrorLevel);
            Assert.Contains(existingProducerName, result.Message);
            Assert.Contains(newProducerName, result.Message);
        }

        private ProducerNameWarningEvaluator ProducerNameWarning()
        {
            return new ProducerNameWarningEvaluator(querySet);
        }
    }
}
