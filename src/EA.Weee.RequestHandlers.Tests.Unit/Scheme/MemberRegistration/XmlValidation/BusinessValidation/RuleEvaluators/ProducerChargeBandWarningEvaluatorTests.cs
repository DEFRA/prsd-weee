namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme.MemberRegistration.XmlValidation.BusinessValidation.RuleEvaluators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Core.XmlBusinessValidation;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Producer;
    using EA.Weee.RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.QuerySets;
    using EA.Weee.RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.RuleEvaluators;
    using EA.Weee.RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.Rules;
    using EA.Weee.RequestHandlers.Scheme.MemberRegistration.XmlValidation.Extensions;
    using FakeItEasy;
    using Xunit;

    public class ProducerChargeBandWarningEvaluatorTests
    {
        [Fact]
        public void Evaluate_Insert_ReturnsPass()
        {
            var producer = new producerType()
            {
                status = statusType.I,
            };

            var result = new EvaluateProducerChargeBandWarningEvaluatorRunner().RunWithProducer(producer);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void Evaluate_Amendment_AndProducerExistsWithMatchingChargeBand_ReturnsPass()
        {
            var runner = new EvaluateProducerChargeBandWarningEvaluatorRunner();

            var fakeProducer = A.Fake<Producer>();
            A.CallTo(() => fakeProducer.ChargeBandType).Returns(ChargeBandType.E.Value);

            A.CallTo(() => runner.QuerySet.GetLatestProducerForComplianceYearAndScheme(A<string>._, A<string>._, A<Guid>._))
                .Returns(fakeProducer);

            var producer = new producerType()
            {
                status = statusType.A
            };

            var result = runner.RunWithProducerChargeBand(ChargeBandType.E);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void Evaluate_Amendment_AndProducerExistsWithDifferentChargeBand_FailAsWarning()
        {
            var runner = new EvaluateProducerChargeBandWarningEvaluatorRunner();

            var fakeProducer = A.Fake<Producer>();
            A.CallTo(() => fakeProducer.ChargeBandType).Returns(ChargeBandType.A.Value);

            A.CallTo(() => runner.QuerySet.GetLatestProducerForComplianceYearAndScheme(A<string>._, A<string>._, A<Guid>._))
                .Returns(fakeProducer);

            var producer = new producerType()
            {
                status = statusType.A
            };

            var result = runner.RunWithProducerChargeBand(ChargeBandType.B);

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

            var runner = new EvaluateProducerChargeBandWarningEvaluatorRunner();

            var fakeProducer = A.Fake<Producer>();
            A.CallTo(() => fakeProducer.ChargeBandType).Returns(existingChargeBand.Value);
            A.CallTo(() => fakeProducer.OrganisationName).Returns(producerName);
            A.CallTo(() => fakeProducer.RegistrationNumber).Returns(registrationNumber);

            A.CallTo(() => runner.QuerySet.GetLatestProducerForComplianceYearAndScheme(A<string>._, A<string>._, A<Guid>._))
                .Returns(fakeProducer);

            var producer = new producerType()
            {
                status = statusType.A,
            };

            var result = runner.RunWithProducerChargeBand(newChargeBand);

            Assert.False(result.IsValid);
            Assert.Equal(Core.Shared.ErrorLevel.Warning, result.ErrorLevel);
            Assert.Contains(producerName, result.Message);
            Assert.Contains(registrationNumber, result.Message);
            Assert.Contains(existingChargeBand.DisplayName, result.Message);
            Assert.Contains(newChargeBand.DisplayName, result.Message);
        }
        
        private class EvaluateProducerChargeBandWarningEvaluatorRunner
        {
            private ProducerChargeBandWarningEvaluator evaluator;
            public IProducerQuerySet QuerySet { get; private set; }
            public schemeType Scheme { get; set; }
            public Guid OrgnanisationId { get; set; }

            public EvaluateProducerChargeBandWarningEvaluatorRunner()
            {
                QuerySet = A.Fake<IProducerQuerySet>();
                evaluator = new ProducerChargeBandWarningEvaluator(QuerySet);
                Scheme = new schemeType();
                OrgnanisationId = Guid.NewGuid();
            }

            public RuleResult RunWithProducer(producerType producer)
            {
                return evaluator.Evaluate(new ProducerChargeBandWarning(Scheme, producer, OrgnanisationId));
            }

            public RuleResult RunWithProducerChargeBand(ChargeBandType chargeBand)
            {
                var producer = new TestProducerType()
                {
                    status = statusType.A,
                    ChargeBandType = chargeBand
                };

                return RunWithProducer(producer);
            }
        }

        private class TestProducerType : producerType
        {
            public ChargeBandType ChargeBandType { get; set; }

            public override ChargeBandType GetProducerChargeBand()
            {
                return ChargeBandType;
            }
        }
    }
}
