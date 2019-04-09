namespace EA.Weee.Sroc.Migration.OverrideImplementations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.Lookup;
    using RequestHandlers.Factories;
    using RequestHandlers.Scheme.MemberRegistration;

    public class MigrationProducerChargeCalculatorDataAccess : IMigrationProducerChargeCalculatorDataAccess
    {
        private readonly WeeeMigrationContext context;

        private Dictionary<ChargeBand, ChargeBandAmount> currentProducerChargeBandAmounts;

        public MigrationProducerChargeCalculatorDataAccess(WeeeMigrationContext context)
        {
            this.context = context;
        }

        public ChargeBandAmount FetchCurrentChargeBandAmount(ChargeBand chargeBandType)
        {
            if (currentProducerChargeBandAmounts == null)
            {
                /* For now we only have one charge band amount for each type, so
                * we can fetch them all. When new charge band amounts are added,
                * this query will need to select only latest charge band amount
                * for each charge band type.
                */
                currentProducerChargeBandAmounts = context
                    .ChargeBandAmounts
                    .ToDictionary(pcb => pcb.ChargeBand, pcb => pcb);
            }

            return currentProducerChargeBandAmounts[chargeBandType];
        }
    }
}
