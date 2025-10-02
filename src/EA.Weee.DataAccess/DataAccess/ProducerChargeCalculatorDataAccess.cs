namespace EA.Weee.DataAccess.DataAccess
{
    using Domain.Lookup;
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Weee.DataAccess;

    public class ProducerChargeCalculatorDataAccess : IProducerChargeCalculatorDataAccess
    {
        private readonly WeeeContext context;

        public ProducerChargeCalculatorDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Fetch the charge band amount that applies for the given producer and date.
        /// </summary>
        /// <param name="competentAuthority"></param>
        /// <param name="vatRegistered"></param>
        /// <param name="annualTurnoverBand"></param>
        /// <param name="eeePlacedOnMarketBand"></param>
        /// <param name="complianceYear"></param>
        /// <param name="asOfUtc"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<ChargeBandAmount> GetChargeBandAmountAsync(
            CompetentAuthorityType competentAuthority,
            bool vatRegistered,
            AnnualTurnoverBand annualTurnoverBand,
            EEEPlacedOnMarketBand eeePlacedOnMarketBand,
            int complianceYear,
            DateTime asOfUtc)
        {
            // Query the ChargeBandAmounts table for the first record that matches the given
            // producer attributes (competent authority, VAT status, annualTurnoverBand, eeePlacedOnMarketBand, 
            // and compliance year) where the EffectiveFrom date is on or before the asOfUtc date.
            // Returns null if no matching record exists.
            var chargeBandAmount = await context.ChargeBandAmounts
                .Where(cba => cba.CompetentAuthority == competentAuthority &&
                            cba.VatRegistered == vatRegistered &&
                            cba.AnnualTurnoverBand == annualTurnoverBand &&
                            cba.EEEPlacedOnMarketBand == eeePlacedOnMarketBand &&
                            cba.ComplianceYear == complianceYear &&
                            cba.EffectiveFrom <= asOfUtc)
                .OrderByDescending(cba => cba.EffectiveFrom)
                .FirstOrDefaultAsync();
            
            return chargeBandAmount;
        }
    }
}