namespace EA.Weee.Xml.MemberRegistration
{
    using DataAccess.DataAccess;
    using Domain.Lookup;
    using System;
    using System.Threading.Tasks;

    public class FetchProducerCharge : IFetchProducerCharge
    {
        private readonly IProducerChargeCalculatorDataAccess producerChargeCalculatorDataAccess;

        public FetchProducerCharge(IProducerChargeCalculatorDataAccess producerChargeCalculatorDataAccess)
        {
            this.producerChargeCalculatorDataAccess = producerChargeCalculatorDataAccess;
        }

        public async Task<ChargeBandAmount> GetChargeBandAmountAsync(
            CompetentAuthorityType competentAuthority,
            bool vatRegistered,
            AnnualTurnoverBand annualTurnoverBand,
            EEEPlacedOnMarketBand eeePlacedOnMarketBand,
            int complianceYear,
            DateTime asOfUtc)
        {
            return await producerChargeCalculatorDataAccess.GetChargeBandAmountAsync(
                competentAuthority,
                vatRegistered,
                annualTurnoverBand,
                eeePlacedOnMarketBand,
                complianceYear,
                asOfUtc);
        }
    }
}
