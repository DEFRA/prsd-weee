namespace EA.Weee.XmlValidation.Tests.Unit.BusinessValidation.Rules.Producer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Weee.Domain;
    using EA.Weee.Xml;
    using EA.Weee.Xml.Schemas;
    using EA.Weee.XmlValidation.BusinessValidation;
    using EA.Weee.XmlValidation.BusinessValidation.QuerySets;
    using EA.Weee.XmlValidation.BusinessValidation.Rules.Producer;
    using FakeItEasy;
    using Xunit;
    using Producer = EA.Weee.Domain.Producer.Producer;

    public class ProducerChargeBandChangeTests
    {
        [Fact]
        public void Evaluate_Insert_ReturnsPass()
        {
            var result = new ProducerChargeBandChangeEvaluator().Evaluate(ChargeBandType.B, statusType.I);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void Evaluate_Amendment_AndProducerExistsWithMatchingChargeBand_ReturnsPass()
        {
            var evaluator = new ProducerChargeBandChangeEvaluator();

            var fakeProducer = A.Fake<Producer>();
            A.CallTo(() => fakeProducer.ChargeBandType).Returns(ChargeBandType.E.Value);

            A.CallTo(() => evaluator.QuerySet.GetLatestProducerForComplianceYearAndScheme(A<string>._, A<string>._, A<Guid>._))
                .Returns(fakeProducer);

            var result = evaluator.Evaluate(ChargeBandType.E);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void Evaluate_Amendment_AndProducerExistsWithDifferentChargeBand_FailAsWarning()
        {
            var evaluator = new ProducerChargeBandChangeEvaluator();

            var fakeProducer = A.Fake<Producer>();
            A.CallTo(() => fakeProducer.ChargeBandType).Returns(ChargeBandType.A.Value);

            A.CallTo(() => evaluator.QuerySet.GetLatestProducerForComplianceYearAndScheme(A<string>._, A<string>._, A<Guid>._))
                .Returns(fakeProducer);

            var result = evaluator.Evaluate(ChargeBandType.B);

            Assert.False(result.IsValid);
            Assert.Equal(Core.Shared.ErrorLevel.Warning, result.ErrorLevel);
        }

        [Fact]
        public void Evaluate_Amendment_AndProducerExistsWithDifferentChargeBand_FailAsWarning_AndIncludesProducerDetailsAndChargeBandsInErrorMessage()
        {
            string producerName = "ProdA";
            string registrationNumber = "reg123";
            var existingChargeBand = ChargeBandType.A;
            var newChargeBand = ChargeBandType.B;

            var evaluator = new ProducerChargeBandChangeEvaluator();

            var fakeProducer = A.Fake<Producer>();
            A.CallTo(() => fakeProducer.ChargeBandType).Returns(existingChargeBand.Value);
            A.CallTo(() => fakeProducer.OrganisationName).Returns(producerName);
            A.CallTo(() => fakeProducer.RegistrationNumber).Returns(registrationNumber);

            A.CallTo(() => evaluator.QuerySet.GetLatestProducerForComplianceYearAndScheme(A<string>._, A<string>._, A<Guid>._))
                .Returns(fakeProducer);

            var result = evaluator.Evaluate(newChargeBand);

            Assert.False(result.IsValid);
            Assert.Equal(Core.Shared.ErrorLevel.Warning, result.ErrorLevel);
            Assert.Contains(producerName, result.Message);
            Assert.Contains(registrationNumber, result.Message);
            Assert.Contains(existingChargeBand.DisplayName, result.Message);
            Assert.Contains(newChargeBand.DisplayName, result.Message);
        }

        private class ProducerChargeBandChangeEvaluator
        {
            public IProducerQuerySet QuerySet { get; private set; }
            public IProducerChargeBandCalculator ProducerChargeBandCalculator { get; private set; }
            private ProducerChargeBandChange producerChargeBandChange;

            public ProducerChargeBandChangeEvaluator()
            {
                QuerySet = A.Fake<IProducerQuerySet>();
                ProducerChargeBandCalculator = A.Fake<IProducerChargeBandCalculator>();

                producerChargeBandChange = new ProducerChargeBandChange(QuerySet, ProducerChargeBandCalculator);
            }

            public RuleResult Evaluate(ChargeBandType producerChargeBand, statusType producerStatus = statusType.A)
            {
                var producer = new producerType()
                {
                    status = producerStatus
                };

                var scheme = new schemeType()
                {
                    complianceYear = "2016"
                };

                A.CallTo(() => ProducerChargeBandCalculator.GetProducerChargeBand(A<annualTurnoverBandType>._, A<bool>._, A<eeePlacedOnMarketBandType>._))
                    .Returns(producerChargeBand);

                return producerChargeBandChange.Evaluate(scheme, producer, A<Guid>._);
            }
        }
    }
}
