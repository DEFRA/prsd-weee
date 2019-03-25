namespace EA.Weee.XmlValidation.Tests.Unit.BusinessValidation.MemberRegistration.Rules.Producer
{
    using System;
    using FakeItEasy;
    using Weee.Domain.Lookup;
    using Xml.MemberRegistration;
    using XmlValidation.BusinessValidation;
    using XmlValidation.BusinessValidation.MemberRegistration.QuerySets;
    using XmlValidation.BusinessValidation.MemberRegistration.Rules.Producer;
    using Xunit;
    using ProducerSubmission = EA.Weee.Domain.Producer.ProducerSubmission;

    public class ProducerChargeBandChangeTests
    {
        private readonly IProducerChargeBandCalculatorChooser producerChargeBandCalculatorChooser;

        public ProducerChargeBandChangeTests()
        {
            producerChargeBandCalculatorChooser = A.Fake<IProducerChargeBandCalculatorChooser>();
        }

        [Fact]
        public void Evaluate_Insert_ReturnsPass()
        {
            var result = new ProducerChargeBandChangeEvaluator(producerChargeBandCalculatorChooser).Evaluate(ChargeBand.B, statusType.I);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void Evaluate_Amendment_AndProducerExistsWithMatchingChargeBand_ReturnsPass()
        {
            var evaluator = new ProducerChargeBandChangeEvaluator(producerChargeBandCalculatorChooser);

            var fakeProducer = A.Fake<ProducerSubmission>();

            ChargeBandAmount producerChargeBand = new ChargeBandAmount(
                new Guid("0B513437-2971-4C6C-B633-75216FAB6757"),
                ChargeBand.E,
                123);

            A.CallTo(() => fakeProducer.ChargeBandAmount).Returns(producerChargeBand);
            A.CallTo(() => evaluator.QuerySet.GetLatestProducerForComplianceYearAndScheme(A<string>._, A<string>._, A<Guid>._))
                .Returns(fakeProducer);
            A.CallTo(() => producerChargeBandCalculatorChooser.GetProducerChargeBand(A<schemeType>._, A<producerType>._)).Returns(ChargeBand.E);

            var result = evaluator.Evaluate(ChargeBand.E);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void Evaluate_Amendment_AndProducerExistsWithDifferentChargeBand_FailAsWarning()
        {
            var evaluator = new ProducerChargeBandChangeEvaluator(producerChargeBandCalculatorChooser);

            var fakeProducer = A.Fake<ProducerSubmission>();

            ChargeBandAmount chargeBandAmount = new ChargeBandAmount(
                new Guid("0B513437-2971-4C6C-B633-75216FAB6757"),
                ChargeBand.E,
                123);

            A.CallTo(() => fakeProducer.ChargeBandAmount)
                .Returns(chargeBandAmount);
            A.CallTo(() => producerChargeBandCalculatorChooser.GetProducerChargeBand(A<schemeType>._, A<producerType>._)).Returns(ChargeBand.B);
            A.CallTo(() => evaluator.QuerySet.GetLatestProducerForComplianceYearAndScheme(A<string>._, A<string>._, A<Guid>._))
                .Returns(fakeProducer);

            var result = evaluator.Evaluate(ChargeBand.B);

            Assert.False(result.IsValid);
            Assert.Equal(Core.Shared.ErrorLevel.Warning, result.ErrorLevel);
        }

        [Fact]
        public void Evaluate_Amendment_AndProducerExistsWithDifferentChargeBand_FailAsWarning_AndIncludesProducerDetailsAndChargeBandsInErrorMessage()
        {
            string producerName = "ProdA";
            string registrationNumber = "reg123";
            var existingChargeBand = ChargeBand.A;
            var newChargeBand = ChargeBand.B;

            var evaluator = new ProducerChargeBandChangeEvaluator(producerChargeBandCalculatorChooser);

            var fakeProducer = A.Fake<ProducerSubmission>();

            ChargeBandAmount chargeBandAmount = new ChargeBandAmount(
                new Guid("0B513437-2971-4C6C-B633-75216FAB6757"),
                existingChargeBand,
                123);

            A.CallTo(() => fakeProducer.ChargeBandAmount).Returns(chargeBandAmount);
            A.CallTo(() => fakeProducer.OrganisationName).Returns(producerName);
            A.CallTo(() => fakeProducer.RegisteredProducer.ProducerRegistrationNumber).Returns(registrationNumber);
            A.CallTo(() => producerChargeBandCalculatorChooser.GetProducerChargeBand(A<schemeType>._, A<producerType>._)).Returns(newChargeBand);
            A.CallTo(() => evaluator.QuerySet.GetLatestProducerForComplianceYearAndScheme(A<string>._, A<string>._, A<Guid>._))
                .Returns(fakeProducer);

            var result = evaluator.Evaluate(newChargeBand);

            Assert.False(result.IsValid);
            Assert.Equal(Core.Shared.ErrorLevel.Warning, result.ErrorLevel);
            Assert.Contains(producerName, result.Message);
            Assert.Contains(registrationNumber, result.Message);
            Assert.Contains(existingChargeBand.ToString(), result.Message);
            Assert.Contains(newChargeBand.ToString(), result.Message);
        }

        private class ProducerChargeBandChangeEvaluator
        {
            public IProducerQuerySet QuerySet { get; private set; }
            public IProducerChargeBandCalculator ProducerChargeBandCalculator { get; private set; }
            private ProducerChargeBandChange producerChargeBandChange;
            
            public ProducerChargeBandChangeEvaluator(IProducerChargeBandCalculatorChooser chooser)
            {
                QuerySet = A.Fake<IProducerQuerySet>();
                ProducerChargeBandCalculator = A.Fake<IProducerChargeBandCalculator>();

                producerChargeBandChange = new ProducerChargeBandChange(QuerySet, chooser);
            }

            public RuleResult Evaluate(ChargeBand producerChargeBand, statusType producerStatus = statusType.A)
            {
                var producer = new producerType()
                {
                    status = producerStatus
                };

                var scheme = new schemeType()
                {
                    complianceYear = "2016"
                };

                return producerChargeBandChange.Evaluate(scheme, producer, A.Dummy<Guid>());
            }
        }
    }
}
