namespace EA.Weee.Xml.Tests.Unit.MemberRegistration
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.Lookup;
    using Domain.Producer;
    using Domain.Producer.Classfication;
    using Domain.Scheme;
    using EA.Weee.DataAccess.DataAccess;
    using FakeItEasy;
    using Xml.MemberRegistration;
    using Xunit;

    public class ProducerAmendmentChargeAmountCalculatorTests
    {
        private readonly ProducerAmendmentChargeCalculator calculator;
        private readonly IEnvironmentAgencyProducerChargeBandCalculator environmentAgencyProducerChargeBandCalculator;
        private readonly IRegisteredProducerDataAccess registeredProducerDataAccess;
        private const int ComplianceYear = 2019;

        public ProducerAmendmentChargeAmountCalculatorTests()
        {
            environmentAgencyProducerChargeBandCalculator = A.Fake<IEnvironmentAgencyProducerChargeBandCalculator>();
            registeredProducerDataAccess = A.Fake<IRegisteredProducerDataAccess>();

            calculator = new ProducerAmendmentChargeCalculator(environmentAgencyProducerChargeBandCalculator, registeredProducerDataAccess);
        }

        [Fact]
        public async Task GetChargeAmount_GivenProducerSubmission_RegisteredProducerShouldBeRetrieved()
        {
            var schemeType = new schemeType() { approvalNo = "app", complianceYear = ComplianceYear.ToString() };
            var producerType = new producerType() { registrationNo = "no" };

            await calculator.GetChargeAmount(schemeType, producerType);

            A.CallTo(() => registeredProducerDataAccess.GetProducerRegistration(producerType.registrationNo, ComplianceYear, schemeType.approvalNo))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task GetChargeAmount_GivenCurrentSubmissionsIsMoreThanFiveTonnesAndPreviousProducerSubmissionWasLessThanFiveTonnes_EnvironmentAgencyChargeShouldBeCalculated()
        {
            var schemeType = new schemeType() { approvalNo = "app", complianceYear = ComplianceYear.ToString() };
            var producerType = new producerType() { registrationNo = "no", eeePlacedOnMarketBand = eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket };

            A.CallTo(() => registeredProducerDataAccess.GetProducerRegistration(producerType.registrationNo, ComplianceYear, schemeType.approvalNo))
                .Returns(RegisteredProducer());

            var result = await calculator.GetChargeAmount(schemeType, producerType);

            A.CallTo(() => environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(producerType)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task GetChargeAmount_GivenCurrentSubmissionsIsMoreThanFiveTonnesAndPreviousProducerSubmissionWasLessThanFiveTonnes_EnvironmentAgencyChargeShouldBeReturned()
        {
            var schemeType = new schemeType() { approvalNo = "app", complianceYear = ComplianceYear.ToString() };
            var producerType = new producerType() { registrationNo = "no", eeePlacedOnMarketBand = eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket };
            var chargeBand = ChargeBand.A;

            A.CallTo(() => registeredProducerDataAccess.GetProducerRegistration(producerType.registrationNo, ComplianceYear, schemeType.approvalNo))
                .Returns(RegisteredProducer());
            A.CallTo(() => environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(producerType)).Returns(chargeBand);

            var result = await calculator.GetChargeAmount(schemeType, producerType);

            Assert.Equal(result, chargeBand);
        }

        [Fact]
        public async Task GetChargeAmount_GivenCurrentSubmissionsIsNotMoreThanFiveTonnes_NullShouldBeReturned()
        {
            var schemeType = new schemeType() { approvalNo = "app", complianceYear = ComplianceYear.ToString() };
            var producerType = new producerType() { registrationNo = "no", eeePlacedOnMarketBand = eeePlacedOnMarketBandType.Lessthan5TEEEplacedonmarket };

            var result = await calculator.GetChargeAmount(schemeType, producerType);

            Assert.Null(result);
        }

        private RegisteredProducer RegisteredProducer()
        {
            var id = Guid.NewGuid();
            var registeredProducer = A.Fake<RegisteredProducer>();
            var memberUpload = A.Fake<MemberUpload>();

            A.CallTo(() => registeredProducer.Scheme.Id).Returns(id);
            A.CallTo(() => registeredProducer.ComplianceYear).Returns(ComplianceYear);
            A.CallTo(() => memberUpload.Scheme.Id).Returns(id);
            A.CallTo(() => memberUpload.ComplianceYear).Returns(ComplianceYear);

            var producerSubmission = new ProducerSubmission(
                registeredProducer,
                memberUpload,
                A.Dummy<ProducerBusiness>(),
                A.Dummy<AuthorisedRepresentative>(),
                A.Dummy<DateTime>(),
                A.Dummy<int>(),
                A.Dummy<bool>(),
                A.Dummy<DateTime>(),
                A.Dummy<string>(),
                EEEPlacedOnMarketBandType.Lessthan5TEEEplacedonmarket,
                A.Dummy<SellingTechniqueType>(),
                A.Dummy<Domain.Obligation.ObligationType>(),
                A.Dummy<AnnualTurnOverBandType>(),
                A.Dummy<List<BrandName>>(),
                A.Dummy<List<SICCode>>(),
                A.Dummy<ChargeBandAmount>(),
                A.Dummy<decimal>());

            A.CallTo(() => registeredProducer.CurrentSubmission).Returns(producerSubmission);
            return registeredProducer;
        }
    }
}
