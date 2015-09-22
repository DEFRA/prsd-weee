namespace EA.Weee.RequestHandlers.Tests.Unit.SchemaExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Weee.Domain;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "This is the naming used in the XML file.")]
    public class producerTypeTests
    {
        [Fact]
        public void GetProducerChargeBand_Lessthan5TEEEplacedonmarket_ReturnsChargeBandE()
        {
            var producer = producerTypeBuilder.WithEeePlacedOnMarketBand(eeePlacedOnMarketBandType.Lessthan5TEEEplacedonmarket);

            Assert.Equal(ChargeBandType.E, producer.GetProducerChargeBand());
        }

        [Fact]
        public void GetProducerChargeBand_Morethanorequalto5TEEEplacedonmarket_GreaterthanonemillionpoundsTurnover_VATRegistered_ReturnsChargeBandA()
        {
            var producer = producerTypeBuilder.WithAnnualTurnoverBandVATRegistered(annualTurnoverBandType.Greaterthanonemillionpounds, true);

            Assert.Equal(ChargeBandType.A, producer.GetProducerChargeBand());
        }

        [Fact]
        public void GetProducerChargeBand_Morethanorequalto5TEEEplacedonmarket_GreaterthanonemillionpoundsTurnover_NotVATRegistered_ReturnsChargeBandD()
        {
            var producer = producerTypeBuilder.WithAnnualTurnoverBandVATRegistered(annualTurnoverBandType.Greaterthanonemillionpounds, false);

            Assert.Equal(ChargeBandType.D, producer.GetProducerChargeBand());
        }

        [Fact]
        public void GetProducerChargeBand_Morethanorequalto5TEEEplacedonmarket_Lessthanorequaltoonemillionpounds_VATRegistered_ReturnsChargeBandB()
        {
            var producer = producerTypeBuilder.WithAnnualTurnoverBandVATRegistered(annualTurnoverBandType.Lessthanorequaltoonemillionpounds, true);

            Assert.Equal(ChargeBandType.B, producer.GetProducerChargeBand());
        }

        [Fact]
        public void GetProducerChargeBand_Morethanorequalto5TEEEplacedonmarket_Lessthanorequaltoonemillionpounds_NotVATRegistered_ReturnsChargeBandC()
        {
            var producer = producerTypeBuilder.WithAnnualTurnoverBandVATRegistered(annualTurnoverBandType.Lessthanorequaltoonemillionpounds, false);

            Assert.Equal(ChargeBandType.C, producer.GetProducerChargeBand());
        }

        private class producerTypeBuilder
        {
            private eeePlacedOnMarketBandType eeePlacedOnMarketBand;
            private annualTurnoverBandType annualTurnoverBand;
            private bool vatRegistered;

            public producerTypeBuilder()
            {
                eeePlacedOnMarketBand = eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket;
                annualTurnoverBand = annualTurnoverBandType.Lessthanorequaltoonemillionpounds;
            }

            public producerType Build()
            {
                var producer = new producerType()
                {
                    eeePlacedOnMarketBand = this.eeePlacedOnMarketBand,
                    VATRegistered = this.vatRegistered,
                    annualTurnoverBand = this.annualTurnoverBand
                };

                return producer;
            }

            public static producerType WithEeePlacedOnMarketBand(eeePlacedOnMarketBandType eeePlacedOnMarketBand)
            {
                var builder = new producerTypeBuilder();
                builder.eeePlacedOnMarketBand = eeePlacedOnMarketBand;

                return builder.Build();
            }

            public static producerType WithAnnualTurnoverBandVATRegistered(annualTurnoverBandType annualTurnoverBand, bool vatRegistered)
            {
                var builder = new producerTypeBuilder();
                builder.annualTurnoverBand = annualTurnoverBand;
                builder.vatRegistered = vatRegistered;

                return builder.Build();
            }
        }
    }
}
