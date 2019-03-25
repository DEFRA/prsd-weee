namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using System;
    using System.Threading.Tasks;
    using Domain.Lookup;
    using Xml.MemberRegistration;

    public class ProducerChargeCalculator : IProducerChargeCalculator
    {
        private readonly IProducerChargeCalculatorDataAccess dataAccess;
        private readonly IProducerChargeBandCalculatorChooser producerChargeBandCalculatorChooser;

        public ProducerChargeCalculator(IProducerChargeCalculatorDataAccess dataAccess, 
            IProducerChargeBandCalculatorChooser producerChargeBandCalculatorChooser)
        {
            this.dataAccess = dataAccess;
            this.producerChargeBandCalculatorChooser = producerChargeBandCalculatorChooser;
        }

        public ProducerCharge CalculateCharge(schemeType scheme, producerType producer)
        {
            var chargeBand = Task.Run(() => producerChargeBandCalculatorChooser.GetProducerChargeBand(scheme, producer)).Result;

            if (chargeBand.HasValue)
            {
                var currentChargeBandAmount = dataAccess.FetchCurrentChargeBandAmount(chargeBand.Value);

                return new ProducerCharge()
                {
                    ChargeBandAmount = currentChargeBandAmount,
                    Amount = currentChargeBandAmount.Amount
                };
            }

            return null;
        }
    }
}
