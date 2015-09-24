namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme.MemberRegistration
{
    using System;
    using System.Collections.Generic;
    using DataAccess;
    using Domain;
    using Domain.Producer;
    using Domain.Scheme;
    using FakeItEasy;
    using RequestHandlers.Scheme.MemberRegistration;
    using Weee.Tests.Core;
    using Xml.Schemas;
    using Xunit;

    public class ProducerChargeBandCalculatorTests
    {
        private const int SomeComplianceYear = 2016;
        private const string SomeRegistrationNumber = "WEE/AB1234CD";

        private readonly DbContextHelper dbHelper = new DbContextHelper();
        
        private readonly ProducerChargeBand fakeA = new ProducerChargeBand("A", 25);
        private readonly ProducerChargeBand fakeB = new ProducerChargeBand("B", 16);
        private readonly ProducerChargeBand fakeC = new ProducerChargeBand("C", 9);
        private readonly ProducerChargeBand fakeD = new ProducerChargeBand("D", 4);
        private readonly ProducerChargeBand fakeE = new ProducerChargeBand("E", 1);

        [Fact]
        public void ProducerChargeBandCalculator_BandIncreases_IncreaseCharge()
        {
            var context = GetContextWithFakeChargeBands();
            A.CallTo(() => context.Producers).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Producer>
            {
                MakeSubmittedProducer(SomeComplianceYear, SomeRegistrationNumber, fakeE.Amount)
            }));

            var calculator = new ProducerChargeBandCalculator(context);

            var producer = GetAmendingProducerType(SomeRegistrationNumber, ChargeBandType.A);
            var producerCharge = calculator.CalculateCharge(producer, SomeComplianceYear);

            Assert.Equal(fakeA.Amount - fakeE.Amount, producerCharge.ChargeAmount);
        }

        [Fact]
        public void ProducerChargeBandCalculator_BandDecreases_ZeroCharge()
        {
            var context = GetContextWithFakeChargeBands();
            A.CallTo(() => context.Producers).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Producer>
            {
                MakeSubmittedProducer(SomeComplianceYear, SomeRegistrationNumber, fakeB.Amount)
            }));

            var calculator = new ProducerChargeBandCalculator(context);

            var producer = GetAmendingProducerType(SomeRegistrationNumber, ChargeBandType.E);
            var producerCharge = calculator.CalculateCharge(producer, SomeComplianceYear);

            Assert.Equal(0, producerCharge.ChargeAmount);
        }

        [Fact]
        public void ProducerChargeBandCalculator_MultipleAmendmentsToHigher_CorrectCharge()
        {
            var context = GetContextWithFakeChargeBands();
            A.CallTo(() => context.Producers).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Producer>
            {
                MakeSubmittedProducer(SomeComplianceYear, SomeRegistrationNumber, fakeC.Amount), // C
                MakeSubmittedProducer(SomeComplianceYear, SomeRegistrationNumber, 0), // D
                MakeSubmittedProducer(SomeComplianceYear, SomeRegistrationNumber, fakeB.Amount - fakeC.Amount), // B
                MakeSubmittedProducer(SomeComplianceYear, SomeRegistrationNumber, 0) // E
            }));

            var calculator = new ProducerChargeBandCalculator(context);

            var producer = GetAmendingProducerType(SomeRegistrationNumber, ChargeBandType.A);
            var producerCharge = calculator.CalculateCharge(producer, SomeComplianceYear);

            Assert.Equal(fakeA.Amount - fakeB.Amount, producerCharge.ChargeAmount);
        }

        [Fact]
        public void ProducerChargeBandCalculator_MultipleAmendmentsToLower_ZeroCharge()
        {
            var context = GetContextWithFakeChargeBands();
            A.CallTo(() => context.Producers).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Producer>
            {
                MakeSubmittedProducer(SomeComplianceYear, SomeRegistrationNumber, fakeC.Amount), // C
                MakeSubmittedProducer(SomeComplianceYear, SomeRegistrationNumber, 0), // D
                MakeSubmittedProducer(SomeComplianceYear, SomeRegistrationNumber, fakeB.Amount - fakeC.Amount), // B
                MakeSubmittedProducer(SomeComplianceYear, SomeRegistrationNumber, fakeA.Amount - fakeB.Amount) // A
            }));

            var calculator = new ProducerChargeBandCalculator(context);

            var producer = GetAmendingProducerType(SomeRegistrationNumber, ChargeBandType.E);
            var producerCharge = calculator.CalculateCharge(producer, SomeComplianceYear);

            Assert.Equal(0, producerCharge.ChargeAmount);
        }

        [Fact]
        public void ProducerChargeBandCalculator_NoHistory_FullCharge()
        {
            var context = GetContextWithFakeChargeBands();
            A.CallTo(() => context.Producers).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Producer>()));

            var calculator = new ProducerChargeBandCalculator(context);

            var producer = GetAmendingProducerType(SomeRegistrationNumber, ChargeBandType.A);
            var producerCharge = calculator.CalculateCharge(producer, SomeComplianceYear);

            Assert.Equal(fakeA.Amount, producerCharge.ChargeAmount);
        }

        [Fact]
        public void ProducerChargeBandCalculatorTests_InternalConsistencyCheck()
        {
            // if this fails, that indicates that our GetAmendingProducerType method has become wrong
            // or the actual implementation has...

            var context = GetContextWithFakeChargeBands();
            A.CallTo(() => context.Producers).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Producer>()));

            var calculator = new ProducerChargeBandCalculator(context);

            var producerInBandA = GetAmendingProducerType(SomeRegistrationNumber, ChargeBandType.A);
            var producerInBandB = GetAmendingProducerType(SomeRegistrationNumber, ChargeBandType.B);
            var producerInBandC = GetAmendingProducerType(SomeRegistrationNumber, ChargeBandType.C);
            var producerInBandD = GetAmendingProducerType(SomeRegistrationNumber, ChargeBandType.D);
            var producerInBandE = GetAmendingProducerType(SomeRegistrationNumber, ChargeBandType.E);

            Assert.Equal(fakeA.Amount, calculator.CalculateCharge(producerInBandA, SomeComplianceYear).ChargeAmount);
            Assert.Equal(fakeB.Amount, calculator.CalculateCharge(producerInBandB, SomeComplianceYear).ChargeAmount);
            Assert.Equal(fakeC.Amount, calculator.CalculateCharge(producerInBandC, SomeComplianceYear).ChargeAmount);
            Assert.Equal(fakeD.Amount, calculator.CalculateCharge(producerInBandD, SomeComplianceYear).ChargeAmount);
            Assert.Equal(fakeE.Amount, calculator.CalculateCharge(producerInBandE, SomeComplianceYear).ChargeAmount);
        }

        private WeeeContext GetContextWithFakeChargeBands()
        {
            var context = A.Fake<WeeeContext>();

            A.CallTo(() => context.ProducerChargeBands)
                .Returns(dbHelper.GetAsyncEnabledDbSet(new List<ProducerChargeBand>
                {
                    fakeA,
                    fakeB,
                    fakeC,
                    fakeD,
                    fakeE
                }));

            return context;
        }

        private Producer MakeSubmittedProducer(int complianceYear, string regNumber, decimal chargeThisUpdate)
        {
            var fakeMemberUpload = A.Fake<MemberUpload>();
            A.CallTo(() => fakeMemberUpload.IsSubmitted).Returns(true);
            A.CallTo(() => fakeMemberUpload.ComplianceYear).Returns(complianceYear);

            var producer = (Producer)Activator.CreateInstance(typeof(Producer), true);
            typeof(Producer).GetProperty("RegistrationNumber").SetValue(producer, regNumber);
            typeof(Producer).GetProperty("MemberUpload").SetValue(producer, fakeMemberUpload);
            typeof(Producer).GetProperty("ChargeThisUpdate").SetValue(producer, chargeThisUpdate);

            return producer;
        }

        private producerType GetAmendingProducerType(string registrationNumber, ChargeBandType chargeBand)
        {
            var producer = A.Fake<producerType>();

            producer.registrationNo = registrationNumber;
            producer.status = statusType.A;

            if (chargeBand == ChargeBandType.A)
            {
                producer.annualTurnoverBand = annualTurnoverBandType.Greaterthanonemillionpounds;
                producer.eeePlacedOnMarketBand = eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket;
                producer.VATRegistered = true;
            }
            else if (chargeBand == ChargeBandType.B)
            {
                producer.annualTurnoverBand = annualTurnoverBandType.Lessthanorequaltoonemillionpounds;
                producer.eeePlacedOnMarketBand = eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket;
                producer.VATRegistered = true;
            }
            else if (chargeBand == ChargeBandType.C)
            {
                producer.annualTurnoverBand = annualTurnoverBandType.Lessthanorequaltoonemillionpounds;
                producer.eeePlacedOnMarketBand = eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket;
                producer.VATRegistered = false;
            }
            else if (chargeBand == ChargeBandType.D)
            {
                producer.annualTurnoverBand = annualTurnoverBandType.Greaterthanonemillionpounds;
                producer.eeePlacedOnMarketBand = eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket;
                producer.VATRegistered = false;
            }
            else if (chargeBand == ChargeBandType.E)
            {
                producer.annualTurnoverBand = annualTurnoverBandType.Lessthanorequaltoonemillionpounds;
                producer.eeePlacedOnMarketBand = eeePlacedOnMarketBandType.Lessthan5TEEEplacedonmarket;
                producer.VATRegistered = true;
            }
            
            return producer;
        }

        /// <summary>
        /// This test ensures that the charge band is A when the amount of EEE placed on the market per year
        /// is at least 5 Tonnes, the annual turnover is greater than £1,000,000 and the company is VAT registered.
        /// </summary>
        [Fact]
        public void GetProducerChargeBand_Morethanorequalto5TEEEplacedonmarket_GreaterthanonemillionpoundsTurnover_VATRegistered_ReturnsChargeBandA()
        {
            // Arrange
            annualTurnoverBandType annualTurnoverBand = annualTurnoverBandType.Greaterthanonemillionpounds;
            bool vatRegistered = true;
            eeePlacedOnMarketBandType eeePlacedOnMarketBand = eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket;

            // Act
            ChargeBandType result = ProducerChargeBandCalculator.GetProducerChargeBand(
                annualTurnoverBand,
                vatRegistered,
                eeePlacedOnMarketBand);

            // Assert
            Assert.Equal(ChargeBandType.A, result);
        }

        /// <summary>
        /// This test ensures that the charge band is B when the amount of EEE placed on the market per year
        /// is at least 5 Tonnes, the annual turnover is at most £1,000,000 and the company is VAT registered.
        /// </summary>
        [Fact]
        public void GetProducerChargeBand_Morethanorequalto5TEEEplacedonmarket_Lessthanorequaltoonemillionpounds_VATRegistered_ReturnsChargeBandB()
        {
            // Arrange
            annualTurnoverBandType annualTurnoverBand = annualTurnoverBandType.Lessthanorequaltoonemillionpounds;
            bool vatRegistered = true;
            eeePlacedOnMarketBandType eeePlacedOnMarketBand = eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket;

            // Act
            ChargeBandType result = ProducerChargeBandCalculator.GetProducerChargeBand(
                annualTurnoverBand,
                vatRegistered,
                eeePlacedOnMarketBand);

            // Assert
            Assert.Equal(ChargeBandType.B, result);
        }

        /// <summary>
        /// This test ensures that the charge band is C when the amount of EEE placed on the market per year
        /// is at least 5 Tonnes, the annual turnover is at most £1,000,000 and the company is not VAT registered.
        /// </summary>
        [Fact]
        public void GetProducerChargeBand_Morethanorequalto5TEEEplacedonmarket_Lessthanorequaltoonemillionpounds_NotVATRegistered_ReturnsChargeBandC()
        {
            // Arrange
            annualTurnoverBandType annualTurnoverBand = annualTurnoverBandType.Lessthanorequaltoonemillionpounds;
            bool vatRegistered = false;
            eeePlacedOnMarketBandType eeePlacedOnMarketBand = eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket;

            // Act
            ChargeBandType result = ProducerChargeBandCalculator.GetProducerChargeBand(
                annualTurnoverBand,
                vatRegistered,
                eeePlacedOnMarketBand);

            // Assert
            Assert.Equal(ChargeBandType.C, result);
        }

        /// <summary>
        /// This test ensures that the charge band is D when the amount of EEE placed on the market per year
        /// is at least 5 Tonnes, the annual turnover is greater than £1,000,000 and the company is not VAT registered.
        /// </summary>
        [Fact]
        public void GetProducerChargeBand_Morethanorequalto5TEEEplacedonmarket_GreaterthanonemillionpoundsTurnover_NotVATRegistered_ReturnsChargeBandD()
        {
            // Arrange
            annualTurnoverBandType annualTurnoverBand = annualTurnoverBandType.Greaterthanonemillionpounds;
            bool vatRegistered = false;
            eeePlacedOnMarketBandType eeePlacedOnMarketBand = eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket;

            // Act
            ChargeBandType result = ProducerChargeBandCalculator.GetProducerChargeBand(
                annualTurnoverBand,
                vatRegistered,
                eeePlacedOnMarketBand);

            // Assert
            Assert.Equal(ChargeBandType.D, result);
        }

        /// <summary>
        /// This test ensures that the charge band is E when the amount of EEE placed on the market per year
        /// is less than 5 Tonnes.
        /// </summary>
        [Fact]
        public void GetProducerChargeBand_Lessthan5TEEEplacedonmarket_ReturnsChargeBandE()
        {
            // Arrange
            annualTurnoverBandType annualTurnoverBand = annualTurnoverBandType.Lessthanorequaltoonemillionpounds;
            bool vatRegistered = false;
            eeePlacedOnMarketBandType eeePlacedOnMarketBand = eeePlacedOnMarketBandType.Lessthan5TEEEplacedonmarket;

            // Act
            ChargeBandType result = ProducerChargeBandCalculator.GetProducerChargeBand(
                annualTurnoverBand,
                vatRegistered,
                eeePlacedOnMarketBand);

            // Assert
            Assert.Equal(ChargeBandType.E, result);
        }

    }
}
