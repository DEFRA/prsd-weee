namespace EA.Weee.RequestHandlers.PCS.MemberRegistration
{
    using System.Linq;
    using DataAccess;
    using Domain;
    using RequestHandlers;

    public class ProducerChargeBandCalculator
    {
        public ProducerCharge CalculateCharge(producerType producer, WeeeContext context)
        {
            var producerCharge = new ProducerCharge();

            if (producer.status == statusType.I)
            {
                if (producer.eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Lessthan5TEEEplacedonmarket)
                {
                    producerCharge.ChargeBandType = ChargeBandType.E;
                    producerCharge.ChargeAmount = GetProducerChargeAmount(context, ChargeBandType.E);
                }
                else
                {
                    if (producer.annualTurnoverBand == annualTurnoverBandType.Greaterthanonemillionpounds
                        && producer.VATRegistered
                        && producer.eeePlacedOnMarketBand ==
                        eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket)
                    {
                        producerCharge.ChargeBandType = ChargeBandType.A;
                        producerCharge.ChargeAmount = GetProducerChargeAmount(context, ChargeBandType.A);
                    }
                    else if (producer.annualTurnoverBand == annualTurnoverBandType.Lessthanorequaltoonemillionpounds
                             && producer.VATRegistered
                             && producer.eeePlacedOnMarketBand ==
                             eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket)
                    {
                        producerCharge.ChargeBandType = ChargeBandType.B;
                        producerCharge.ChargeAmount = GetProducerChargeAmount(context, ChargeBandType.B);
                    }
                    else if (producer.annualTurnoverBand == annualTurnoverBandType.Greaterthanonemillionpounds
                             && producer.VATRegistered == false
                             && producer.eeePlacedOnMarketBand ==
                             eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket)
                    {
                        producerCharge.ChargeBandType = ChargeBandType.D;
                        producerCharge.ChargeAmount = GetProducerChargeAmount(context, ChargeBandType.D);
                    }
                    else if (producer.annualTurnoverBand == annualTurnoverBandType.Lessthanorequaltoonemillionpounds
                             && producer.VATRegistered == false
                             && producer.eeePlacedOnMarketBand ==
                             eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket)
                    {
                        producerCharge.ChargeBandType = ChargeBandType.C;
                        producerCharge.ChargeAmount = GetProducerChargeAmount(context, ChargeBandType.C);
                    }
                }
            }
            return producerCharge;
        }

        public decimal GetProducerChargeAmount(WeeeContext context, ChargeBandType chargeBandType)
        {
            return context.ProducerChargeBands.Single(pc => pc.Name == chargeBandType.DisplayName).Amount;
        }
    }
}
