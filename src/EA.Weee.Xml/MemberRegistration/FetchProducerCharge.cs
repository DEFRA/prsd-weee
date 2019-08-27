namespace EA.Weee.Xml.MemberRegistration
{
    using DataAccess.DataAccess;
    using Domain.Lookup;
    using System.Threading.Tasks;

    public class FetchProducerCharge : IFetchProducerCharge
    {
        private readonly IProducerChargeCalculatorDataAccess producerChargeCalculatorDataAccess;

        public FetchProducerCharge(IProducerChargeCalculatorDataAccess producerChargeCalculatorDataAccess)
        {
            this.producerChargeCalculatorDataAccess = producerChargeCalculatorDataAccess;
        }

        public async Task<ProducerCharge> GetCharge(ChargeBand chargeBand)
        {
            var currentChargeBandAmount = await producerChargeCalculatorDataAccess.FetchCurrentChargeBandAmount(chargeBand);

            return new ProducerCharge()
            {
                ChargeBandAmount = currentChargeBandAmount,
                Amount = currentChargeBandAmount.Amount
            };
        }
    }
}
