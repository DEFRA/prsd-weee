namespace EA.Weee.Xml.MemberRegistration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.Lookup;

    public class ProducerChargeBandCalculatorChooser : IProducerChargeBandCalculatorChooser
    {
        private readonly IEnumerable<IProducerChargeBandCalculator> chargeChargeBandCalculators;

        public ProducerChargeBandCalculatorChooser(IEnumerable<IProducerChargeBandCalculator> chargeChargeBandCalculators)
        {
            this.chargeChargeBandCalculators = chargeChargeBandCalculators;
        }

        public async Task<ChargeBand?> GetProducerChargeBand(schemeType scheme, producerType producer)
        {
            var calculator = chargeChargeBandCalculators.FirstOrDefault(c => c.IsMatch(scheme, producer));

            if (calculator == null)
            {
                throw new ApplicationException(string.Format("No charge band calculator found for scheme {0} and producer {1}", scheme.approvalNo, producer.tradingName));
            }

            return await calculator.GetProducerChargeBand(scheme, producer);
        }
    }
}
