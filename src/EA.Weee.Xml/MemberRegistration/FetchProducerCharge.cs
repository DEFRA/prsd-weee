namespace EA.Weee.Xml.MemberRegistration
{
    using DataAccess.DataAccess;
    using Domain.Lookup;
    using System;
    using System.Threading.Tasks;

    public class FetchProducerCharge : IFetchProducerCharge
    {
        private readonly IProducerChargeCalculatorDataAccess producerChargeCalculatorDataAccess;
        private readonly IOnlineMarketplaceChargeDataAccess onlineMarketplaceChargeDataAccess;

        public FetchProducerCharge(IProducerChargeCalculatorDataAccess producerChargeCalculatorDataAccess,
                                   IOnlineMarketplaceChargeDataAccess onlineMarketplaceChargeDataAccess)
        {
            this.producerChargeCalculatorDataAccess = producerChargeCalculatorDataAccess;
            this.onlineMarketplaceChargeDataAccess = onlineMarketplaceChargeDataAccess;
        }

        public async Task<ProducerCharge> GetChargeBandAmountAsyncLegacy(ChargeBand chargeBand)
        {
            var currentChargeBandAmount = await producerChargeCalculatorDataAccess.GetChargeBandAmountAsyncLegacy(chargeBand);

            return new ProducerCharge()
            {
                ChargeBandAmount = currentChargeBandAmount,
                Amount = currentChargeBandAmount.Amount
            };
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

        public async Task<decimal?> GetOnlineMarketplaceChargeAsync(CompetentAuthorityType competentAuthority, DateTime asOfUtc)
        {
            var charge = await onlineMarketplaceChargeDataAccess.GetChargeAmountAsync(competentAuthority, asOfUtc);
            return charge?.Amount;
        }
    }
}
