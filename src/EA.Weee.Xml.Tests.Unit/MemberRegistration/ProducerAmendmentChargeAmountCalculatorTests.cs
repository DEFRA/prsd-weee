namespace EA.Weee.Xml.Tests.Unit.MemberRegistration
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.Lookup;
    using Domain.Producer;
    using Domain.Producer.Classfication;
    using Domain.Producer.Classification;
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
        public async void GetProducerChargeBand_GivenProducerSubmission_RegisteredProducerShouldBeRetrieved()
        {
            var schemeType = new schemeType() { approvalNo = "app", complianceYear = ComplianceYear.ToString() };
            var producerType = new producerType() { registrationNo = "no" };

            await calculator.GetProducerChargeBand(schemeType, producerType);

            A.CallTo(() => registeredProducerDataAccess.GetProducerRegistration(producerType.registrationNo, ComplianceYear, schemeType.approvalNo))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void GetProducerChargeBand_GivenProducerSubmission_PreviousAmendmentChargeShouldBeRetrieved()
        {
            var schemeType = new schemeType() { approvalNo = "app", complianceYear = ComplianceYear.ToString() };
            var producerType = new producerType() { registrationNo = "no" };

            await calculator.GetProducerChargeBand(schemeType, producerType);

            A.CallTo(() => registeredProducerDataAccess.HasPreviousAmendmentCharge(producerType.registrationNo, ComplianceYear, schemeType.approvalNo))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void GetProducerChargeBand_GivenCurrentSubmissionsIsMoreThanFiveTonnesAndPreviousProducerSubmissionWasLessThanFiveTonnes_EnvironmentAgencyChargeShouldBeCalculated()
        {
            var schemeType = new schemeType() { approvalNo = "app", complianceYear = ComplianceYear.ToString() };
            var producerType = new producerType() { registrationNo = "no", eeePlacedOnMarketBand = eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket };

            A.CallTo(() => registeredProducerDataAccess.GetProducerRegistration(producerType.registrationNo, ComplianceYear, schemeType.approvalNo))
                .Returns(RegisteredProducer());

            var result = await calculator.GetProducerChargeBand(schemeType, producerType);

            A.CallTo(() => environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(A<schemeType>._, producerType)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void GetProducerChargeBand_GivenCurrentSubmissionsIsMoreThanFiveTonnesAndPreviousProducerSubmissionWasLessThanFiveTonnes_EnvironmentAgencyChargeShouldBeReturned()
        {
            var schemeType = new schemeType() { approvalNo = "app", complianceYear = ComplianceYear.ToString() };
            var producerType = new producerType() { registrationNo = "no", eeePlacedOnMarketBand = eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket };
            var chargeBand = ChargeBand.A;

            A.CallTo(() => registeredProducerDataAccess.GetProducerRegistration(producerType.registrationNo, ComplianceYear, schemeType.approvalNo))
                .Returns(RegisteredProducer());
            A.CallTo(() => environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(A<schemeType>._, producerType)).Returns(chargeBand);

            var result = await calculator.GetProducerChargeBand(schemeType, producerType);

            Assert.Equal(result, chargeBand);
        }

        [Fact]
        public async void GetProducerChargeBand_GivenCurrentSubmissionsIsNotMoreThanFiveTonnes_NotApplicableChargeShouldBeReturned()
        {
            var schemeType = new schemeType() { approvalNo = "app", complianceYear = ComplianceYear.ToString() };
            var producerType = new producerType() { registrationNo = "no", eeePlacedOnMarketBand = eeePlacedOnMarketBandType.Lessthan5TEEEplacedonmarket };

            var result = await calculator.GetProducerChargeBand(schemeType, producerType);

            Assert.Equal(result, ChargeBand.NA);
        }

        [Fact]
        public async void GetProducerChargeBand_GivenPreviousAmendmentChargeAndChargeQualifies_CalculatorShouldNotBeCalled()
        {
            var schemeType = new schemeType() { approvalNo = "app", complianceYear = ComplianceYear.ToString() };
            var producerType = new producerType() { registrationNo = "no", eeePlacedOnMarketBand = eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket };
            var chargeBand = ChargeBand.A;

            A.CallTo(() => registeredProducerDataAccess.GetProducerRegistration(producerType.registrationNo, ComplianceYear, schemeType.approvalNo))
                .Returns(RegisteredProducer());
            A.CallTo(() => environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(A<schemeType>._, producerType)).Returns(chargeBand);
            A.CallTo(() => registeredProducerDataAccess.HasPreviousAmendmentCharge(A<string>._, A<int>._, A<string>._)).Returns(true);

            var result = await calculator.GetProducerChargeBand(schemeType, producerType);

            A.CallTo(() => environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(A<schemeType>._, A<producerType>._)).WithAnyArguments().MustNotHaveHappened();
        }

        [Fact]
        public async void GetProducerChargeBand_GivenNoPreviousAmendmentCharge_NotApplicableChargeShouldBeReturned()
        {
            var schemeType = new schemeType() { approvalNo = "app", complianceYear = ComplianceYear.ToString() };
            var producerType = new producerType() { registrationNo = "no", eeePlacedOnMarketBand = eeePlacedOnMarketBandType.Lessthan5TEEEplacedonmarket };

            A.CallTo(() => registeredProducerDataAccess.HasPreviousAmendmentCharge(A<string>._, A<int>._, A<string>._)).Returns(false);

            var result = await calculator.GetProducerChargeBand(schemeType, producerType);

            Assert.Equal(result, ChargeBand.NA);
        }

        [Fact]
        public async void GetProducerChargeBand_GivenAmendmentCannotFindPreviousSubmission_NotApplicableChargeShouldBeReturned()
        {
            var schemeType = new schemeType() { approvalNo = "app", complianceYear = ComplianceYear.ToString() };
            var producerType = new producerType() { registrationNo = "no", eeePlacedOnMarketBand = eeePlacedOnMarketBandType.Lessthan5TEEEplacedonmarket };

            A.CallTo(() => registeredProducerDataAccess.GetProducerRegistration(A<string>._, A<int>._, A<string>._)).Returns((RegisteredProducer)null);

            var result = await calculator.GetProducerChargeBand(schemeType, producerType);

            Assert.Equal(result, ChargeBand.NA);
        }

        [Fact]
        public async void GetProducerChargeBand_GivenAmendmentAndPreviousSubmissionCurrentSubmissionIsNull_NotApplicableChargeShouldBeReturned()
        {
            var schemeType = new schemeType() { approvalNo = "app", complianceYear = ComplianceYear.ToString() };
            var producerType = new producerType() { registrationNo = "no", eeePlacedOnMarketBand = eeePlacedOnMarketBandType.Lessthan5TEEEplacedonmarket };
            var producer = new RegisteredProducer(A.Dummy<string>(), A.Dummy<int>(), A.Dummy<Scheme>());

            A.CallTo(() => registeredProducerDataAccess.GetProducerRegistration(A<string>._, A<int>._, A<string>._)).Returns(producer);

            var result = await calculator.GetProducerChargeBand(schemeType, producerType);

            Assert.Equal(result, ChargeBand.NA);
        }

        [Fact]
        public void IsMatch_GivenProducerIsAmendement_TrueShouldBeReturned()
        {
            var producer = new producerType() { status = statusType.A };

            var result = calculator.IsMatch(A.Dummy<schemeType>(), producer);

            Assert.True(result);
        }

        [Fact]
        public void IsMatch_GivenProducerIsInsert_FalseShouldBeReturned()
        {
            var producer = new producerType() { status = statusType.I };

            var result = calculator.IsMatch(A.Dummy<schemeType>(), producer);

            Assert.False(result);
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
                A.Dummy<decimal>(),
                A.Dummy<StatusType>());

            A.CallTo(() => registeredProducer.CurrentSubmission).Returns(producerSubmission);
            return registeredProducer;
        }
    }
}
