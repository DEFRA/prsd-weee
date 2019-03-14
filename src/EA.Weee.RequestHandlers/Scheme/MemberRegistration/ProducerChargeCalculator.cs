namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using System;
    using Domain.Lookup;
    using Xml.MemberRegistration;

    public class ProducerChargeCalculator : IProducerChargeCalculator
    {
        private readonly IProducerChargeCalculatorDataAccess dataAccess;
        private readonly IProducerChargeBandCalculatorChooser producerChargeBandCalculatorChooser;
        public ProducerChargeCalculator(IProducerChargeCalculatorDataAccess dataAccess, 
            IProducerChargeBandCalculator producerChargeBandCalculator, 
            IProducerChargeBandCalculatorChooser producerChargeBandCalculatorChooser)
        {
            this.dataAccess = dataAccess;
            this.producerChargeBandCalculatorChooser = producerChargeBandCalculatorChooser;
        }

        public ProducerCharge CalculateCharge(schemeType scheme, producerType producer, int complianceYear)
        {
            ProducerCharge producerCharge = GetProducerCharge(scheme, producer, complianceYear);

            // commented out as per Acceptance criteria PBI: 68472:SROC: Update EA charge bands * BUSINESS Q
            //if (producer.status == statusType.A)
            //{
            //    decimal sumOfExistingCharges = dataAccess.FetchSumOfExistingCharges(scheme.approvalNo, producer.registrationNo, complianceYear);

            //    producerCharge.Amount = Math.Max(0, producerCharge.ChargeBandAmount.Amount - sumOfExistingCharges);
            //}

            return producerCharge;
        }

        private ProducerCharge GetProducerCharge(schemeType scheme, producerType producer, int complianceYear)
        {
            var calculator = producerChargeBandCalculatorChooser.GetCalculator(scheme, producer, complianceYear);

            ChargeBand chargeBandType = calculator.GetProducerChargeBand(producer);

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
