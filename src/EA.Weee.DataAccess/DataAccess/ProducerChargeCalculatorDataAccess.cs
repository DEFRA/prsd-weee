namespace EA.Weee.DataAccess.DataAccess
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.Lookup;
    using Weee.DataAccess;

    public class ProducerChargeCalculatorDataAccess : IProducerChargeCalculatorDataAccess
    {
        private readonly WeeeContext context;

        private Dictionary<ChargeBand, ChargeBandAmount> currentProducerChargeBandAmounts;

        public ProducerChargeCalculatorDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<ChargeBandAmount> FetchCurrentChargeBandAmount(ChargeBand chargeBandType)
        {
            if (currentProducerChargeBandAmounts == null)
            {
                /* For now we only have one charge band amount for each type, so
                * we can fetch them all. When new charge band amounts are added,
                * this query will need to select only latest charge band amount
                * for each charge band type.
                */
                currentProducerChargeBandAmounts = await context
                    .ChargeBandAmounts
                    .ToDictionaryAsync(pcb => pcb.ChargeBand, pcb => pcb);
            }

            return currentProducerChargeBandAmounts[chargeBandType];
        }
    }
}