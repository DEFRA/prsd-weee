namespace EA.Weee.Sroc.Migration.OverrideImplementations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.Scheme;
    using Xml.MemberRegistration;

    public class MigrationProducerChargeBandCalculatorChooser : IMigrationProducerChargeBandCalculatorChooser
    {
        private readonly IEnumerable<IMigrationChargeBandCalculator> chargeChargeBandCalculators;

        public MigrationProducerChargeBandCalculatorChooser(IEnumerable<IMigrationChargeBandCalculator> chargeChargeBandCalculators)
        {
            this.chargeChargeBandCalculators = chargeChargeBandCalculators;
        }

        public async Task<ProducerCharge> GetProducerChargeBand(schemeType scheme, producerType producer, MemberUpload upload, string name)
        {
            var calculator = chargeChargeBandCalculators.FirstOrDefault(c => c.IsMatch(scheme, producer, upload, name));

            if (calculator == null)
            {
                throw new ApplicationException(string.Format("No charge band calculator found for scheme {0} and producer {1}", scheme.approvalNo, producer.tradingName));
            }

            return await calculator.GetProducerChargeBand(scheme, producer, upload.CreatedDate);
        }
    }
}
