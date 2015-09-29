namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DataAccess;
    using Domain;
    using Domain.Producer;
    using EA.Weee.Xml;
    using RequestHandlers;
    using Xml.Schemas;

    public class ProducerChargeCalculator : IProducerChargeCalculator
    {
        private readonly IProducerChargeCalculatorDataAccess dataAccess;
        private readonly IProducerChargeBandCalculator producerChargeBandCalculator;

        public ProducerChargeCalculator(IProducerChargeCalculatorDataAccess dataAccess, IProducerChargeBandCalculator producerChargeBandCalculator)
        {
            this.dataAccess = dataAccess;
            this.producerChargeBandCalculator = producerChargeBandCalculator;
        }

        public ProducerCharge CalculateCharge(producerType producer, int complianceYear)
        {
            ProducerCharge producerCharge = GetProducerCharge(producer);

            if (producer.status == statusType.A)
            {
                decimal sumOfExistingCharges = dataAccess.FetchSumOfExistingCharges(producer.registrationNo, complianceYear);

                producerCharge.ChargeAmount = Math.Max(0, producerCharge.ChargeAmount - sumOfExistingCharges);
            }

            return producerCharge;
        }

        private ProducerCharge GetProducerCharge(producerType producer)
        {
            ChargeBandType chargeBandType = producerChargeBandCalculator.GetProducerChargeBand(
                producer.annualTurnoverBand,
                producer.VATRegistered,
                producer.eeePlacedOnMarketBand);

            decimal chargeAmount = dataAccess.FetchChargeBandAmount(chargeBandType);

            return new ProducerCharge()
            {
                ChargeBandType = chargeBandType,
                ChargeAmount = chargeAmount
            };
        }
    }
}
