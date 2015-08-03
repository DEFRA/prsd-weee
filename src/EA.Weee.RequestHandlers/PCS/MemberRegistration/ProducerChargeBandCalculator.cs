namespace EA.Weee.RequestHandlers.PCS.MemberRegistration
{
    using System.Collections.Generic;
    using System.Linq;
    using DataAccess;
    using Domain;
    using Domain.Producer;
    using RequestHandlers;

    public class ProducerChargeBandCalculator
    {
        private readonly WeeeContext context;
        private readonly List<ProducerChargeBand> producerChargeBands;

        public ProducerChargeBandCalculator(WeeeContext context)
        {
            this.context = context;
            producerChargeBands = context.ProducerChargeBands.ToList();
        }

        public ProducerCharge CalculateCharge(producerType producer)
        {
            var producerCharge = new ProducerCharge();

            if (producer.status == statusType.I)
            {
                if (producer.eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Lessthan5TEEEplacedonmarket)
                {
                    producerCharge.ChargeBandType = ChargeBandType.E;
                    producerCharge.ChargeAmount = GetProducerChargeAmount(ChargeBandType.E);
                }
                else
                {
                    if (producer.annualTurnoverBand == annualTurnoverBandType.Greaterthanonemillionpounds
                        && producer.VATRegistered
                        && producer.eeePlacedOnMarketBand ==
                        eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket)
                    {
                        producerCharge.ChargeBandType = ChargeBandType.A;
                        producerCharge.ChargeAmount = GetProducerChargeAmount(ChargeBandType.A);
                    }
                    else if (producer.annualTurnoverBand == annualTurnoverBandType.Lessthanorequaltoonemillionpounds
                             && producer.VATRegistered
                             && producer.eeePlacedOnMarketBand ==
                             eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket)
                    {
                        producerCharge.ChargeBandType = ChargeBandType.B;
                        producerCharge.ChargeAmount = GetProducerChargeAmount(ChargeBandType.B);
                    }
                    else if (producer.annualTurnoverBand == annualTurnoverBandType.Greaterthanonemillionpounds
                             && producer.VATRegistered == false
                             && producer.eeePlacedOnMarketBand ==
                             eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket)
                    {
                        producerCharge.ChargeBandType = ChargeBandType.D;
                        producerCharge.ChargeAmount = GetProducerChargeAmount(ChargeBandType.D);
                    }
                    else if (producer.annualTurnoverBand == annualTurnoverBandType.Lessthanorequaltoonemillionpounds
                             && producer.VATRegistered == false
                             && producer.eeePlacedOnMarketBand ==
                             eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket)
                    {
                        producerCharge.ChargeBandType = ChargeBandType.C;
                        producerCharge.ChargeAmount = GetProducerChargeAmount(ChargeBandType.C);
                    }
                }
            }
            return producerCharge;
        }

        public decimal GetProducerChargeAmount(ChargeBandType chargeBandType)
        {
            return producerChargeBands.Single(pc => pc.Name == chargeBandType.DisplayName).Amount;
        }
    }
}
