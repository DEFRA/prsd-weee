namespace EA.Weee.RequestHandlers.PCS.MemberRegistration
{
    using Domain;
    using RequestHandlers;

    public class ProducerChargeBandCalculator
    {
        public ProducerCharge CalculateCharge(producerType producer)
        {
            var producerCharge = new ProducerCharge();

            if (producer.status == statusType.I)
            {
                if (producer.eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Lessthan5TEEEplacedonmarket)
                {
                    producerCharge.ChargeBandType = ChargeBandType.E;
                    producerCharge.ChargeAmount = 30;
                }
                else
                {
                    if (producer.annualTurnoverBand == annualTurnoverBandType.Greaterthanonemillionpounds
                        && producer.VATRegistered
                        && producer.eeePlacedOnMarketBand ==
                        eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket)
                    {
                        producerCharge.ChargeBandType = ChargeBandType.A;
                        producerCharge.ChargeAmount = 445;
                    }
                    else if (producer.annualTurnoverBand == annualTurnoverBandType.Lessthanorequaltoonemillionpounds
                             && producer.VATRegistered
                             && producer.eeePlacedOnMarketBand ==
                             eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket)
                    {
                        producerCharge.ChargeBandType = ChargeBandType.B;
                        producerCharge.ChargeAmount = 210;
                    }
                    else if (producer.annualTurnoverBand == annualTurnoverBandType.Greaterthanonemillionpounds
                             && producer.VATRegistered == false
                             && producer.eeePlacedOnMarketBand ==
                             eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket)
                    {
                        producerCharge.ChargeBandType = ChargeBandType.D;
                        producerCharge.ChargeAmount = 30;
                    }
                    else if (producer.annualTurnoverBand == annualTurnoverBandType.Lessthanorequaltoonemillionpounds
                             && producer.VATRegistered == false
                             && producer.eeePlacedOnMarketBand ==
                             eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket)
                    {
                        producerCharge.ChargeBandType = ChargeBandType.C;
                        producerCharge.ChargeAmount = 30;
                    }
                }
            }
            return producerCharge;
        }
    }
}
