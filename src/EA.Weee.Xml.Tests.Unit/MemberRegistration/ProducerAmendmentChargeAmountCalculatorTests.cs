namespace EA.Weee.Xml.Tests.Unit.MemberRegistration
{
    using Domain.Lookup;
    using Domain.Producer;
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
        public void GetChargeAmount_GivenProducerSubmission_RegisteredProducerShouldBeRetrieved()
        {
            var schemeType = new schemeType() { approvalNo = "app", complianceYear = ComplianceYear.ToString() };
            var producerType = new producerType() { registrationNo = "no" };

            calculator.GetChargeAmount(schemeType, producerType);

            A.CallTo(() => registeredProducerDataAccess.GetProducerRegistration(producerType.registrationNo, ComplianceYear, schemeType.approvalNo))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void GetChargeAmount_GivenCurrentSubmissionsIsMoreThanFiveTonnesAndPreviousProducerSubmissionWasLessThanFiveTonnes_EnvironmentAgencyChargeShouldBeCalculated()
        {
            var schemeType = new schemeType() { approvalNo = "app", complianceYear = ComplianceYear.ToString() };
            var producerType = new producerType() { registrationNo = "no", eeePlacedOnMarketBand = eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket };

            var producerSubmission = A.Fake<ProducerSubmission>();
            A.CallTo(() => producerSubmission.EEEPlacedOnMarketBandType).Returns((int)eeePlacedOnMarketBandType.Lessthan5TEEEplacedonmarket);

            var result = calculator.GetChargeAmount(schemeType, producerType);

            A.CallTo(() => environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(producerType)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void GetChargeAmount_GivenCurrentSubmissionsIsMoreThanFiveTonnesAndPreviousProducerSubmissionWasLessThanFiveTonnes_EnvironmentAgencyChargeShouldBeReturned()
        {
            var schemeType = new schemeType() { approvalNo = "app", complianceYear = ComplianceYear.ToString() };
            var producerType = new producerType() { registrationNo = "no", eeePlacedOnMarketBand = eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket };
            var chargeBand = A.Fake<ChargeBand>();

            var producerSubmission = A.Fake<ProducerSubmission>();
            A.CallTo(() => producerSubmission.EEEPlacedOnMarketBandType).Returns((int)eeePlacedOnMarketBandType.Lessthan5TEEEplacedonmarket);
            A.CallTo(() => environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(producerType)).Returns(chargeBand);

            var result = calculator.GetChargeAmount(schemeType, producerType);

            Assert.Equal(result, chargeBand);
        }

        [Fact]
        public void GetChargeAmount_GivenCurrentSubmissionsIsNotMoreThanFiveTonnes_NullShouldBeReturned()
        {
            var schemeType = new schemeType() { approvalNo = "app", complianceYear = ComplianceYear.ToString() };
            var producerType = new producerType() { registrationNo = "no", eeePlacedOnMarketBand = eeePlacedOnMarketBandType.Lessthan5TEEEplacedonmarket };

            var result = calculator.GetChargeAmount(schemeType, producerType);

            Assert.Null(result);
        }
    }
}
