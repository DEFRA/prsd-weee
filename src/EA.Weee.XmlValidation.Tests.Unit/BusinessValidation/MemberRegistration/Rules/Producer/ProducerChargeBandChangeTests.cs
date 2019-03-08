﻿namespace EA.Weee.XmlValidation.Tests.Unit.BusinessValidation.MemberRegistration.Rules.Producer
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
            var result = new ProducerChargeBandChangeEvaluator().Evaluate(ChargeBand.B, statusType.I);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void Evaluate_Amendment_AndProducerExistsWithMatchingChargeBand_ReturnsPass()
        {
            var evaluator = new ProducerChargeBandChangeEvaluator();

            var fakeProducer = A.Fake<ProducerSubmission>();

            ChargeBandAmount producerChargeBand = new ChargeBandAmount(
                new Guid("0B513437-2971-4C6C-B633-75216FAB6757"),
                ChargeBand.E,
                123);

            A.CallTo(() => fakeProducer.ChargeBandAmount)
                .Returns(producerChargeBand);

            A.CallTo(() => evaluator.QuerySet.GetLatestProducerForComplianceYearAndScheme(A<string>._, A<string>._, A<Guid>._))
                .Returns(fakeProducer);

            var result = evaluator.Evaluate(ChargeBand.E);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void Evaluate_Amendment_AndProducerExistsWithDifferentChargeBand_FailAsWarning()
        {
            var evaluator = new ProducerChargeBandChangeEvaluator();

            var fakeProducer = A.Fake<ProducerSubmission>();

            ChargeBandAmount chargeBandAmount = new ChargeBandAmount(
                new Guid("0B513437-2971-4C6C-B633-75216FAB6757"),
                ChargeBand.E,
                123);

            A.CallTo(() => fakeProducer.ChargeBandAmount)
                .Returns(chargeBandAmount);

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

            var evaluator = new ProducerChargeBandChangeEvaluator();

            var fakeProducer = A.Fake<ProducerSubmission>();

            ChargeBandAmount chargeBandAmount = new ChargeBandAmount(
                new Guid("0B513437-2971-4C6C-B633-75216FAB6757"),
                existingChargeBand,
                123);

            A.CallTo(() => fakeProducer.ChargeBandAmount).Returns(chargeBandAmount);
            A.CallTo(() => fakeProducer.OrganisationName).Returns(producerName);
            A.CallTo(() => fakeProducer.RegisteredProducer.ProducerRegistrationNumber).Returns(registrationNumber);

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
            public IProducerChargeBandCalculatorChooser ProducerChargeBandCalculatorChooser { get; private set; }

            public ProducerChargeBandChangeEvaluator()
            {
                QuerySet = A.Fake<IProducerQuerySet>();
                ProducerChargeBandCalculatorChooser = A.Fake<IProducerChargeBandCalculatorChooser>();

                producerChargeBandChange = new ProducerChargeBandChange(QuerySet, ProducerChargeBandCalculatorChooser);
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

                A.CallTo(() => ProducerChargeBandCalculator.GetProducerChargeBand(A<producerType>._)).Returns(producerChargeBand);

                return producerChargeBandChange.Evaluate(scheme, producer, A.Dummy<Guid>());
            }
        }
    }
}
