namespace EA.Weee.Sroc.Migration.OverrideImplementations
{
    using DataAccess.DataAccess;
    using Domain.Lookup;
    using Xml.MemberRegistration;

    public class MigrationFetchProducerCharge : IMigrationFetchProducerCharge
    {
        private readonly IMigrationProducerChargeCalculatorDataAccess producerChargeCalculatorDataAccess;

        public MigrationFetchProducerCharge(IMigrationProducerChargeCalculatorDataAccess producerChargeCalculatorDataAccess)
        {
            this.producerChargeCalculatorDataAccess = producerChargeCalculatorDataAccess;
        }

        public ProducerCharge GetCharge(ChargeBand chargeBand)
        {
            var currentChargeBandAmount = producerChargeCalculatorDataAccess.FetchCurrentChargeBandAmount(chargeBand);

            return new ProducerCharge()
            {
                ChargeBandAmount = currentChargeBandAmount,
                Amount = currentChargeBandAmount.Amount
            };
        }
    }
}
