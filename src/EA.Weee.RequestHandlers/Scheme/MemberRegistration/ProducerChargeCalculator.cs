namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using Domain.Lookup;
    using System;
    using Xml.MemberRegistration;

    public class ProducerChargeCalculator : IProducerChargeCalculator
    {
        private readonly IProducerChargeCalculatorDataAccess dataAccess;
        private readonly IProducerChargeBandCalculator producerChargeBandCalculator;

        public ProducerChargeCalculator(IProducerChargeCalculatorDataAccess dataAccess, IProducerChargeBandCalculator producerChargeBandCalculator)
        {
            this.dataAccess = dataAccess;
            this.producerChargeBandCalculator = producerChargeBandCalculator;
        }

        public ProducerCharge CalculateCharge(string schemeApprovalNumber, producerType producer, int complianceYear)
        {
            ProducerCharge producerCharge = GetProducerCharge(producer);

            if (producer.status == statusType.A)
            {
                decimal sumOfExistingCharges = dataAccess.FetchSumOfExistingCharges(schemeApprovalNumber, producer.registrationNo, complianceYear);

                producerCharge.Amount = Math.Max(0, producerCharge.ChargeBandAmount.Amount - sumOfExistingCharges);
            }

            return producerCharge;
        }

        private ProducerCharge GetProducerCharge(producerType producer)
        {
            ChargeBand chargeBandType = producerChargeBandCalculator.GetProducerChargeBand(
                producer.annualTurnoverBand,
                producer.VATRegistered,
                producer.eeePlacedOnMarketBand);

            ChargeBandAmount currentChargeBandAmount =
                dataAccess.FetchCurrentChargeBandAmount(chargeBandType);

            return new ProducerCharge()
            {
                ChargeBandAmount = currentChargeBandAmount,
                Amount = currentChargeBandAmount.Amount
            };
        }
    }
}
